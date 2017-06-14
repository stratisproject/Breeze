using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using NTumbleBit;
using Stratis.Bitcoin.MemoryPool;

namespace Breeze.TumbleBit.Client
{
    public class ExternalServices
    {
        public Transaction FundTransaction(TxOut txOut, FeeRate feeRate)
        {
            return null;
        }

        public void Track(Script scriptPubkey)
        {
        }

        public bool Broadcast(Transaction tx)
        {
            return true;
        }

        public void TrustedBroadcast(int cycleStart, TransactionType transactionType, uint correlation, TrustedBroadcastRequest broadcast)
        {
        }

        public TransactionInformation[] GetTransactions(Script scriptPubKey, bool withProof)
        {
            return null;
        }

        public FeeRate GetFeeRate()
        {
            decimal relayFee = MempoolValidator.MinRelayTxFee.FeePerK.ToUnit(MoneyUnit.BTC);
            var minimumRate = new FeeRate(Money.Coins(relayFee * 2), 1000); //0.00002000 BTC/kB
            var fallbackFeeRate = new FeeRate(Money.Satoshis(50), 1);       //0.00050000 BTC/kB

            // TODO add real fee estimation 
            //var rate = _RPCClient.TryEstimateFeeRate(1) ??
            //           _RPCClient.TryEstimateFeeRate(2) ??
            //           _RPCClient.TryEstimateFeeRate(3) ??
            //           FallBackFeeRate;

            //if (rate < MinimumFeeRate)
            //    rate = MinimumFeeRa
            
            return fallbackFeeRate;
        }
    }
}
