using System;
using System.IO;
using NBitcoin;
using Newtonsoft.Json;

namespace Breeze.Wallet
{
    /// <summary>
    /// A manager providing operations on wallets.
    /// </summary>
    public class WalletManager : IWalletManager
    {        
        /// <inheritdoc />
        public Mnemonic CreateWallet(string password, string walletFilePath, Network network, string passphrase = null)
        {
            // generate the root seed used to generate keys from a mnemonic picked at random 
            // and a passphrase optionally provided by the user
            Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            ExtKey extendedKey = mnemonic.DeriveExtKey(passphrase);

            // create a wallet file 
            Wallet wallet = this.GenerateWalletFile(password, walletFilePath, network, extendedKey);            
            return mnemonic;
        }

        /// <inheritdoc />
        public Wallet LoadWallet(string password, string walletFilePath)
        {
            if (!File.Exists(walletFilePath))
                throw new FileNotFoundException($"No wallet file found at {walletFilePath}");

            // load the file from the local system
            Wallet wallet = JsonConvert.DeserializeObject<Wallet>(File.ReadAllText(walletFilePath));            
            return wallet;
        }

        /// <inheritdoc />
        public Wallet RecoverWallet(Mnemonic mnemonic, string password, string walletFilePath, Network network, string passphrase = null, DateTimeOffset? creationTime = null)
        {
            // generate the root seed used to generate keys
            ExtKey extendedKey = mnemonic.DeriveExtKey(passphrase);

            // create a wallet file 
            Wallet wallet = this.GenerateWalletFile(password, walletFilePath, network, extendedKey, creationTime);           
            return wallet;
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
        /// <param name="walletFilePath">The location of the wallet file.</param>
        /// <param name="network">The network this wallet is for.</param>
        /// <param name="extendedKey">The root key used to generate keys.</param>
        /// <param name="creationTime">The time this wallet was created.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        private Wallet GenerateWalletFile(string password, string walletFilePath, Network network, ExtKey extendedKey, DateTimeOffset? creationTime = null)
        {
            if (File.Exists(walletFilePath))
                throw new InvalidOperationException($"Wallet already exists at {walletFilePath}");

            Wallet walletFile = new Wallet
            {
                EncryptedSeed = extendedKey.PrivateKey.GetEncryptedBitcoinSecret(password, network).ToWif(),
                ChainCode = extendedKey.ChainCode,
                CreationTime = creationTime ?? DateTimeOffset.Now,
                Network = network                
            };

            // create a folder if none exists and persist the file
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(walletFilePath)));
            File.WriteAllText(walletFilePath, JsonConvert.SerializeObject(walletFile, Formatting.Indented));

            return walletFile;
        }
    }
}
