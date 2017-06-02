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

namespace Breeze.Wallet
{
    public class LightWalletSyncManager : WalletSyncManager
    {
        private readonly WalletManager walletManager;
        private readonly ConcurrentChain chain;
        private readonly BlockNotification blockNotification;
        private readonly CoinType coinType;
        private readonly ILogger logger;

        public LightWalletSyncManager(ILoggerFactory loggerFactory, IWalletManager walletManager, ConcurrentChain chain, Network network, 
			BlockNotification blockNotification):base(loggerFactory, walletManager, chain, network)
        {
            this.blockNotification = blockNotification;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            // get the chain headers. This needs to be up-to-date before we really do anything
            this.WaitForChainDownloadAsync().GetAwaiter().GetResult();

            // start syncing blocks
            var bestHeightForSyncing = this.FindBestHeightForSyncing();
            this.SyncFrom(bestHeightForSyncing);
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
        public override void SyncFrom(DateTime date)
        {
            int blockSyncStart = this.chain.GetHeightAtTime(date);

            // start syncing blocks
            this.SyncFrom(blockSyncStart);
        }

        /// <inheritdoc />
        public override void SyncFrom(int height)
        {
            this.blockNotification.SyncFrom(this.chain.GetBlock(height).HashBlock);
        }
    }
}
