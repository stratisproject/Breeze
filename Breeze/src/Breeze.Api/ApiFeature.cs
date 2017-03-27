using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Builder.Feature;

namespace Breeze.Api
{
	public class ApiFeature : FullNodeFeature
	{				
		public override void Start()
		{
			Program.Main(null);
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
				});
			});

			return fullNodeBuilder;
		}
	}
}
