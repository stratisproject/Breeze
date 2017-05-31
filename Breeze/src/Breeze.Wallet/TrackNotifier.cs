using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
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
    public class TrackNotifier
    {
        private readonly WalletManager walletManager;
        private readonly ConcurrentChain chain;
        private readonly BlockNotification blockNotification;
        private readonly CoinType coinType;
        private readonly ILogger logger;

        public TrackNotifier(ILoggerFactory loggerFactory, IWalletManager walletManager, 
			ConcurrentChain chain, BlockNotification blockNotification, Network network)
        {
            this.walletManager = walletManager as WalletManager;
            this.chain = chain;
            this.blockNotification = blockNotification;
            this.coinType = (CoinType)network.Consensus.CoinType;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <inheritdoc />
        public Task Initialize()
        {
            // get the chain headers. This needs to be up-to-date before we really do anything
            //await this.WaitForChainDownloadAsync();

            // start syncing blocks
            var bestHeightForSyncing = this.FindBestHeightForSyncing();
            this.SyncFrom(bestHeightForSyncing);
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
    }
}
