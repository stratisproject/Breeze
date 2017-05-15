using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Breeze.Wallet.Helpers;
using Breeze.Wallet.Models;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;
using Newtonsoft.Json;
using Stratis.Bitcoin.Connection;
using Transaction = NBitcoin.Transaction;

namespace Breeze.Wallet
{
    /// <summary>
    /// A manager providing operations on wallets.
    /// </summary>
    public class WalletManager : IWalletManager
    {
        public List<Wallet> Wallets { get; }

        private const int UnusedAddressesBuffer = 20;

        private const int WalletRecoveryAccountsCount = 3;

        private const int WalletCreationAccountsCount = 2;

        private readonly CoinType coinType;

        private readonly ConnectionManager connectionManager;

        private Dictionary<Script, ICollection<TransactionData>> keysLookup;

        /// <summary>
        /// Occurs when a transaction is found.
        /// </summary>
        public event EventHandler<TransactionFoundEventArgs> TransactionFound;

        public WalletManager(ConnectionManager connectionManager, Network netwrok)
        {
            this.Wallets = new List<Wallet>();

            // find wallets and load them in memory
            foreach (var path in this.GetWalletFilesPaths())
            {
                this.Load(this.DeserializeWallet(path));
            }

            this.connectionManager = connectionManager;
            this.coinType = (CoinType)netwrok.Consensus.CoinType;

            // load data in memory for faster lookups
            this.LoadKeysLookup();

            // register events
            this.TransactionFound += this.OnTransactionFound;
        }

        /// <inheritdoc />
        public Mnemonic CreateWallet(string password, string folderPath, string name, string network, string passphrase = null)
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

            Network coinNetwork = WalletHelpers.GetNetwork(network);

            // create a wallet file 
            Wallet wallet = this.GenerateWalletFile(password, folderPath, name, coinNetwork, extendedKey);

            // generate multiple accounts and addresses from the get-go
            for (int i = 0; i < WalletCreationAccountsCount; i++)
            {
                HdAccount account = CreateNewAccount(wallet, this.coinType, password);
                this.CreateAddressesInAccount(account, coinNetwork, UnusedAddressesBuffer);
                this.CreateAddressesInAccount(account, coinNetwork, UnusedAddressesBuffer, true);
            }

            // save the changes to the file and add addresses to be tracked
            this.SaveToFile(wallet);
            this.LoadKeysLookup();
            this.Load(wallet);

            return mnemonic;
        }

        /// <inheritdoc />
        public Wallet LoadWallet(string password, string folderPath, string name)
        {
            string walletFilePath = Path.Combine(folderPath, $"{name}.json");

            // load the file from the local system
            Wallet wallet = this.DeserializeWallet(walletFilePath);

            this.Load(wallet);
            return wallet;
        }

        /// <inheritdoc />
        public Wallet RecoverWallet(string password, string folderPath, string name, string network, string mnemonic, string passphrase = null, DateTime? creationTime = null)
        {
            // for now the passphrase is set to be the password by default.
            if (passphrase == null)
            {
                passphrase = password;
            }

            // generate the root seed used to generate keys
            ExtKey extendedKey = (new Mnemonic(mnemonic)).DeriveExtKey(passphrase);

            Network coinNetwork = WalletHelpers.GetNetwork(network);

            // create a wallet file 
            Wallet wallet = this.GenerateWalletFile(password, folderPath, name, coinNetwork, extendedKey, creationTime);

            // generate multiple accounts and addresses from the get-go
            for (int i = 0; i < WalletRecoveryAccountsCount; i++)
            {
                HdAccount account = CreateNewAccount(wallet, this.coinType, password);
                this.CreateAddressesInAccount(account, coinNetwork, UnusedAddressesBuffer);
                this.CreateAddressesInAccount(account, coinNetwork, UnusedAddressesBuffer, true);
            }

            // save the changes to the file and add addresses to be tracked
            this.SaveToFile(wallet);
            this.LoadKeysLookup();
            this.Load(wallet);

            return wallet;
        }

        /// <inheritdoc />
        public HdAccount GetUnusedAccount(string walletName, CoinType coinType, string password)
        {
            Wallet wallet = this.GetWalletByName(walletName);

            return this.GetUnusedAccount(wallet, coinType, password);
        }

