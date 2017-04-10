using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Breeze.Wallet.Models
{
	public class WalletGeneralInfoModel
	{
		[JsonProperty(PropertyName = "walletFilePath")]
		public string WalletFilePath { get; set; }

		[JsonProperty(PropertyName = "encryptedSeed")]
		public string EncryptedSeed { get; set; }

		[JsonProperty(PropertyName = "chainCode")]
		public string ChainCode { get; set; }

		[JsonProperty(PropertyName = "network")]
		public string Network { get; set; }

		[JsonProperty(PropertyName = "creationTime")]
		public string CreationTime { get; set; }

		[JsonProperty(PropertyName = "isDecrypted")]
		public bool IsDecrypted { get; set; }

		[JsonProperty(PropertyName = "uniqueId")]
		public string UniqueId { get; set; }
	}
}
