using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Breeze.Wallet.Notifications;
using NBitcoin;
using Stratis.Bitcoin;
using Stratis.Bitcoin.Notifications;
using Stratis.Bitcoin.Utilities;

namespace Breeze.Wallet
{
    public class Tracker : ITracker
    {
        private readonly WalletManager walletManager;
        private readonly ConcurrentChain chain;
        private readonly Signals signals;
        private readonly BlockNotification blockNotification;
        private readonly CoinType coinType;

        public Tracker(IWalletManager walletManager, ConcurrentChain chain, Signals signals, BlockNotification blockNotification, Network network)
        {
            this.walletManager = walletManager as WalletManager;
            this.chain = chain;
            this.signals = signals;
            this.blockNotification = blockNotification;
			this.coinType = (CoinType)network.Consensus.CoinType;
        }

        /// <inheritdoc />
        public async Task Initialize()
        {
            // get the chain headers. This needs to be up-to-date before we really do anything
            await this.WaitForChainDownloadAsync();

            // subscribe to receiving blocks and transactions
            BlockSubscriber sub = new BlockSubscriber(this.signals.Blocks, new BlockObserver(this.chain, this.walletManager));
            sub.Subscribe();
            TransactionSubscriber txSub = new TransactionSubscriber(this.signals.Transactions, new TransactionObserver(this.walletManager));
            txSub.Subscribe();

            // start syncing blocks
            this.blockNotification.SyncFrom(this.chain.GetBlock(this.FindBestHeightForSyncing()).HashBlock);
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
                        cancellationTokenSource.Cancel();
                    }

                    return Task.CompletedTask;
                },
                cancellationTokenSource.Token,
                repeatEvery: TimeSpans.FiveSeconds);
        }

        /// <inheritdoc />
        public void SyncFrom(DateTime date)
        {
            int blockSyncStart = this.chain.GetHeightAtTime(date);
            
            // start syncing blocks
            this.SyncFrom(blockSyncStart);
        }

        /// <inheritdoc />
        public void SyncFrom(int height)
        {                       
            this.blockNotification.SyncFrom(this.chain.GetBlock(height).HashBlock);
        }

        private bool BlocksSynced()
        {
            return this.walletManager.Wallets.All(w => w.AccountsRoot.Single(a => a.CoinType == this.coinType).LastBlockSyncedHeight == this.chain.Tip.Height);
        }

    }
}