        /// <inheritdoc />
        public HdAccount GetUnusedAccount(Wallet wallet, CoinType coinType, string password)
        {
            // get the accounts root for this type of coin
            var accountsRoot = wallet.AccountsRoot.Single(a => a.CoinType == coinType);

            // check if an unused account exists
            if (accountsRoot.Accounts.Any())
            {
                // gets an unused account
                var firstUnusedAccount = accountsRoot.GetFirstUnusedAccount();
                if (firstUnusedAccount != null)
                {
                    return firstUnusedAccount;
                }
            }

            // all accounts contain transactions, create a new one
            var newAccount = this.CreateNewAccount(wallet, coinType, password);

            // save the changes to the file
            this.SaveToFile(wallet);
            return newAccount;
        }

        /// <inheritdoc />
        public HdAccount CreateNewAccount(Wallet wallet, CoinType coinType, string password)
        {
            // get the accounts for this type of coin
            var accounts = wallet.AccountsRoot.Single(a => a.CoinType == coinType).Accounts.ToList();

            int newAccountIndex = 0;
            if (accounts.Any())
            {
                newAccountIndex = accounts.Max(a => a.Index) + 1;
            }

            // get the extended pub key used to generate addresses for this account
            var privateKey = Key.Parse(wallet.EncryptedSeed, password, wallet.Network);
            var seedExtKey = new ExtKey(privateKey, wallet.ChainCode);
            var accountHdPath = $"m/44'/{(int)coinType}'/{newAccountIndex}'";
            KeyPath keyPath = new KeyPath(accountHdPath);
            ExtKey accountExtKey = seedExtKey.Derive(keyPath);
            ExtPubKey accountExtPubKey = accountExtKey.Neuter();

            var newAccount = new HdAccount
            {
                Index = newAccountIndex,
                ExtendedPubKey = accountExtPubKey.ToString(wallet.Network),
                ExternalAddresses = new List<HdAddress>(),
                InternalAddresses = new List<HdAddress>(),
                Name = $"account {newAccountIndex}",
                HdPath = accountHdPath,
                CreationTime = DateTimeOffset.Now
            };

            accounts.Add(newAccount);
            wallet.AccountsRoot.Single(a => a.CoinType == coinType).Accounts = accounts;

            return newAccount;
        }

        /// <inheritdoc />
        public string GetUnusedAddress(string walletName, CoinType coinType, string accountName)
        {
            Wallet wallet = this.GetWalletByName(walletName);

            // get the account
            HdAccount account = wallet.AccountsRoot.Single(a => a.CoinType == coinType).GetAccountByName(accountName);

            // validate address creation
            if (account.ExternalAddresses.Any())
            {
                // check last created address contains transactions.
                var firstUnusedExternalAddress = account.GetFirstUnusedReceivingAddress();
                if (firstUnusedExternalAddress != null)
                {
                    return firstUnusedExternalAddress.Address;
                }
            }

            // creates an address
            this.CreateAddressesInAccount(account, wallet.Network, 1);

            // persists the address to the wallet file
            this.SaveToFile(wallet);

            // adds the address to the list of tracked addresses
            this.LoadKeysLookup();
            return account.GetFirstUnusedReceivingAddress().Address;
        }

        /// <inheritdoc />
        public IEnumerable<HdAddress> GetHistoryByCoinType(string walletName, CoinType coinType)
        {
            Wallet wallet = this.GetWalletByName(walletName);

            return this.GetHistoryByCoinType(wallet, coinType);
        }

        /// <inheritdoc />
        public IEnumerable<HdAddress> GetHistoryByCoinType(Wallet wallet, CoinType coinType)
        {
            var accounts = wallet.GetAccountsByCoinType(coinType).ToList();

            foreach (var address in accounts.SelectMany(a => a.ExternalAddresses).Concat(accounts.SelectMany(a => a.InternalAddresses)))
            {
                if (address.Transactions.Any())
                {
                    yield return address;
                }
            }
        }

