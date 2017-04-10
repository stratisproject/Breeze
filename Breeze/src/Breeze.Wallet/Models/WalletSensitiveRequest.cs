using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Breeze.Wallet.Models
{
    public class WalletSensitiveRequest
    {
		[Required(ErrorMessage = "A password is required.")]
		public string Password { get; set; }
	}
}
