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
        int lastCycleStarted;

        private ClassicTumblerParameters TumblerParameters { get; set; }

        public TumbleBitManager(ILoggerFactory loggerFactory, IWalletManager walletManager, ConcurrentChain chain, Network network, Signals signals)
        {
            this.lastCycleStarted = 0;
            this.walletManager = walletManager;
            this.chain = chain;
            this.signals = signals;
            this.network = network;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);

            // load the persisted tumbling state 
            this.tumblingState = TumblingState.LoadState();
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

            if (this.tumblingState == null)
            {
                this.tumblingState = new TumblingState();
            }

            // update and save the state
            this.tumblingState.TumblerParameters = this.TumblerParameters;
            this.tumblingState.Save();

            return this.TumblerParameters;
        }

        /// <inheritdoc />
        public Task TumbleAsync(string destinationWalletName)
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
                throw new Exception($"Destination not found. Have you created a wallet with name {destinationWalletName}?");
            }

            // update the state and save
            this.tumblingState.DestinationWalletName = destinationWalletName;
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
            // TODO start the state machine 
            this.logger.LogDebug($"Receive block with height {height}");

            // update the block height in the tumbling state
            this.tumblingState.LastBlockReceivedHeight = height;
            this.tumblingState.Save();
            
            // get the next cycle to be started
            var cycle = this.TumblerParameters.CycleGenerator.GetRegistratingCycle(height);

            // check if we need to start a new session starting from the registration cycle
            if (this.lastCycleStarted != cycle.Start)
            {
                this.lastCycleStarted = cycle.Start;
                this.logger.LogDebug($"new registration cycle at {cycle.Start}");

                if (this.tumblingState.Sessions.SingleOrDefault(s => s.StartCycle == cycle.Start) == null)
                {
                    this.tumblingState.CreateNewSession(cycle.Start);
                    this.logger.LogDebug($"new session created at {cycle.Start}");
                }
            }

            // update the state of the tumbling session in this new block
            this.tumblingState.Update();
        }
    }
}