        /// <summary>
        /// Creates a number of addresses in the provided account.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="network">The network.</param>
        /// <param name="addressesQuantity">The number of addresses to create.</param>
        /// <param name="isChange">Whether the addresses added are change (internal) addresses or receiving (external) addresses.</param>
        /// <returns>A list of addresses in Base58.</returns>
        private List<string> CreateAddressesInAccount(HdAccount account, Network network, int addressesQuantity, bool isChange = false)
        {
            List<string> addressesCreated = new List<string>();

            var addresses = isChange ? account.InternalAddresses : account.ExternalAddresses;

            // gets the index of the last address with transactions
            int firstNewAddressIndex = 0;
            if (addresses.Any())
            {
                firstNewAddressIndex = addresses.Max(add => add.Index) + 1;
            }

            for (int i = firstNewAddressIndex; i < firstNewAddressIndex + addressesQuantity; i++)
            {
                // generate new receiving address
                BitcoinPubKeyAddress address = this.GenerateAddress(account.ExtendedPubKey, i, isChange, network);

                // add address details
                addresses.Add(new HdAddress
                {
                    Index = i,
                    HdPath = CreateBip44Path(account.GetCoinType(), account.Index, i, isChange),
                    ScriptPubKey = address.ScriptPubKey,
                    Address = address.ToString(),
                    Transactions = new List<TransactionData>()
                });

                addressesCreated.Add(address.ToString());
            }

            if (isChange)
            {
                account.InternalAddresses = addresses;
            }
            else
            {
                account.ExternalAddresses = addresses;
            }

            return addressesCreated;
        }

        public WalletGeneralInfoModel GetGeneralInfo(string name)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IEnumerable<HdAccount> GetAccountsByCoinType(string walletName, CoinType coinType)
        {
            Wallet wallet = this.GetWalletByName(walletName);

            return wallet.GetAccountsByCoinType(coinType);
        }

        /// <inheritdoc />
        public (string hex, Money fee) BuildTransaction(string walletName, string accountName, CoinType coinType, string password, string destinationAddress, Money amount, string feeType, bool allowUnconfirmed)
        {
            if (amount == Money.Zero)
            {
                throw new Exception($"Cannot send transaction with 0 {this.coinType}");
            }

            // get the wallet and the account
            Wallet wallet = this.GetWalletByName(walletName);
            HdAccount account = wallet.AccountsRoot.Single(a => a.CoinType == coinType).GetAccountByName(accountName);

            // get a list of transactions outputs that have not been spent
            IEnumerable<TransactionData> spendableTransactions = account.GetSpendableTransactions();

            // get total spendable balance in the account.
            var balance = spendableTransactions.Sum(t => t.Amount);

            // make sure we have enough funds
            if (balance < amount)
            {
                throw new Exception("Not enough funds.");
            }

            // calculate which addresses needs to be used as well as the fee to be charged
            var calculationResult = this.CalculateFees(spendableTransactions, amount);

            // get extended private key
            var privateKey = Key.Parse(wallet.EncryptedSeed, password, wallet.Network);
            var seedExtKey = new ExtKey(privateKey, wallet.ChainCode);

            var signingKeys = new HashSet<ISecret>();
            var coins = new List<Coin>();
            foreach (var transactionToUse in calculationResult.transactionsToUse)
            {
                var address = account.FindAddressForTransaction(transactionToUse.Id);
                ExtKey addressExtKey = seedExtKey.Derive(new KeyPath(address.HdPath));
                BitcoinExtKey addressPrivateKey = addressExtKey.GetWif(wallet.Network);
                signingKeys.Add(addressPrivateKey);

                coins.Add(new Coin(transactionToUse.Id, (uint)transactionToUse.Index, transactionToUse.Amount, address.ScriptPubKey));
            }

            // get address to send the change to
            var changeAddress = account.GetFirstUnusedChangeAddress();

            // get script destination address
            Script destinationScript = PayToPubkeyHashTemplate.Instance.GenerateScriptPubKey(new BitcoinPubKeyAddress(destinationAddress, wallet.Network));

            // build transaction
            var builder = new TransactionBuilder();
            Transaction tx = builder
                .AddCoins(coins)
                .AddKeys(signingKeys.ToArray())
                .Send(destinationScript, amount)
                .SetChange(changeAddress.ScriptPubKey)
                .SendFees(calculationResult.fee)
                .BuildTransaction(true);

            if (!builder.Verify(tx))
            {
                throw new Exception("Could not build transaction, please make sure you entered the correct data.");
            }

            return (tx.ToHex(), calculationResult.fee);
        }

