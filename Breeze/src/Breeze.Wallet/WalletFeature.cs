using Stratis.Bitcoin.Builder.Feature;
using Breeze.Wallet.Controllers;
using Breeze.Wallet.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Stratis.Bitcoin.Builder;

namespace Breeze.Wallet
{
	public class WalletFeature : FullNodeFeature
	{
		public override void Start()
		{
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
					services.AddTransient<IWalletWrapper, WalletWrapper>();
					services.AddTransient<ITrackerWrapper, TrackerWrapper>();
					services.AddSingleton<WalletController>();
				});
			});

			return fullNodeBuilder;
		}
	}
}
