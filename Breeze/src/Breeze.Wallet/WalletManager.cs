using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Breeze.Wallet.Helpers;
using Breeze.Wallet.Models;
using NBitcoin;
using Newtonsoft.Json;

namespace Breeze.Wallet
{
    /// <summary>
    /// A manager providing operations on wallets.
    /// </summary>
    public class WalletManager : IWalletManager
    {
        public List<Wallet> Wallets { get; }

        public WalletManager()
        {
            this.Wallets = new List<Wallet>();
        }

        /// <inheritdoc />
        public Mnemonic CreateWallet(string password, string folderPath, string name, string network, string passphrase = null, CoinType coinType = CoinType.Bitcoin)
        {
            // for now the passphrase is set to be the password by default.
            if (passphrase == null)
            {
                passphrase = password;
            }

            // generate the root seed used to generate keys from a mnemonic picked at random 
            // and a passphrase optionally provided by the user
            Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            ExtKey extendedKey = mnemonic.DeriveExtKey(passphrase);

            // create a wallet file 
            Wallet wallet = this.GenerateWalletFile(password, folderPath, name, WalletHelpers.GetNetwork(network), extendedKey, coinType);

            this.Load(wallet);
            return mnemonic;
        }

        /// <inheritdoc />
        public Wallet LoadWallet(string password, string folderPath, string name)
        {
            string walletFilePath = Path.Combine(folderPath, $"{name}.json");

            if (!File.Exists(walletFilePath))
                throw new FileNotFoundException($"No wallet file found at {walletFilePath}");

            // load the file from the local system
            Wallet wallet = JsonConvert.DeserializeObject<Wallet>(File.ReadAllText(walletFilePath));

            this.Load(wallet);
            return wallet;
        }

        /// <inheritdoc />
        public Wallet RecoverWallet(string password, string folderPath, string name, string network, string mnemonic, string passphrase = null, CoinType coinType = CoinType.Bitcoin, DateTimeOffset? creationTime = null)
        {
            // for now the passphrase is set to be the password by default.
            if (passphrase == null)
            {
                passphrase = password;
            }

            // generate the root seed used to generate keys
            ExtKey extendedKey = (new Mnemonic(mnemonic)).DeriveExtKey(passphrase);

            // create a wallet file 
            Wallet wallet = this.GenerateWalletFile(password, folderPath, name, WalletHelpers.GetNetwork(network), extendedKey, coinType, creationTime);

            this.Load(wallet);
            return wallet;
        }
        
        /// <inheritdoc />
        public string CreateNewAccount(string walletName, string accountName)
        {
            Wallet wallet = this.Wallets.SingleOrDefault(w => w.Name == walletName);
            if (wallet == null)
            {
                throw new Exception($"No wallet with name {walletName} could be found.");    
            }

            var lastAccountIndex = 0;

            // validate account creation
            if (wallet.Accounts.Any())
            {
                // check account with same name doesn't already exists
                if (wallet.Accounts.Any(a => a.Name == accountName))
                {
                    throw new Exception($"Account with name '{accountName}' already exists in '{walletName}'.");
                }

                // check account at index i - 1 contains transactions.
                lastAccountIndex = wallet.Accounts.Max(a => a.Index);
                HdAccount previousAccount = wallet.Accounts.Single(a => a.Index == lastAccountIndex);
                if (!previousAccount.ExternalAddresses.Any(addresses => addresses.Transactions.Any()) && !previousAccount.InternalAddresses.Any(addresses => addresses.Transactions.Any()))
                {
                    throw new Exception($"Cannot create new account '{accountName}' in '{walletName}' if the previous account '{previousAccount.Name}' has not been used.");
                }
            }

            wallet.Accounts = wallet.Accounts.Concat(new[] {new HdAccount
            {
                Index = lastAccountIndex + 1,
                ExternalAddresses = new List<HdAddress>(),
                InternalAddresses = new List<HdAddress>(),
                Name = accountName,
                CreationTime = DateTimeOffset.Now
            }});

            this.SaveToFile(wallet);

            return accountName;
        }

        public WalletGeneralInfoModel GetGeneralInfo(string name)
        {
            throw new System.NotImplementedException();
        }

        public WalletBalanceModel GetBalance(string walletName)
        {
            throw new System.NotImplementedException();
        }

        public WalletHistoryModel GetHistory(string walletName)
        {
            throw new System.NotImplementedException();
        }

        public WalletBuildTransactionModel BuildTransaction(string password, string address, Money amount, string feeType,
            bool allowUnconfirmed)
        {
            throw new System.NotImplementedException();
        }

        public bool SendTransaction(string transactionHex)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void DeleteWallet(string walletFilePath)
        {
            File.Delete(walletFilePath);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // TODO Safely persist the wallet before disposing
        }

        /// <summary>
        /// Generates the wallet file.
        /// </summary>
        /// <param name="password">The password used to encrypt sensitive info.</param>
        /// <param name="folderPath">The folder where the wallet will be generated.</param>
        /// <param name="name">The name of the wallet.</param>
        /// <param name="network">The network this wallet is for.</param>
        /// <param name="extendedKey">The root key used to generate keys.</param>
        /// <param name="coinType">The type of coin for which this wallet is created.</param>
        /// <param name="creationTime">The time this wallet was created.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        private Wallet GenerateWalletFile(string password, string folderPath, string name, Network network, ExtKey extendedKey, CoinType coinType = CoinType.Bitcoin, DateTimeOffset? creationTime = null)
        {
            string walletFilePath = Path.Combine(folderPath, $"{name}.json");

            if (File.Exists(walletFilePath))
                throw new InvalidOperationException($"Wallet already exists at {walletFilePath}");

            Wallet walletFile = new Wallet
            {
                Name = name,
                EncryptedSeed = extendedKey.PrivateKey.GetEncryptedBitcoinSecret(password, network).ToWif(),
                ChainCode = extendedKey.ChainCode,
                CreationTime = creationTime ?? DateTimeOffset.Now,
                Network = network,
                Accounts = new List<HdAccount>(),
                CoinType = coinType,
                WalletFilePath = walletFilePath
            };

            // create a folder if none exists and persist the file
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(walletFilePath)));
            File.WriteAllText(walletFilePath, JsonConvert.SerializeObject(walletFile, Formatting.Indented));

            return walletFile;
        }

        /// <summary>
        /// Saves the wallet into the file system.
        /// </summary>
        /// <param name="wallet">The wallet to save.</param>
        private void SaveToFile(Wallet wallet)
        {
            File.WriteAllText(wallet.WalletFilePath, JsonConvert.SerializeObject(wallet, Formatting.Indented));
        }

        /// <summary>
        /// Loads the wallet to be used by the manager.
        /// </summary>
        /// <param name="wallet">The wallet to load.</param>
        private void Load(Wallet wallet)
        {
            if (this.Wallets.All(w => w.Name != wallet.Name))
            {
                this.Wallets.Add(wallet);
            }
        }
    }
}
