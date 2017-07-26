using NTumbleBit.ClassicTumbler;
using NTumbleBit.PuzzleSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin;
using NTumbleBit.PuzzlePromise;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NBitcoin.SPV;
using NTumbleBit;
using Stratis.Bitcoin.Features.Wallet;
using Wallet = Stratis.Bitcoin.Features.Wallet.Wallet;

namespace Breeze.TumbleBit.Client
{
    public partial class TumblingState : IStateMachine
    {        
        public bool NonCooperative { get; set; }

        private ExternalServices services = new ExternalServices();

        public async Task Update(Session session)
        {            
            int height = this.chain.Tip.Height;
            CycleParameters cycle;
            CyclePhase phase;
            if (session.ClientChannelNegotiation == null)
            {
                cycle = this.TumblerParameters.CycleGenerator.GetRegistratingCycle(height);
                phase = CyclePhase.Registration;
            }
            else
            {
                cycle = session.ClientChannelNegotiation.GetCycle();
                var phases = new CyclePhase[]
                {
                        CyclePhase.Registration,
                        CyclePhase.ClientChannelEstablishment,
                        CyclePhase.TumblerChannelEstablishment,
                        CyclePhase.PaymentPhase,
                        CyclePhase.TumblerCashoutPhase,
                        CyclePhase.ClientCashoutPhase
                };
                if (!phases.Any(p => cycle.IsInPhase(p, height)))
                    return;
                phase = phases.First(p => cycle.IsInPhase(p, height));
            }

            logger.LogInformation("Cycle " + cycle.Start + " in phase " + Enum.GetName(typeof(CyclePhase), phase) + ", ending in " + (cycle.GetPeriods().GetPeriod(phase).End - height) + " blocks");
            var correlation = session.SolverClientSession == null ? 0 : GetCorrelation(session.SolverClientSession.EscrowedCoin.ScriptPubKey);

            FeeRate feeRate = null;
            switch (phase)
            {
                // in the registration phase, Bob asks the tumbler for a voucher.
                // he cannot open it so he gives it to Alice.
                case CyclePhase.Registration:
                    if (session.ClientChannelNegotiation == null)
                    {
                        //Client asks for voucher
                        var voucherResponse = await this.BobClient.AskUnsignedVoucherAsync();
                        //Client ensures he is in the same cycle as the tumbler (would fail if one tumbler or client's chain isn't sync)
                        var tumblerCycle = this.TumblerParameters.CycleGenerator.GetCycle(voucherResponse.CycleStart);
                        Assert(tumblerCycle.Start == cycle.Start, "invalid-phase");
                        //Saving the voucher for later
                        session.StartCycle = cycle.Start;
                        session.ClientChannelNegotiation = new ClientChannelNegotiation(this.TumblerParameters, cycle.Start);
                        session.ClientChannelNegotiation.ReceiveUnsignedVoucher(voucherResponse);
                        logger.LogInformation("Registration Complete");
                    }
                    break;
                // in this phase, Alice creates 2 transactions: an escrow transaction and a redeem transaction (in case things don't go as planned) 
                // on the next phase, she sends the blinded voucher to the tumbler, who doesn't know this voucher is coming from Bob.
                // the tumbler signs the voucher and replies with a solution to the voucher.
                case CyclePhase.ClientChannelEstablishment:
                    if (session.ClientChannelNegotiation.Status == TumblerClientSessionStates.WaitingTumblerClientTransactionKey)
                    {
                        var key = await this.AliceClient.RequestTumblerEscrowKeyAsync(cycle.Start);
                        session.ClientChannelNegotiation.ReceiveTumblerEscrowKey(key.PubKey, key.KeyIndex);
                        //Client create the escrow
                        var escrowTxOut = session.ClientChannelNegotiation.BuildClientEscrowTxOut();
                        feeRate = GetFeeRate();

                        Transaction clientEscrowTx = null;
                        try
                        {
                            clientEscrowTx = services.FundTransaction(escrowTxOut, feeRate);
                        }
                        catch (NotEnoughFundsException ex)
                        {
                            logger.LogInformation($"Not enough funds in the wallet to tumble. Missing about {ex.Missing}. Denomination is {this.TumblerParameters.Denomination}.");
                            break;
                        }

                        session.SolverClientSession = session.ClientChannelNegotiation.SetClientSignedTransaction(clientEscrowTx);                        
                        correlation = GetCorrelation(session.SolverClientSession.EscrowedCoin.ScriptPubKey);

                        // Tracker.AddressCreated(cycle.Start, TransactionType.ClientEscrow, escrowTxOut.ScriptPubKey, correlation);
                        // Tracker.TransactionCreated(cycle.Start, TransactionType.ClientEscrow, clientEscrowTx.GetHash(), correlation);                        
                        this.watchOnlyWalletManager.Watch(escrowTxOut.ScriptPubKey);
                        
                        var redeemDestination = this.OriginWallet.GetAccountsByCoinType(this.coinType).First().GetFirstUnusedReceivingAddress().ScriptPubKey;// Services.WalletService.GenerateAddress().ScriptPubKey;
                        var redeemTx = session.SolverClientSession.CreateRedeemTransaction(feeRate, redeemDestination);

                        //Tracker.AddressCreated(cycle.Start, TransactionType.ClientRedeem, redeemDestination, correlation);
                        //redeemTx does not be to be recorded to the tracker, this is TrustedBroadcastService job

                        services.Broadcast(clientEscrowTx);
                        services.TrustedBroadcast(cycle.Start, TransactionType.ClientRedeem, correlation, redeemTx);

                        logger.LogInformation("Client escrow broadcasted " + clientEscrowTx.GetHash());
                        logger.LogInformation("Client escrow redeem " + redeemTx.Transaction.GetHash() + " will be broadcast later if tumbler unresponsive");
                    }
                    else if (session.ClientChannelNegotiation.Status == TumblerClientSessionStates.WaitingSolvedVoucher)
                    {
                        TransactionInformation clientTx = GetTransactionInformation(session.SolverClientSession.EscrowedCoin, true);
                        var state = session.ClientChannelNegotiation.GetInternalState();
                        if (clientTx != null && clientTx.Confirmations >= cycle.SafetyPeriodDuration)
                        {
                            //Client asks the public key of the Tumbler and sends its own
                            var aliceEscrowInformation = session.ClientChannelNegotiation.GenerateClientTransactionKeys();
                            var voucher = await this.AliceClient.SignVoucherAsync(new Models.SignVoucherRequest
                            {
                                MerkleProof = clientTx.MerkleProof,
                                Transaction = clientTx.Transaction,
                                KeyReference = state.TumblerEscrowKeyReference,
                                ClientEscrowInformation = aliceEscrowInformation,
                                TumblerEscrowPubKey = state.ClientEscrowInformation.OtherEscrowKey
                            });
                            session.ClientChannelNegotiation.CheckVoucherSolution(voucher);
                            logger.LogInformation("Voucher solution obtained");
                        }
                    }
                    break;
                // in this phase, Bob now opens a channel with the tumbler by sending the unblinded voucher he received from Alice.
                // at this point, the tumbler knows that there is an Alice somewhere that wants to tumble with Bob.
                // the tumbler then creates a new transaction that spends the money and sends it to Bob.
                // but the transaction is locked and Bob doesn't know the signature.
                // to prove to Bob that behind the lock there is a signature, the tumbler uses the puzzle promise protocol.
                case CyclePhase.TumblerChannelEstablishment:
                    if (session.ClientChannelNegotiation != null && session.ClientChannelNegotiation.Status == TumblerClientSessionStates.WaitingGenerateTumblerTransactionKey)
                    {
                        //Client asks the Tumbler to make a channel
                        var bobEscrowInformation = session.ClientChannelNegotiation.GetOpenChannelRequest();
                        var tumblerInformation = await this.BobClient.OpenChannelAsync(bobEscrowInformation);
                        session.PromiseClientSession = session.ClientChannelNegotiation.ReceiveTumblerEscrowedCoin(tumblerInformation);
                        //Tell to the block explorer we need to track that address (for checking if it is confirmed in payment phase)
                        this.watchOnlyWalletManager.Watch(session.PromiseClientSession.EscrowedCoin.ScriptPubKey);
                        //Tracker.AddressCreated(cycle.Start, TransactionType.TumblerEscrow, PromiseClientSession.EscrowedCoin.ScriptPubKey, correlation);
                        //Tracker.TransactionCreated(cycle.Start, TransactionType.TumblerEscrow, PromiseClientSession.EscrowedCoin.Outpoint.Hash, correlation);

                        //Channel is done, now need to run the promise protocol to get valid puzzle
                        var cashoutDestination = this.DestinationWallet.GetAccountsByCoinType(CoinType.Bitcoin).First().GetFirstUnusedReceivingAddress().ScriptPubKey;
                        //Tracker.AddressCreated(cycle.Start, TransactionType.TumblerCashout, cashoutDestination, correlation);

                        feeRate = GetFeeRate();
                        var sigReq = session.PromiseClientSession.CreateSignatureRequest(cashoutDestination, feeRate);
                        var commiments = await this.BobClient.SignHashesAsync(cycle.Start, session.PromiseClientSession.Id, sigReq);
                        var revelation = session.PromiseClientSession.Reveal(commiments);
                        var proof = await this.BobClient.CheckRevelationAsync(cycle.Start, session.PromiseClientSession.Id, revelation);
                        var puzzle = session.PromiseClientSession.CheckCommitmentProof(proof);
                        session.SolverClientSession.AcceptPuzzle(puzzle);
                        logger.LogInformation("Tumbler escrow broadcasted " + session.PromiseClientSession.EscrowedCoin.Outpoint.Hash);
                    }
                    break;
                // the tumbler then creates 2 transactions: an escrow transaction and a redeem transaction (in case things don't go as planned with Bob)
                
                case CyclePhase.PaymentPhase:
                    if (session.PromiseClientSession != null)
                    {
                        TransactionInformation tumblerTx = GetTransactionInformation(session.PromiseClientSession.EscrowedCoin, false);
                        //Ensure the tumbler coin is confirmed before paying anything
                        if (tumblerTx == null || tumblerTx.Confirmations < cycle.SafetyPeriodDuration)
                        {
                            if (tumblerTx != null)
                                logger.LogInformation("Tumbler escrow " + tumblerTx.Transaction.GetHash() + " expecting " + cycle.SafetyPeriodDuration + " current is " + tumblerTx.Confirmations);
                            else
                                logger.LogInformation("Tumbler escrow not found");
                            return;
                        }
                        if (session.SolverClientSession.Status == SolverClientStates.WaitingGeneratePuzzles)
                        {
                            logger.LogInformation("Tumbler escrow confirmed " + tumblerTx.Transaction.GetHash());
                            feeRate = GetFeeRate();
                            var puzzles = session.SolverClientSession.GeneratePuzzles();
                            var commmitments = await this.AliceClient.SolvePuzzlesAsync(cycle.Start, session.SolverClientSession.Id, puzzles);
                            var revelation2 = session.SolverClientSession.Reveal(commmitments);
                            var solutionKeys = await this.AliceClient.CheckRevelationAsync(cycle.Start, session.SolverClientSession.Id, revelation2);
                            var blindFactors = session.SolverClientSession.GetBlindFactors(solutionKeys);
                            var offerInformation = await this.AliceClient.CheckBlindFactorsAsync(cycle.Start, session.SolverClientSession.Id, blindFactors);
                            var offerSignature = session.SolverClientSession.SignOffer(offerInformation);

                            var offerRedeemAddress = this.OriginWallet.GetAccountsByCoinType(this.coinType).First().GetFirstUnusedReceivingAddress(); // Services.WalletService.GenerateAddress($"Cycle {cycle.Start} Tumbler Redeem").ScriptPubKey);
                            var offerRedeem = session.SolverClientSession.CreateOfferRedeemTransaction(feeRate, offerRedeemAddress.ScriptPubKey);
                            //May need to find solution in the fulfillment transaction                                                        
                            this.watchOnlyWalletManager.Watch(offerRedeem.PreviousScriptPubKey);
                            //Tracker.AddressCreated(cycle.Start, TransactionType.ClientOfferRedeem, offerRedeemAddress.ScriptPubKey, correlation);
                            services.TrustedBroadcast(cycle.Start, TransactionType.ClientOfferRedeem, correlation, offerRedeem);
                            logger.LogInformation("Offer redeem " + offerRedeem.Transaction.GetHash() + " locked until " + offerRedeem.Transaction.LockTime.Height);
                            try
                            {
                                solutionKeys = await this.AliceClient.FulfillOfferAsync(cycle.Start, session.SolverClientSession.Id, offerSignature);
                                session.SolverClientSession.CheckSolutions(solutionKeys);
                                var tumblingSolution = session.SolverClientSession.GetSolution();
                                var transaction = session.PromiseClientSession.GetSignedTransaction(tumblingSolution);

                                services.TrustedBroadcast(cycle.Start, TransactionType.TumblerCashout, correlation, new TrustedBroadcastRequest()
                                {
                                    BroadcastAt = cycle.GetPeriods().ClientCashout.Start,
                                    Transaction = transaction
                                });
                                if (!NonCooperative)
                                {
                                    var signature = session.SolverClientSession.SignEscape();
                                    await this.AliceClient.GiveEscapeKeyAsync(cycle.Start, session.SolverClientSession.Id, signature);
                                }
                                logger.LogInformation("Solution recovered from cooperative tumbler");
                            }
                            catch (Exception ex)
                            {
                                logger.LogWarning("Uncooperative tumbler detected, keep connection open.");
                                logger.LogWarning(ex.ToString());
                            }
                            logger.LogInformation("Payment completed");
                        }
                    }
                    break;
                // now Bob can unlock the signature of the transaction and broadcast it to take his coin.
                case CyclePhase.ClientCashoutPhase:
                    if (session.SolverClientSession != null)
                    {
                        //If the tumbler is uncooperative, he published solutions on the blockchain
                        if (session.SolverClientSession.Status == SolverClientStates.WaitingPuzzleSolutions)
                        {
                            var transactions = services.GetTransactions(session.SolverClientSession.GetOfferScriptPubKey(), false);
                            if (transactions.Length == 0)
                            {
                                logger.LogInformation("Solution of puzzle not on the blockchain");
                            }
                            else
                            {
                                session.SolverClientSession.CheckSolutions(transactions.Select(t => t.Transaction).ToArray());
                                logger.LogInformation("Solution recovered from blockchain transaction");

                                var tumblingSolution = session.SolverClientSession.GetSolution();
                                var transaction = session.PromiseClientSession.GetSignedTransaction(tumblingSolution);
                                // Tracker.TransactionCreated(cycle.Start, TransactionType.TumblerCashout, transaction.GetHash(), correlation);
                                services.Broadcast(transaction);
                                logger.LogInformation("Client Cashout completed " + transaction.GetHash());
                            }
                        }
                    }
                    break;
            }
        }

        private uint GetCorrelation(Script scriptPubKey)
        {
            return new uint160(scriptPubKey.Hash.ToString()).GetLow32();
        }

        private TransactionInformation GetTransactionInformation(ICoin coin, bool withProof)
        {
            var tx = services.GetTransactions(coin.TxOut.ScriptPubKey, withProof)
                .FirstOrDefault(t => t.Transaction.GetHash() == coin.Outpoint.Hash);
            return tx;
        }

        private FeeRate GetFeeRate()
        {
            return services.GetFeeRate();
        }

        private void Assert(bool test, string error)
        {
            if (!test)
                throw new PuzzleException(error);
        }
    }

    public class TransactionInformation
    {
        public int Confirmations
        {
            get; set;
        }
        public MerkleBlock MerkleProof
        {
            get;
            set;
        }
        public Transaction Transaction
        {
            get; set;
        }
    }
}
