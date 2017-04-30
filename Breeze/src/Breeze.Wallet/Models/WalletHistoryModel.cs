using System;
using System.Collections.Generic;
using Breeze.Wallet.JsonConverters;
using NBitcoin;
using NBitcoin.JsonConverters;
using Newtonsoft.Json;

namespace Breeze.Wallet.Models
{
    public class WalletHistoryModel
    {
		[JsonProperty(PropertyName = "transactions")]
		public List<TransactionItem> Transactions { get; set; }
	}

	public class TransactionItem
	{
	    /// <summary>
	    /// The Base58 representation of this address.
	    /// </summary>
	    [JsonProperty(PropertyName = "address")]
	    public string Address { get; set; }

        [JsonProperty(PropertyName = "txId")]
		[JsonConverter(typeof(UInt256JsonConverter))]
        public uint256 TransactionId { get; set; }

		[JsonProperty(PropertyName = "amount")]
		public Money Amount { get; set; }

		[JsonProperty(PropertyName = "confirmed")]
		public bool Confirmed { get; set; } 

		[JsonProperty(PropertyName = "timestamp")]
		[JsonConverter(typeof(DateTimeOffsetConverter))]
        public DateTimeOffset Timestamp { get; set; }        
    }
}
