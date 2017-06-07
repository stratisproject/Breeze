using System;
using System.Collections.Generic;
using Breeze.TumbleBit.Client;
using Breeze.TumbleBit.Controllers;
using Stratis.Bitcoin.Builder.Feature;
using Microsoft.Extensions.DependencyInjection;
using Stratis.Bitcoin.Builder;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Stratis.Bitcoin.Wallet.JsonConverters;

namespace Breeze.TumbleBit
{
    public class TumbleBitFeature : FullNodeFeature
    {       
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
                        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                        {
                            Formatting = Formatting.Indented,                            
                            ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            Converters = new List<JsonConverter> { new NetworkConverter() }
                        };

                        services.AddSingleton<ITumbleBitManager, TumbleBitManager> ();
                        services.AddSingleton<TumbleBitController>();
                    });
            });

            return fullNodeBuilder;
        }
    }
}
