using System;
using Breeze.TumbleBit.Client;
using Breeze.TumbleBit.Controllers;
using Stratis.Bitcoin.Builder.Feature;
using Microsoft.Extensions.DependencyInjection;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Logging;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Breeze.TumbleBit
{
    public class TumbleBitFeature : FullNodeFeature
    {
        public TumbleBitFeature()
        {
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
        }
    }

    public static class TumbleBitFeatureExtension
    {
        public static IFullNodeBuilder UseTumbleBit(this IFullNodeBuilder fullNodeBuilder, Uri serverAddress)
        {
            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                .AddFeature<TumbleBitFeature>()
                .FeatureServices(services =>
                    {
                        services.AddSingleton<ITumbleBitManager, TumbleBitManager> ();
                        services.AddSingleton<TumbleBitController>();
                    });
            });

            return fullNodeBuilder;
        }
    }
}
