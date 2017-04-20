using System;
using NBitcoin;

namespace Breeze.Wallet
{
    /// <summary>
    /// Interface for a manager providing operations on wallets.
    /// </summary>
    public interface IWalletManager : IDisposable
    {
        /// <summary>
        /// Creates a wallet and persist it as a file on the local system.
        /// </summary>
        /// <param name="password">The password used to encrypt sensitive info.</param>
        /// <param name="passphrase">The passphrase used in the seed.</param>
        /// <param name="walletFilePath">The path where the wallet file will be created.</param>
        /// <param name="network">The network this wallet is for.</param>
        /// <returns>A mnemonic defining the wallet's seed used to generate addresses.</returns>
        Mnemonic CreateWallet(string password, string walletFilePath, Network network, string passphrase = null);

        /// <summary>
        /// Loads a wallet from a file.
        /// </summary>
        /// <param name="password">The password used to encrypt sensitive info.</param>
        /// <param name="walletFilePath">The location of the wallet file.</param>
        /// <returns>The wallet.</returns>
        Wallet LoadWallet(string password, string walletFilePath);

        /// <summary>
        /// Recovers a wallet.
        /// </summary>
        /// <param name="mnemonic">A mnemonic defining the wallet's seed used to generate addresses.</param>
        /// <param name="password">The password used to encrypt sensitive info.</param>
        /// <param name="walletFilePath">The location of the wallet file.</param>
        /// <param name="network">The network this wallet is for.</param>
        /// <param name="passphrase">The passphrase used in the seed.</param>
        /// <param name="creationTime">The time this wallet was created.</param>
        /// <returns>The recovered wallet.</returns>
        Wallet RecoverWallet(Mnemonic mnemonic, string password, string walletFilePath, Network network, string passphrase = null, DateTimeOffset? creationTime = null);

        /// <summary>
        /// Deleted a wallet.
        /// </summary>
        /// <param name="walletFilePath">The location of the wallet file.</param>        
        void DeleteWallet(string walletFilePath);
    }
}
