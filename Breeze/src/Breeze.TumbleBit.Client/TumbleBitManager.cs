using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NTumbleBit.ClassicTumbler;
using Stratis.Bitcoin;
using Stratis.Bitcoin.Logging;
using Stratis.Bitcoin.Wallet;

namespace Breeze.TumbleBit.Client
{
    /// <summary>
    /// An implementation of a tumbler manager.
    /// </summary>
    /// <seealso cref="Breeze.TumbleBit.Client.ITumbleBitManager" />
    public class TumbleBitManager : ITumbleBitManager
    {
        private ITumblerService tumblerService;

        private IWalletManager walletManager;
        private readonly ILogger logger;
        private readonly Signals signals;
        private readonly ConcurrentChain chain;

        private ClassicTumblerParameters TumblerParameters { get; set; }

        public TumbleBitManager(ILoggerFactory loggerFactory, IWalletManager walletManager, ConcurrentChain chain, Network network, Signals signals)
        {            
            this.walletManager = walletManager;
            this.chain = chain;
            this.signals = signals;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }
        
        /// <inheritdoc />
        public async Task<ClassicTumblerParameters> ConnectToTumblerAsync(Uri serverAddress)
        {
            this.tumblerService = new TumblerService(serverAddress);
            this.TumblerParameters = await this.tumblerService.GetClassicTumblerParametersAsync();
            return this.TumblerParameters;
        }

        /// <inheritdoc />
        public Task TumbleAsync(string destinationWalletName)
        {
            if (this.TumblerParameters == null || this.tumblerService == null)
            {
                throw new Exception("Please connect to the tumbler first.");
            }

            Wallet destinationWallet = this.walletManager.GetWallet(destinationWalletName);
            if (destinationWallet == null)
            {
                throw new Exception($"Destination not found. Have you created a wallet with name {destinationWalletName}?");
            }
            
            // subscribe to receiving blocks and transactions
            this.signals.Blocks.Subscribe(new BlockObserver(this.chain, this));
            
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void ProcessBlock(int height, Block block)
        {
            // TODO start the state machine 
            this.logger.LogDebug($"Receive block with height {height}");
        }
    }
}