        /// <summary>
        /// Calculates which outputs are to be used in the transaction, as well as the fees that will be charged.
        /// </summary>
        /// <param name="spendableTransactions">The transactions with unspent funds.</param>
        /// <param name="amount">The amount to be sent.</param>
        /// <returns>The collection of transactions to be used and the fee to be charged</returns>
        private (List<TransactionData> transactionsToUse, Money fee) CalculateFees(IEnumerable<TransactionData> spendableTransactions, Money amount)
        {
            // TODO make this a bit smarter!            
            List<TransactionData> transactionsToUse = new List<TransactionData>();
            foreach (var transaction in spendableTransactions)
            {
                transactionsToUse.Add(transaction);
                if (transactionsToUse.Sum(t => t.Amount) >= amount)
                {
                    break;
                }
            }

            Money fee = new Money(new decimal(0.001), MoneyUnit.BTC);
            return (transactionsToUse, fee);
        }

        /// <inheritdoc />
        public bool SendTransaction(string transactionHex)
        {
            // TODO move this to a behavior on the full node
            // parse transaction
            Transaction transaction = Transaction.Parse(transactionHex);
            TxPayload payload = new TxPayload(transaction);

            foreach (var node in this.connectionManager.ConnectedNodes)
            {
                node.SendMessage(payload);
            }

            return true;
        }

        /// <inheritdoc />
        public void ProcessBlock(int height, Block block)
        {
            Console.WriteLine($"block notification: height: {height}, block hash: {block.Header.GetHash()}, coin type: {this.coinType}");

            foreach (Transaction transaction in block.Transactions)
            {
                this.ProcessTransaction(transaction, height, block.Header.Time);
            }

            // update the wallets with the last processed block height
            foreach (var wallet in this.Wallets)
            {
                foreach (var accountRoot in wallet.AccountsRoot.Where(a => a.CoinType == this.coinType))
                {
                    accountRoot.LastBlockSyncedHeight = height;
                }
            }
        }

        /// <inheritdoc />
        public void ProcessTransaction(Transaction transaction, int? blockHeight = null, uint? blockTime = null)
        {
            Console.WriteLine($"transaction notification: tx hash {transaction.GetHash()}, coin type: {this.coinType}");

            // check the outputs
            foreach (var pubKey in this.keysLookup.Keys)
            {
                // check if the outputs contain one of our addresses
                var utxo = transaction.Outputs.SingleOrDefault(o => pubKey == o.ScriptPubKey);
                if (utxo != null)
                {
                    AddTransactionToWallet(transaction.GetHash(), transaction.Time, transaction.Outputs.IndexOf(utxo), utxo.Value, pubKey, blockHeight, blockTime);
                }
            }

            // check the inputs - include those that have a reference to a transaction containing one of our scripts and the same index            
            foreach (TxIn input in transaction.Inputs.Where(txIn => this.keysLookup.Values.SelectMany(v => v).Any(trackedTx => trackedTx.Id == txIn.PrevOut.Hash && trackedTx.Index == txIn.PrevOut.N)))
            {
                TransactionData tTx = this.keysLookup.Values.SelectMany(v => v).Single(trackedTx => trackedTx.Id == input.PrevOut.Hash && trackedTx.Index == input.PrevOut.N);

                // find the script this input references
                var keyToSpend = this.keysLookup.Single(v => v.Value.Contains(tTx)).Key;
                AddTransactionToWallet(transaction.GetHash(), transaction.Time, null, -tTx.Amount, keyToSpend, blockHeight, blockTime, tTx.Id, tTx.Index);
            }
        }

