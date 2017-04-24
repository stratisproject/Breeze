using System;
using Breeze.Wallet.JsonConverters;
using NBitcoin;
using Newtonsoft.Json;

namespace Breeze.Wallet
{
    /// <summary>
    /// A wallet
    /// </summary>
    public class Wallet
    {

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
        [JsonIgnore]
        public string WalletFilePath { get; set; }

        /// <summary>
        /// The hierarchy of the wallet's accounts and addresses.
        /// </summary>
        [JsonProperty(PropertyName = "hierarchy")]
        public WalletHierarchy Hierarchy { get; set; }
    }
}