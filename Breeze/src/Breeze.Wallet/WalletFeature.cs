using Stratis.Bitcoin.Builder.Feature;
using Breeze.Wallet.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Logging;
using Microsoft.Extensions.Logging;

namespace Breeze.Wallet
{
    public class WalletFeature : FullNodeFeature
    {
        private readonly ITracker tracker;
        private readonly IWalletManager walletManager;
        
        public WalletFeature(ITracker tracker, IWalletManager walletManager)
        {
            this.tracker = tracker;
            this.walletManager = walletManager;
        }

        public override void Start()
        {            
            this.tracker.Initialize();
        }

        public override void Stop()
        {
            this.walletManager.Dispose();
            base.Stop();
        }
    }

    public static class WalletFeatureExtension
    {
        public static IFullNodeBuilder UseWallet(this IFullNodeBuilder fullNodeBuilder)
        {
            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                .AddFeature<WalletFeature>()
                .FeatureServices(services =>
                {
                    services.AddSingleton<ITracker, Tracker>();
                    services.AddSingleton<ILoggerFactory>(Logs.LoggerFactory);
                    services.AddSingleton<IWalletManager, WalletManager>();
                    services.AddSingleton<WalletController>();
                });
            });

            return fullNodeBuilder;
        }
    }
}
