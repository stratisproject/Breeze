using System.Collections.Generic;
using NBitcoin;
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
		[JsonProperty(PropertyName = "txId")]
		public string TransactionId { get; set; }

		[JsonProperty(PropertyName = "amount")]
		public Money Amount { get; set; }

		[JsonProperty(PropertyName = "confirmed")]
		public bool Confirmed { get; set; } 

		[JsonProperty(PropertyName = "timestamp")]
		public string Timestamp { get; set; }
	}
}
