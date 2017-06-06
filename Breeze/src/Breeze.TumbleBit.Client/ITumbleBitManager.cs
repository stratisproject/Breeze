using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using NTumbleBit.ClassicTumbler;

namespace Breeze.TumbleBit.Client
{
    /// <summary>
    /// An interface for managing interactions with the TumbleBit service.
    /// </summary>
    public interface ITumbleBitManager
    {
        /// <summary>
        /// Connects to the tumbler.
        /// </summary>
        /// <param name="serverAddress">The URI of the tumbler.</param>
        /// <returns></returns>
        Task<ClassicTumblerParameters> ConnectToTumblerAsync(Uri serverAddress);

        Task TumbleAsync(string destinationWalletName);

        /// <summary>
        /// Processes a block received from the network.
        /// </summary>
        /// <param name="height">The height of the block in the blockchain.</param>
        /// <param name="block">The block.</param>
        void ProcessBlock(int height, Block block);
    }
}
