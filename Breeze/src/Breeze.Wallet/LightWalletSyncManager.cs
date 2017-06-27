using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin;
using Stratis.Bitcoin.Notifications;
using Stratis.Bitcoin.Utilities;
using Stratis.Bitcoin.Wallet;
using Stratis.Bitcoin.Wallet.Notifications;

namespace Breeze.Wallet
{
    public class LightWalletSyncManager : IWalletSyncManager
    {
        private readonly WalletManager walletManager;
        private readonly ConcurrentChain chain;
        private readonly BlockNotification blockNotification;
        private readonly CoinType coinType;
        private readonly ILogger logger;
        private readonly Signals signals;

        private ChainedBlock walletTip;

        public LightWalletSyncManager(ILoggerFactory loggerFactory, IWalletManager walletManager, ConcurrentChain chain, Network network,
            BlockNotification blockNotification, Signals signals)
        {
            this.walletManager = walletManager as WalletManager;
            this.chain = chain;
            this.signals = signals;
            this.blockNotification = blockNotification;
            this.coinType = (CoinType)network.Consensus.CoinType;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <inheritdoc />
        public Task Initialize()
        {            
            // if there is no wallet created yet, the wallet tip is the chain tip.
            if (!this.walletManager.Wallets.Any())
            {
                this.walletTip = this.chain.Tip;
            }
            else
            {
                this.walletTip = this.chain.GetBlock(this.walletManager.WalletTipHash);
                if (this.walletTip == null)
                {
                    // the wallet tip was not found in the main chain.
                    // this can happen if the node crashes unexpectedly.
                    // to recover we need to find the first common fork 
                    // with the best chain, as the wallet does not have a  
                    // list of chain headers we use a BlockLocator and persist 
                    // that in the wallet. the block locator will help finding 
                    // a common fork and bringing the wallet back to a good 
                    // state (behind the best chain)
                    var locators = this.walletManager.Wallets.First().BlockLocator;
                    BlockLocator blockLocator = new BlockLocator { Blocks = locators.ToList() };
                    var fork = this.chain.FindFork(blockLocator);
                    this.walletManager.RemoveBlocks(fork);
                    this.walletManager.WalletTipHash = fork.HashBlock;
                    this.walletTip = fork;
                }
            }

            // subscribe to receiving blocks and transactions
            BlockSubscriber sub = new BlockSubscriber(this.signals.Blocks, new BlockObserver(this));
            sub.Subscribe();
            TransactionSubscriber txSub = new TransactionSubscriber(this.signals.Transactions, new TransactionObserver(this));
            txSub.Subscribe();

            // start syncing blocks
            var bestHeightForSyncing = this.FindBestHeightForSyncing();
            this.blockNotification.SyncFrom(this.chain.GetBlock(bestHeightForSyncing).HashBlock);
            this.logger.LogInformation($"Tracker initialized. Syncing from {bestHeightForSyncing}.");

            return Task.CompletedTask;
        }

        private int FindBestHeightForSyncing()
        {
            // if there are no wallets, get blocks from now
            if (!this.walletManager.Wallets.Any())
            {
                return this.chain.Tip.Height;
            }

            // sync the accounts with new blocks, starting from the most out of date
            int? syncFromHeight = this.walletManager.Wallets.Min(w => w.AccountsRoot.Single(a => a.CoinType == this.coinType).LastBlockSyncedHeight);
            if (syncFromHeight == null)
            {
                return this.chain.Tip.Height;
            }

            return Math.Min(syncFromHeight.Value, this.chain.Tip.Height);
        }

        /// <inheritdoc />
        public Task WaitForChainDownloadAsync()
        {
            // make sure the chain is downloaded
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            return AsyncLoop.Run("WalletFeature.DownloadChain", token =>
                {
                    // wait until the chain is downloaded. We wait until a block is from an hour ago.
                    if (this.chain.IsDownloaded())
                    {
                        this.logger.LogInformation($"Chain downloaded. Tip height is {this.chain.Tip.Height}.");
                        cancellationTokenSource.Cancel();
                    }

                    return Task.CompletedTask;
                },
                cancellationTokenSource.Token,
                repeatEvery: TimeSpans.FiveSeconds);
        }

        /// <inheritdoc />
        public void ProcessBlock(Block block)
        {
            // if the new block previous hash is the same as the 
            // wallet hash then just pass the block to the manager 
            if (block.Header.HashPrevBlock != this.walletTip.HashBlock)
            {
                // if previous block does not match there might have 
                // been a reorg, check if the wallet is still on the main chain
                ChainedBlock inBestChain = this.chain.GetBlock(this.walletTip.HashBlock);
                if (inBestChain == null)
                {
                    // the current wallet hash was not found on the main chain
                    // a reorg happenend so bring the wallet back top the last known fork

                    var fork = this.walletTip;

                    // we walk back the chained block object to find the fork
                    while (this.chain.GetBlock(fork.HashBlock) == null)
                        fork = fork.Previous;

                    Guard.Assert(fork.HashBlock == block.Header.HashPrevBlock);
                    this.walletManager.RemoveBlocks(fork);
                }
                else
                {
                    ChainedBlock incomingBlock = this.chain.GetBlock(block.GetHash());
                    if (incomingBlock.Height > this.walletTip.Height)
                    {
                        // the wallet is falling behind we need to catch up
                        this.logger.LogTrace($"block received with height: {inBestChain.Height} and hash: {block.Header.GetHash()} is too far in advance. put the pull back.");
                        this.blockNotification.SyncFrom(this.walletTip.HashBlock);
                        return;
                    }
                }
            }

            this.walletTip = this.chain.GetBlock(block.GetHash());
            this.walletManager.ProcessBlock(block, this.walletTip);
        }

        public void ProcessTransaction(Transaction transaction)
        {
            this.walletManager.ProcessTransaction(transaction);
        }

        public void SyncFrom(DateTime date)
        {
            int blockSyncStart = this.chain.GetHeightAtTime(date);
            this.SyncFrom(blockSyncStart);
        }

        public void SyncFrom(int height)
        {
            var chainedBlock = this.chain.GetBlock(height);
            if (chainedBlock == null)
                throw new WalletException("Invalid block height");
            this.walletTip = chainedBlock;
            this.walletManager.WalletTipHash = chainedBlock.HashBlock;
            this.blockNotification.SyncFrom(chainedBlock.HashBlock);
        }
    }
}
