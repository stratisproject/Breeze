using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Breeze.Wallet.Models
{
	/// <summary>
	/// Object used to create a new wallet
	/// </summary>
	public class WalletCreationModel
	{
		[Required(ErrorMessage = "A password is required.")]
		public string Password { get; set; }

		public string Network { get; set; }

		[Required(ErrorMessage = "The folder path where the wallet will be created is required.")]
		public string FolderPath { get; set; }

		[Required(ErrorMessage = "The name of the wallet to create is missing.")]
		public string Name { get; set; }
	}

	public class WalletLoadModel
	{
		[Required(ErrorMessage = "A password is required.")]
		public string Password { get; set; }       

		[Required(ErrorMessage = "The folder path is required.")]
		public string FolderPath { get; set; }

		[Required(ErrorMessage = "The name of the wallet is missing.")]
		public string Name { get; set; }
	}

	public class WalletRecoveryModel
	{
		[Required(ErrorMessage = "A mnemonic is required.")]
		public string Mnemonic { get; set; }

		[Required(ErrorMessage = "A password is required.")]
		public string Password { get; set; }

		[Required(ErrorMessage = "The folder path is required.")]
		public string FolderPath { get; set; }

		[Required(ErrorMessage = "The name of the wallet is missing.")]
		public string Name { get; set; }

		public string Network { get; set; }
	}	
}
