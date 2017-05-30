using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NTumbleBit.ClassicTumbler;
using Refit;
using NBitcoin.JsonConverters;
using NTumbleBit.JsonConverters;

namespace Breeze.TumbleBit.Client
{
    /// <summary>
    /// An implementation of a tumbler manager.
    /// </summary>
    /// <seealso cref="Breeze.TumbleBit.Client.ITumbleBitManager" />
    public class TumbleBitManager : ITumbleBitManager
    {
        private ITumblerService tumblerService;

        public TumbleBitManager(Uri serverAddress)
        {
            this.InitializeTumblerService(serverAddress);
        }

        /// <summary>
        /// Initializes the tumbler service.
        /// </summary>
        /// <param name="serverAddress">The server address.</param>
        public void InitializeTumblerService(Uri serverAddress)
        {
            this.tumblerService = RestService.For<ITumblerService>(serverAddress.ToString(),
                new RefitSettings
                {
                    JsonSerializerSettings = new JsonSerializerSettings
                    {
                        Converters = new List<JsonConverter> { new NetworkJsonConverter(), new RsaKeyJsonConverter(), new UInt256JsonConverter() }

                    }
                });
        }

        /// <inheritdoc />
        public async Task<ClassicTumblerParameters> ConnectToTumblerAsync()
        {
            return await this.tumblerService.GetClassicTumblerParametersAsync();
        }
    }
}
