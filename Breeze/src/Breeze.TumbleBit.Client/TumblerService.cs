using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using NBitcoin.JsonConverters;
using Newtonsoft.Json;
using NTumbleBit.ClassicTumbler;
using NTumbleBit.JsonConverters;

namespace Breeze.TumbleBit.Client
{
    public class TumblerService : ITumblerService
    {
        private readonly string serverAddress;

        public TumblerService(Uri serverAddress)
        {
            this.serverAddress = serverAddress.ToString();
            FlurlHttp.Configure(c => {
                c.JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new NetworkJsonConverter(), new RsaKeyJsonConverter(), new UInt256JsonConverter() }

                });
            });
        }

        /// <inheritdoc />
        public async Task<ClassicTumblerParameters> GetClassicTumblerParametersAsync()
        {
            ClassicTumblerParameters result = await this.serverAddress.AppendPathSegment("/api/v1/tumblers/0/parameters").GetJsonAsync<ClassicTumblerParameters>();
            return result;
        }
    }
}
