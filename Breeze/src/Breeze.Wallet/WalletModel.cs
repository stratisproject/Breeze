using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Breeze.Wallet
{
	public class WalletModel
	{
		public string Network { get; set; }

		public string FileName { get; set; }

		public IEnumerable<string> Addresses { get; set; }
	}
}
