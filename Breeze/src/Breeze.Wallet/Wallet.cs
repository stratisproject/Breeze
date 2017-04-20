using System;
using NBitcoin;

namespace Breeze.Wallet
{
    /// <summary>
    /// A wallet
    /// </summary>
    public class Wallet
    {
        /// <summary>
        /// The chain code. 
        /// </summary>
        public byte[] ChainCode { get; set; }

        /// <summary>
        /// The network this wallet is for.
        /// </summary>
        public Network Network { get; set; }

        /// <summary>
        /// The time this wallet was created.
        /// </summary>
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// The location of the wallet file on the local system.
        /// </summary>
        public string WalletFilePath { get; set; }

        /// <summary>
        /// The key used to generate keys.
        /// </summary>
        public ExtKey ExtendedKey { get; set; }
    }
}