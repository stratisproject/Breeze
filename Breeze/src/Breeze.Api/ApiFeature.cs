using System;
using System.Threading.Tasks;
using Breeze.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stratis.Bitcoin;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Builder.Feature;
using Stratis.Bitcoin.Logging;
using Stratis.Bitcoin.Utilities;

namespace Breeze.Api
{
	/// <summary>
	/// Provides an Api to the full node
	/// </summary>
	public class ApiFeature : FullNodeFeature
	{		
		private readonly IFullNodeBuilder fullNodeBuilder;
		private readonly FullNode fullNode;
	    private readonly ApiFeatureOptions apiFeatureOptions;
	    private readonly IAsyncLoopFactory asyncLoopFactory;

	    public ApiFeature(IFullNodeBuilder fullNodeBuilder, FullNode fullNode, ApiFeatureOptions apiFeatureOptions, IAsyncLoopFactory asyncLoopFactory)
		{
			this.fullNodeBuilder = fullNodeBuilder;
			this.fullNode = fullNode;
		    this.apiFeatureOptions = apiFeatureOptions;
		    this.asyncLoopFactory = asyncLoopFactory;
		}

		public override void Start()
		{
		    Logs.FullNode.LogInformation($"Api starting on url {this.fullNode.Settings.ApiUri}");
            Program.Initialize(this.fullNodeBuilder.Services, this.fullNode);

		    this.TryStartHeartbeat();
		}

        /// <summary>
        /// A heartbeat monitor that when enabled will shutdown 
        /// the node if no external beat was made during the trashold
        /// </summary>
        public void TryStartHeartbeat()
	    {
	        if (this.apiFeatureOptions.HeartbeatMonitor?.HeartbeatInterval.TotalSeconds > 0)
	        {
	            this.asyncLoopFactory.Run("ApiFeature.MonitorHeartbeat", token =>
	                {
	                    // shortened for redability
	                    var monitor = this.apiFeatureOptions.HeartbeatMonitor;

	                    // check the trashold to trigger a shutdown
	                    if (monitor.LastBeat.Add(monitor.HeartbeatInterval) < DateTime.UtcNow)
	                        this.fullNode.Stop();

	                    return Task.CompletedTask;
	                },
	                this.fullNode.GlobalCancellation.Cancellation.Token,
	                repeatEvery: this.apiFeatureOptions.HeartbeatMonitor?.HeartbeatInterval,
	                startAfter: TimeSpans.Minute);
	        }
	    }
	}

    public class ApiFeatureOptions
    {
        public HeartbeatMonitor HeartbeatMonitor { get; set; }

        public void Heartbeat(TimeSpan timeSpan)
        {
            this.HeartbeatMonitor = new HeartbeatMonitor {HeartbeatInterval = timeSpan};
        }
    }

    public static class ApiFeatureExtension
	{
		public static IFullNodeBuilder UseApi(this IFullNodeBuilder fullNodeBuilder, Action<ApiFeatureOptions> optionsAction = null)
		{
            // TODO: move the options in to the feature builder
		    var options = new ApiFeatureOptions();
            optionsAction?.Invoke(options);

            fullNodeBuilder.ConfigureFeature(features =>
			{
				features
				.AddFeature<ApiFeature>()
				.FeatureServices(services =>
					{
						services.AddSingleton(fullNodeBuilder);
					    services.AddSingleton(options);
					});
			});

			return fullNodeBuilder;
		}
	}	
}
