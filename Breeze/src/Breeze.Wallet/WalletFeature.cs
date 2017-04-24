using Stratis.Bitcoin.Builder.Feature;
using Breeze.Wallet.Controllers;
using Breeze.Wallet.Notifications;
using Breeze.Wallet.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using NBitcoin;
using Stratis.Bitcoin;
using Stratis.Bitcoin.Builder;

namespace Breeze.Wallet
{
	public class WalletFeature : FullNodeFeature
	{
		private readonly ITrackerWrapper trackerWrapper;
		private readonly Signals signals;
		private readonly ConcurrentChain chain;

		public WalletFeature(ITrackerWrapper trackerWrapper, Signals signals, ConcurrentChain chain)
		{
			this.trackerWrapper = trackerWrapper;
			this.signals = signals;
			this.chain = chain;
		}

		public override void Start()
		{
			BlockSubscriber sub = new BlockSubscriber(signals.Blocks, new BlockObserver(chain, trackerWrapper));
			sub.Subscribe();
            TransactionSubscriber txSub = new TransactionSubscriber(signals.Transactions, new TransactionObserver(trackerWrapper));
		    txSub.Subscribe();
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
					services.AddSingleton<ITrackerWrapper, TrackerWrapper>();
				    services.AddSingleton<IWalletManager, WalletManager>();
		            services.AddSingleton<WalletController>();
				});
			});

			return fullNodeBuilder;
		}
	}
}
