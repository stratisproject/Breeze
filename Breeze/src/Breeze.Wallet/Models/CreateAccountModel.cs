using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Breeze.Wallet.Models
{
    public class CreateAccountModel
    {
        /// <summary>
        /// The name of the wallet in which to create the account.
        /// </summary>
        [Required]
        public string WalletName { get; set; }

        /// <summary>
        /// The name of the account.
        /// </summary>
        [Required]
        public string AccountName { get; set; }
    }
}
