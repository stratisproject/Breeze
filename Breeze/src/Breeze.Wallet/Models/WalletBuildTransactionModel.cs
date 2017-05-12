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
		[JsonProperty(PropertyName = "fee")]
		public Money Fee { get; set; }
        
		[JsonProperty(PropertyName = "hex")]
		public string Hex { get; set; }        
	}   	
}
