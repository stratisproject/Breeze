using Stratis.Bitcoin.Builder.Feature;
using Microsoft.Extensions.DependencyInjection;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Logging;
using Microsoft.Extensions.Logging;
using Serilog;
using Stratis.Bitcoin.Wallet;

namespace Breeze.Wallet
{
    public class LightWalletFeature : FullNodeFeature
    {
	    private readonly TrackNotifier trackNotifier;

	    public LightWalletFeature(TrackNotifier trackNotifier)
	    {
		    this.trackNotifier = trackNotifier;
	    }

        public override void Start()
        {
			this.trackNotifier.Initialize().GetAwaiter().GetResult();
        }

        public override void Stop()
        {
            base.Stop();
        }
    }

    public static class WalletFeatureExtension
    {
        public static IFullNodeBuilder UseLightWallet(this IFullNodeBuilder fullNodeBuilder)
        {
			// use the wallet and on top of that start to notifier
	        fullNodeBuilder.UseWallet();

            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                .AddFeature<LightWalletFeature>()
                .FeatureServices(services =>
                    {
                        services.AddSingleton<TrackNotifier>();
                    });
            });

            return fullNodeBuilder;
        }
    }
}
