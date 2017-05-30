using System;
using System.Threading.Tasks;
using NTumbleBit.ClassicTumbler;

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
            this.tumblerService = new TumblerService(serverAddress);
        }

        /// <inheritdoc />
        public async Task<ClassicTumblerParameters> ConnectToTumblerAsync()
        {
            return await this.tumblerService.GetClassicTumblerParametersAsync();
        }
    }
}
