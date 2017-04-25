using System;
using Breeze.Wallet.Models;
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
        /// <param name="folderPath">The folder where the wallet will be saved.</param>
        /// <param name="name">The name of the wallet.</param>
        /// <param name="network">The network this wallet is for.</param>
        /// <param name="passphrase">The passphrase used in the seed.</param>
        /// <returns>A mnemonic defining the wallet's seed used to generate addresses.</returns>
        Mnemonic CreateWallet(string password, string folderPath, string name, string network, string passphrase = null);

        /// <summary>
        /// Loads a wallet from a file.
        /// </summary>
        /// <param name="password">The user's password.</param>
        /// <param name="folderPath">The folder where the wallet will be loaded.</param>
        /// <param name="name">The name of the wallet.</param>
        /// <returns>The wallet.</returns>
        Wallet LoadWallet(string password, string folderPath, string name);

        /// <summary>
        /// Recovers a wallet.
        /// </summary>
        /// <param name="password">The user's password.</param>
        /// <param name="folderPath">The folder where the wallet will be loaded.</param>
        /// <param name="name">The name of the wallet.</param>
        /// <param name="network">The network in which to creae this wallet</param>
        /// <param name="mnemonic">The user's mnemonic for the wallet.</param>		
        /// <param name="passphrase">The passphrase used in the seed.</param>
        /// <param name="creationTime">The time this wallet was created.</param>
        /// <returns>The recovered wallet.</returns>
        Wallet RecoverWallet(string password, string folderPath, string name, string network, string mnemonic, string passphrase = null, DateTimeOffset? creationTime = null);

        /// <summary>
        /// Deleted a wallet.
        /// </summary>
        /// <param name="walletFilePath">The location of the wallet file.</param>        
        void DeleteWallet(string walletFilePath);

        /// <summary>
        /// Creates a new account.
        /// </summary>
        /// <param name="walletName">The name of the wallet in which this account will be created.</param>
        /// <param name="coinType">the type of coin for which to create an account.</param>
        /// <param name="accountName">The name by which this account will be identified.</param>
        /// <param name="password">The password used to decrypt the private key.</param>
        /// <remarks>
        /// According to BIP44, an account at index (i) can only be created when the account
        /// at index (i - 1) contains transactions.
        /// </remarks>
        /// <returns>The name of the new account.</returns>
        string CreateNewAccount(string walletName, CoinType coinType, string accountName, string password);

        /// <summary>
        /// Creates the new address.
        /// </summary>
        /// <param name="walletName">The name of the wallet in which this address will be created.</param>
        /// <param name="coinType">the type of coin for which to create an account.</param>
        /// <param name="accountName">The name of the account in which this address will be created.</param>        
        /// <returns>The new address, in Base58 format.</returns>
        string CreateNewAddress(string walletName, CoinType coinType, string accountName);
        
        WalletGeneralInfoModel GetGeneralInfo(string walletName);

        WalletBalanceModel GetBalance(string walletName);

        WalletHistoryModel GetHistory(string walletName);

        WalletBuildTransactionModel BuildTransaction(string password, string address, Money amount, string feeType, bool allowUnconfirmed);

        bool SendTransaction(string transactionHex);
    }
}
