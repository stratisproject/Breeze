using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin;
using Newtonsoft.Json;

namespace Breeze.Wallet.Models
{
    public class WalletBuildTransactionModel
    {
		[JsonProperty(PropertyName = "spendsUnconfirmed")]
		public bool SpendsUnconfirmed { get; set; }

		[JsonProperty(PropertyName = "fee")]
		public Money Fee { get; set; }

		[JsonProperty(PropertyName = "feePercentOfSent")]
		public double FeePercentOfSent { get; set; }

		[JsonProperty(PropertyName = "hex")]
		public string Hex { get; set; }

		[JsonProperty(PropertyName = "transaction")]
		public Transaction Transaction { get; set; }
	}

	public class Transaction
	{
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }

		[JsonProperty(PropertyName = "isCoinbase")]
		public bool IsCoinbase { get; set; }

		[JsonProperty(PropertyName = "block")]
		public string Block { get; set; }

		[JsonProperty(PropertyName = "spentCoins")]
		public IEnumerable<TransactionDetails> SpentCoins { get; set; }

		[JsonProperty(PropertyName = "receivedCoins")]
		public IEnumerable<TransactionDetails> ReceivedCoins { get; set; }

		[JsonProperty(PropertyName = "firstSendDate")]
		public DateTime FirstSeenDate { get; set; }

		[JsonProperty(PropertyName = "fees")]
		public Money Fees { get; set; }
	}

	public class TransactionDetails
	{
		[JsonProperty(PropertyName = "transactionId")]
		public string TransactionId { get; set; }

		[JsonProperty(PropertyName = "index")]
		public int Index { get; set; }

		[JsonProperty(PropertyName = "value")]
		public int Value { get; set; }

		[JsonProperty(PropertyName = "scriptPubKey")]
		public string ScriptPubKey { get; set; }

		[JsonProperty(PropertyName = "redeemScript")]
		public string RedeemScript { get; set; }
	}	
}
