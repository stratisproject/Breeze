using Microsoft.Extensions.DependencyInjection;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Builder.Feature;

namespace Breeze.Api
{
	public class ApiFeature : FullNodeFeature
	{
		private readonly IFullNodeBuilder fullNodeBuilder;

		public ApiFeature(IFullNodeBuilder fullNodeBuilder)
		{
			this.fullNodeBuilder = fullNodeBuilder;
		}

		public override void Start()
		{
			Program.Initialize(this.fullNodeBuilder.Services);
		}
	}

	public static class ApiFeatureExtension
	{
		public static IFullNodeBuilder UseApi(this IFullNodeBuilder fullNodeBuilder)
		{
			fullNodeBuilder.ConfigureFeature(features =>
			{
				features
				.AddFeature<ApiFeature>()
				.FeatureServices(services =>
					{
						services.AddSingleton(fullNodeBuilder);
					});
			});

			return fullNodeBuilder;
		}
	}	
}
