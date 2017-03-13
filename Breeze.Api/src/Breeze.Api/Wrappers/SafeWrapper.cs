using System.IO;
using System.Linq;
using HBitcoin.KeyManagement;
using NBitcoin;
using Breeze.Api.Models;

namespace Breeze.Api.Wrappers
{
    /// <summary>
    /// An implementation of the <see cref="ISafeWrapper"/> interface.
    /// </summary>
    public class SafeWrapper : ISafeWrapper
    {
        /// <summary>
        /// Creates a safe on the local device.
        /// </summary>
        /// <param name="password">The user's password.</param>
        /// <param name="folderPath">The folder where the safe will be saved.</param>
        /// <param name="name">The name of the safe.</param>
        /// <param name="network">The network for which to create a safe.</param>
        /// <returns>A mnemonic allowing recovery of the safe.</returns>
        public string Create(string password, string folderPath, string name, string network)
        {
            // any network different than MainNet will default to TestNet
            Network net;
            switch (network.ToLowerInvariant())
            {
                case "main":
                case "mainnet":
                    net = Network.Main;
                    break;
                default:
                    net = Network.TestNet;
                    break;
            }

            Mnemonic mnemonic;
            Safe safe = Safe.Create(out mnemonic, password, Path.Combine(folderPath, $"{name}.json"), net);
            return mnemonic.ToString();
        }

        /// <summary>
        /// Loads a safe from the local device.
        /// </summary>
        /// <param name="password">The user's password.</param>
        /// <param name="folderPath">The folder where the safe will be loaded.</param>
        /// <param name="name">The name of the safe.</param>
        /// <returns>The safe loaded from the local device</returns>
        public SafeModel Load(string password, string folderPath, string name)
        {
            Safe safe = Safe.Load(password, Path.Combine(folderPath, $"{name}.json"));

            //TODO review here which data should be returned
            return new SafeModel
            {
                Network = safe.Network.Name,
                Addresses = safe.GetFirstNAddresses(10).Select(a => a.ToWif()),
                FileName = safe.WalletFilePath
            };
        }
    }
}
