using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NTumbleBit.ClassicTumbler;
using Stratis.Bitcoin;
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
        private readonly IWalletManager walletManager;
        private readonly ILogger logger;
        private readonly Signals signals;
        private readonly ConcurrentChain chain;
        private readonly Network network;
        private TumblingState tumblingState;
        private IDisposable blockReceiver;
     
        private ClassicTumblerParameters TumblerParameters { get; set; }

        public TumbleBitManager(ILoggerFactory loggerFactory, IWalletManager walletManager, ConcurrentChain chain, Network network, Signals signals)
        {
            this.walletManager = walletManager;
            this.chain = chain;
            this.signals = signals;
            this.network = network;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);

            this.tumblingState = new TumblingState(loggerFactory, this.chain, this.walletManager, this.network);
        }

        /// <inheritdoc />
        public async Task<ClassicTumblerParameters> ConnectToTumblerAsync(Uri serverAddress)
        {
            this.tumblerService = new TumblerService(serverAddress);
            this.TumblerParameters = await this.tumblerService.GetClassicTumblerParametersAsync();

            if (this.TumblerParameters.Network != this.network)
            {
                throw new Exception($"The tumbler is on network {this.TumblerParameters.Network} while the wallet is on network {this.network}.");
            }
            
            // load the current tumbling state fromt he file system
            this.tumblingState.LoadStateFromMemory();
            
            // update and save the state
            this.tumblingState.TumblerUri = serverAddress;
            this.tumblingState.TumblerParameters = this.TumblerParameters;
            this.tumblingState.SetClients(this.tumblerService);
            this.tumblingState.Save();

            return this.TumblerParameters;
        }

        /// <inheritdoc />
        public Task TumbleAsync(string originWalletName, string destinationWalletName)
        {
            // make sure the tumbler service is initialized
            if (this.TumblerParameters == null || this.tumblerService == null)
            {
                throw new Exception("Please connect to the tumbler first.");
            }

            // make sure that the user is not trying to resume the process with a different wallet
            if (!string.IsNullOrEmpty(this.tumblingState.DestinationWalletName) && this.tumblingState.DestinationWalletName != destinationWalletName)
            {
                throw new Exception("Please use the same destination wallet until the end of this tumbling session.");
            }

            Wallet destinationWallet = this.walletManager.GetWallet(destinationWalletName);
            if (destinationWallet == null)
            {
                throw new Exception($"Destination wallet not found. Have you created a wallet with name {destinationWalletName}?");
            }

            Wallet originWallet = this.walletManager.GetWallet(originWalletName);
            if (originWallet == null)
            {
                throw new Exception($"Origin wallet not found. Have you created a wallet with name {originWalletName}?");
            }

            // update the state and save
            this.tumblingState.DestinationWallet = destinationWallet;
            this.tumblingState.DestinationWalletName = destinationWalletName;
            this.tumblingState.OriginWallet = originWallet;
            this.tumblingState.OriginWalletName = originWalletName;

            this.tumblingState.Save();

            // subscribe to receiving blocks
            this.blockReceiver = this.signals.Blocks.Subscribe(new BlockObserver(this.chain, this));

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void PauseTumbling()
        {
            this.logger.LogDebug($"Stopping the tumbling. Current height is {this.chain.Tip.Height}.");
            this.blockReceiver.Dispose();
            this.tumblingState.Save();
        }

        /// <inheritdoc />
        public void FinishTumbling()
        {
            this.logger.LogDebug($"The tumbling process is wrapping up. Current height is {this.chain.Tip.Height}.");
            this.blockReceiver.Dispose();
            this.tumblingState.Delete();
            this.tumblingState = null;
        }

        /// <inheritdoc />
        public void ProcessBlock(int height, Block block)
        {            
            this.logger.LogDebug($"Received block with height {height} during tumbling session.");

            // update the block height in the tumbling state
            this.tumblingState.LastBlockReceivedHeight = height;
            this.tumblingState.Save();
            
            // update the state of the tumbling session in this new block
            this.tumblingState.Update();
        }
    }
}