        /// <summary>
        /// Adds the transaction to the wallet.
        /// </summary>
        /// <param name="transactionHash">The transaction hash.</param>
        /// <param name="time">The time.</param>
        /// <param name="index">The index.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="script">The script.</param>
        /// <param name="blockHeight">Height of the block.</param>
        /// <param name="blockTime">The block time.</param>
        /// <param name="spendingTransactionId">The id of the transaction containing the output being spent, if this is a spending transaction.</param>
        /// <param name="spendingTransactionIndex">The index of the output in the transaction being referenced, if this is a spending transaction.</param>
        private void AddTransactionToWallet(uint256 transactionHash, uint time, int? index, Money amount, Script script, int? blockHeight = null, uint? blockTime = null, uint256 spendingTransactionId = null, int? spendingTransactionIndex = null)
        {
            // get the collection of transactions to add to.
            this.keysLookup.TryGetValue(script, out ICollection<TransactionData> trans);

            // if it's the first time we see this transaction
            if (trans != null && trans.All(t => t.Id != transactionHash))
            {
                trans.Add(new TransactionData
                {
                    Amount = amount,
                    BlockHeight = blockHeight,
                    Confirmed = blockHeight.HasValue,
                    Id = transactionHash,
                    CreationTime = DateTimeOffset.FromUnixTimeMilliseconds(blockTime ?? time),
                    Index = index
                });

                // if this is a spending transaction, mark the spent transaction as such
                if (spendingTransactionId != null)
                {
                    var transactions = this.keysLookup.Values.SelectMany(v => v).Where(t => t.Id == spendingTransactionId);
                    if (transactions.Any())
                    {
                        transactions.Single(t => t.Index == spendingTransactionIndex).SpentInTransaction = transactionHash;
                    }
                }
            }
            else if (trans.Any(t => t.Id == transactionHash && !t.Confirmed)) // if this is an unconfirmed transaction now received in a block
            {
                var foundTransaction = trans.Single(t => t.Id == transactionHash && !t.Confirmed);
                if (blockHeight != null)
                {
                    foundTransaction.Confirmed = true;
                }
            }

            // notify a transaction has been found
            this.TransactionFound?.Invoke(this, new TransactionFoundEventArgs(script, transactionHash));
        }

        private void OnTransactionFound(object sender, TransactionFoundEventArgs a)
        {
            foreach (Wallet wallet in this.Wallets)
            {
                foreach (var account in wallet.GetAccountsByCoinType(this.coinType))
                {
                    bool isChange;
                    if (account.ExternalAddresses.Any(address => address.ScriptPubKey == a.Script))
                    {
                        isChange = false;
                    }
                    else if (account.InternalAddresses.Any(address => address.ScriptPubKey == a.Script))
                    {
                        isChange = true;
                    }
                    else
                    {
                        continue;
                    }

                    // calculate how many accounts to add to keep a buffer of 20 unused addresses
                    int lastUsedAddressIndex = account.GetLastUsedAddress(isChange).Index;
                    int addressesCount = isChange ? account.InternalAddresses.Count() : account.ExternalAddresses.Count();
                    int emptyAddressesCount = addressesCount - lastUsedAddressIndex - 1;
                    int accountsToAdd = UnusedAddressesBuffer - emptyAddressesCount;
                    this.CreateAddressesInAccount(account, wallet.Network, accountsToAdd, isChange);

                    // persists the address to the wallet file
                    this.SaveToFile(wallet);
                }
            }

            this.LoadKeysLookup();
        }

        /// <inheritdoc />
        public void DeleteWallet(string walletFilePath)
        {
            File.Delete(walletFilePath);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // safely persist the wallets to the file system before disposing
            foreach (var wallet in this.Wallets)
            {
                this.SaveToFile(wallet);
            }
        }

        /// <summary>
        /// Generates the wallet file.
        /// </summary>
        /// <param name="password">The password used to encrypt sensitive info.</param>
        /// <param name="folderPath">The folder where the wallet will be generated.</param>
        /// <param name="name">The name of the wallet.</param>
        /// <param name="network">The network this wallet is for.</param>
        /// <param name="extendedKey">The root key used to generate keys.</param>
        /// <param name="creationTime">The time this wallet was created.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        private Wallet GenerateWalletFile(string password, string folderPath, string name, Network network, ExtKey extendedKey, DateTimeOffset? creationTime = null)
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
                AccountsRoot = new List<AccountRoot> {
                    new AccountRoot { Accounts = new List<HdAccount>(), CoinType = CoinType.Bitcoin },
                    new AccountRoot { Accounts = new List<HdAccount>(), CoinType = CoinType.Testnet },
                    new AccountRoot { Accounts = new List<HdAccount>(), CoinType = CoinType.Stratis} },
                WalletFilePath = walletFilePath,

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
        /// Gets the wallet located at the specified path.
        /// </summary>
        /// <param name="walletFilePath">The wallet file path.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        private Wallet DeserializeWallet(string walletFilePath)
        {
            if (!File.Exists(walletFilePath))
                throw new FileNotFoundException($"No wallet file found at {walletFilePath}");

            // load the file from the local system
            return JsonConvert.DeserializeObject<Wallet>(File.ReadAllText(walletFilePath));
        }

