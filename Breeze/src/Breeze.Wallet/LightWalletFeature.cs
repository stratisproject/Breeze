using Stratis.Bitcoin.Builder.Feature;
using Microsoft.Extensions.DependencyInjection;
using NBitcoin;
using NBitcoin.Protocol;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Connection;
using Stratis.Bitcoin.Consensus;
using Stratis.Bitcoin.Consensus.Deployments;
using Stratis.Bitcoin.Wallet;
using Stratis.Bitcoin.Wallet.Controllers;

namespace Breeze.Wallet
{
    public class LightWalletFeature : FullNodeFeature
    {
        private readonly IWalletSyncManager walletSyncManager;
        private readonly IWalletManager walletManager;
        private readonly IConnectionManager connectionManager;
        private readonly ConcurrentChain chain;
        private readonly NodeDeployments nodeDeployments;

        public LightWalletFeature(IWalletSyncManager walletSyncManager, IWalletManager walletManager, IConnectionManager connectionManager, 
            ConcurrentChain chain, NodeDeployments nodeDeployments)
        {
            this.walletSyncManager = walletSyncManager;
            this.walletManager = walletManager;
            this.connectionManager = connectionManager;
            this.chain = chain;
            this.nodeDeployments = nodeDeployments;
        }

        public override void Start()
        {
            this.connectionManager.Parameters.TemplateBehaviors.Add(new DropNodesBehaviour(this.chain, this.connectionManager));

            this.walletManager.Initialize();
            this.walletSyncManager.Initialize();

            var flags = this.nodeDeployments.GetFlags(this.walletSyncManager.WalletTip);
            if (flags.ScriptFlags.HasFlag(ScriptVerify.Witness))
                this.connectionManager.AddDiscoveredNodesRequirement(NodeServices.NODE_WITNESS);
        }

        public override void Stop()
        {
            base.Stop();
        }
    }

    public static class LightWalletFeatureExtension
    {
        public static IFullNodeBuilder UseLightWallet(this IFullNodeBuilder fullNodeBuilder)
        {
            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<LightWalletFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<IWalletSyncManager, LightWalletSyncManager>();
                        services.AddSingleton<IWalletManager, WalletManager>();
                        services.AddSingleton<IWalletFeePolicy, LightWalletFeePolicy>();
                        services.AddSingleton<WalletController>();

                    });
            });

            return fullNodeBuilder;
        }
    }
}
