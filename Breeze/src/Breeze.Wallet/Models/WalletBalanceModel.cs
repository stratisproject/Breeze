using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin;
using Newtonsoft.Json;

namespace Breeze.Wallet.Models
{
    public class WalletBalanceModel
    {
		[JsonProperty(PropertyName = "isSynced")]
		public bool IsSynced { get; set; }

		[JsonProperty(PropertyName = "confirmed")]
		public Money Confirmed { get; set; }

		[JsonProperty(PropertyName = "unconfirmed")]
		public Money Unconfirmed { get; set; }
	}
}