        /// <summary>
        /// Loads the wallet to be used by the manager.
        /// </summary>
        /// <param name="wallet">The wallet to load.</param>
        private void Load(Wallet wallet)
        {
            if (this.Wallets.Any(w => w.Name == wallet.Name))
            {
                return;
            }

            this.Wallets.Add(wallet);
        }

        private BitcoinPubKeyAddress GenerateAddress(string accountExtPubKey, int index, bool isChange, Network network)
        {
            int change = isChange ? 1 : 0;
            KeyPath keyPath = new KeyPath($"{change}/{index}");
            ExtPubKey extPubKey = ExtPubKey.Parse(accountExtPubKey).Derive(keyPath);
            return extPubKey.PubKey.GetAddress(network);
        }

        private IEnumerable<string> GetWalletFilesPaths()
        {
            // TODO look in user-chosen folder as well.
            // maybe the api can maintain a list of wallet paths it knows about
            var defaultFolderPath = GetDefaultWalletFolderPath();

            // create the directory if it doesn't exist
            Directory.CreateDirectory(defaultFolderPath);
            return Directory.EnumerateFiles(defaultFolderPath, "*.json", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Creates the bip44 path.
        /// </summary>
        /// <param name="coinType">Type of the coin.</param>
        /// <param name="accountIndex">Index of the account.</param>
        /// <param name="addressIndex">Index of the address.</param>
        /// <param name="isChange">if set to <c>true</c> [is change].</param>
        /// <returns></returns>
        public static string CreateBip44Path(CoinType coinType, int accountIndex, int addressIndex, bool isChange = false)
        {
            //// populate the items according to the BIP44 path 
            //// [m/purpose'/coin_type'/account'/change/address_index]

            int change = isChange ? 1 : 0;
            return $"m/44'/{(int)coinType}'/{accountIndex}'/{change}/{addressIndex}";
        }

        /// <summary>
        /// Gets the path of the default folder in which the wallets will be stored.
        /// </summary>
        /// <returns>The folder path for Windows, Linux or OSX systems.</returns>
        public static string GetDefaultWalletFolderPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return $@"{Environment.GetEnvironmentVariable("AppData")}\Breeze";
            }

            return $"{Environment.GetEnvironmentVariable("HOME")}/.breeze";
        }

        /// <summary>
        /// Loads the keys and transactions we're tracking in memory for faster lookups.
        /// </summary>
        /// <returns></returns>
        private void LoadKeysLookup()
        {
            this.keysLookup = new Dictionary<Script, ICollection<TransactionData>>();
            foreach (var wallet in this.Wallets)
            {
                var accounts = wallet.GetAccountsByCoinType(this.coinType);
                foreach (var account in accounts)
                {
                    var addresses = account.ExternalAddresses.Concat(account.InternalAddresses);
                    foreach (var address in addresses)
                    {
                        this.keysLookup.Add(address.ScriptPubKey, address.Transactions);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a wallet given its name.
        /// </summary>
        /// <param name="walletName">The name of the wallet to get.</param>
        /// <returns>A wallet or null if it doesn't exist</returns>
        private Wallet GetWalletByName(string walletName)
        {
            Wallet wallet = this.Wallets.SingleOrDefault(w => w.Name == walletName);
            if (wallet == null)
            {
                throw new Exception($"No wallet with name {walletName} could be found.");
            }

            return wallet;
        }
    }

    public class TransactionDetails
    {
        public uint256 Hash { get; set; }

        public int? Index { get; set; }

        public Money Amount { get; internal set; }

    }

    public class TransactionFoundEventArgs : EventArgs
    {
        public Script Script { get; set; }

        public uint256 TransactionHash { get; set; }

        public TransactionFoundEventArgs(Script script, uint256 transactionHash)
        {
            this.Script = script;
            this.TransactionHash = transactionHash;
        }
    }
}
