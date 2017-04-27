using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Breeze.Wallet.Models
{
    public class CreateAddressModel
    {
        /// <summary>
        /// The name of the wallet in which to create the address.
        /// </summary>
        [Required]
        public string WalletName { get; set; }

        /// <summary>
        /// The type of coin this account contains.
        /// </summary>
        [Required]
        public CoinType CoinType { get; set; }

        /// <summary>
        /// The name of the account in which to create the address.
        /// </summary>
        [Required]
        public string AccountName { get; set; }
    }
}
