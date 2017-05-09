using System;
using System.Collections.Generic;
using System.Linq;
using Breeze.Wallet.JsonConverters;
using NBitcoin;
using NBitcoin.JsonConverters;
using Newtonsoft.Json;

namespace Breeze.Wallet
{
    /// <summary>
    /// A wallet
    /// </summary>
    public class Wallet
    {
        /// <summary>
        /// The name of this wallet.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The seed for this wallet, password encrypted.
        /// </summary>
        [JsonProperty(PropertyName = "encryptedSeed")]
        public string EncryptedSeed { get; set; }

        /// <summary>
        /// The chain code. 
        /// </summary>
        [JsonProperty(PropertyName = "chainCode")]
        [JsonConverter(typeof(ByteArrayConverter))]
        public byte[] ChainCode { get; set; }
        
        /// <summary>
        /// The network this wallet is for.
        /// </summary>
        [JsonProperty(PropertyName = "network")]
        [JsonConverter(typeof(NetworkConverter))]
        public Network Network { get; set; }

        /// <summary>
        /// The time this wallet was created.
        /// </summary>
        [JsonProperty(PropertyName = "creationTime")]
        [JsonConverter(typeof(DateTimeOffsetConverter))]
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// The location of the wallet file on the local system.
        /// </summary>
        [JsonProperty(PropertyName = "walletFilePath")]
        public string WalletFilePath { get; set; }

        /// <summary>
        /// The root of the accounts tree.
        /// </summary>
        [JsonProperty(PropertyName = "accountsRoot")]
        public IEnumerable<AccountRoot> AccountsRoot { get; set; }
    }

    /// <summary>
    /// The root for the accounts for any type of coins.
    /// </summary>
    public class AccountRoot
    {
        /// <summary>
        /// The type of coin, Bitcoin or Stratis.
        /// </summary>
        [JsonProperty(PropertyName = "coinType")]
        public CoinType CoinType { get; set; }

        /// <summary>
        /// The height of the last block that was synced.
        /// </summary>
        [JsonProperty(PropertyName = "lastBlockSyncedHeight", NullValueHandling = NullValueHandling.Ignore)]
        public int? LastBlockSyncedHeight { get; set; }
        
        /// <summary>
        /// The accounts used in the wallet.
        /// </summary>
        [JsonProperty(PropertyName = "accounts")]
        public IEnumerable<HdAccount> Accounts { get; set; }

        /// <summary>
        /// Gets the first account that contains no transaction.
        /// </summary>
        /// <returns>An unused account</returns>
        public HdAccount GetFirstUnusedAccount()
        {
            var unusedAccounts = this.Accounts.Where(acc => !acc.ExternalAddresses.Any() && !acc.InternalAddresses.Any()).ToList();
            if (!unusedAccounts.Any())
            {
                return null;
            }

            // gets the unused account with the lowest index
            var index = unusedAccounts.Min(a => a.Index);
            return unusedAccounts.Single(a => a.Index == index);
        }
    }

    /// <summary>
    /// The type of coin, as specified in BIP44.
    /// </summary>
    /// <remarks>For more, see https://github.com/satoshilabs/slips/blob/master/slip-0044.md</remarks>
    public enum CoinType
    {
        /// <summary>
        /// Bitcoin
        /// </summary>
        Bitcoin = 0,

        /// <summary>
        /// Testnet (all coins)
        /// </summary>
        Testnet = 1,

        /// <summary>
        /// Stratis
        /// </summary>
        Stratis = 105
    }

    /// <summary>
    /// An Hd account's details.
    /// </summary>
    public class HdAccount
    {
        /// <summary>
        /// The index of the account.
        /// </summary>
        /// <remarks>
        /// According to BIP44, an account at index (i) can only be created when the account
        /// at index (i - 1) contains transactions.
        /// </remarks>
        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }

        /// <summary>
        /// The name of this account.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// A path to the account as defined in BIP44.
        /// </summary>
        [JsonProperty(PropertyName = "hdPath")]
        public string HdPath { get; set; }

