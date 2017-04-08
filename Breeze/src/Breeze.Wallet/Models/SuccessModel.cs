using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Breeze.Wallet.Models
{
    public class SuccessModel
    {
		[JsonProperty(PropertyName = "success")]
		public bool Success { get; set; }
	}
}