        /// <summary>
        /// An extended pub key used to generate addresses.
        /// </summary>
        [JsonProperty(PropertyName = "extPubKey")]
        public string ExtendedPubKey { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        [JsonProperty(PropertyName = "creationTime")]
        [JsonConverter(typeof(DateTimeOffsetConverter))]
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// The list of external addresses, typically used for receiving money.
        /// </summary>
        [JsonProperty(PropertyName = "externalAddresses")]
        public IEnumerable<HdAddress> ExternalAddresses { get; set; }

        /// <summary>
        /// The list of internal addresses, typically used to receive change.
        /// </summary>
        [JsonProperty(PropertyName = "internalAddresses")]
        public IEnumerable<HdAddress> InternalAddresses { get; set; }

        /// <summary>
        /// Gets the type of coin this account is for.
        /// </summary>
        /// <returns>A <see cref="CoinType"/>.</returns>
        public CoinType GetCoinType()
        {
            string[] pathElements = this.HdPath.Split('/');
            int coinType = int.Parse(pathElements[2].Replace("'", string.Empty));
            return (CoinType)coinType;
        }

        /// <summary>
        /// Gets the first receiving address that contains no transaction.
        /// </summary>
        /// <returns>An unused address</returns>
        public HdAddress GetFirstUnusedExternalAddress()
        {
            var unusedAddresses = this.ExternalAddresses.Where(acc => !acc.Transactions.Any()).ToList();
            if (!unusedAddresses.Any())
            {
                return null;
            }

            // gets the unused address with the lowest index
            var index = unusedAddresses.Min(a => a.Index);
            return unusedAddresses.Single(a => a.Index == index);
        }
    }

    /// <summary>
    /// An Hd address.
    /// </summary>
    public class HdAddress
    {
        /// <summary>
        /// The index of the address.
        /// </summary>
        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        [JsonProperty(PropertyName = "creationTime")]
        [JsonConverter(typeof(DateTimeOffsetConverter))]
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// The script pub key for this address.
        /// </summary>
        [JsonProperty(PropertyName = "scriptPubKey")]
        [JsonConverter(typeof(ScriptJsonConverter))]
        public Script ScriptPubKey { get; set; }

        /// <summary>
        /// The Base58 representation of this address.
        /// </summary>
        [JsonProperty(PropertyName = "address")]        
        public string Address { get; set; }

        /// <summary>
        /// A path to the address as defined in BIP44.
        /// </summary>
        [JsonProperty(PropertyName = "hdPath")]
        public string HdPath { get; set; }

        /// <summary>
        /// A list detailing which blocks have been scanned for this address.
        /// </summary>
        [JsonIgnore]
        public SortedList<int, int> BlocksScanned { get; set; }

        /// <summary>
        /// A list of transactions involving this address.
        /// </summary>
        [JsonProperty(PropertyName = "transactions")]
        public IEnumerable<TransactionData> Transactions { get; set; }
    }

    /// <summary>
    /// An object containing transaction data.
    /// </summary>
    public class TransactionData
    {
        /// <summary>
        /// Transaction id.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        [JsonConverter(typeof(UInt256JsonConverter))]
        public uint256 Id { get; set; }

        /// <summary>
        /// The transaction amount.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        [JsonConverter(typeof(MoneyJsonConverter))]
        public Money Amount { get; set; }

        /// <summary>
        /// The index of this scriptPubKey in the transaction it is contained.
        /// </summary>
        [JsonProperty(PropertyName = "index", NullValueHandling = NullValueHandling.Ignore)]
        public int? Index { get; set; }

        /// <summary>
        /// The height of the block including this transaction.
        /// </summary>
        [JsonProperty(PropertyName = "blockHeight")]
        public int? BlockHeight { get; set; }

        /// <summary>
        /// Whether this transaction has been confirmed or not.
        /// </summary>
        [JsonProperty(PropertyName = "confirmed")]
        public bool Confirmed { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        [JsonProperty(PropertyName = "creationTime")]
        [JsonConverter(typeof(DateTimeOffsetConverter))]
        public DateTimeOffset CreationTime { get; set; }
    }
}