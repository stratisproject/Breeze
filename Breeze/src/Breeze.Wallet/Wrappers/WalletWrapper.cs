using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Breeze.Wallet.Models;
using HBitcoin.FullBlockSpv;
using HBitcoin.KeyManagement;
using HBitcoin.Models;
using NBitcoin;

namespace Breeze.Wallet.Wrappers
{
	/// <summary>
	/// An implementation of the <see cref="IWalletWrapper"/> interface.
	/// </summary>
	public class WalletWrapper : IWalletWrapper
	{
		private readonly SafeAccount _aliceAccount = new SafeAccount(1);
		private readonly SafeAccount _bobAccount = new SafeAccount(2);
		private WalletJob _walletJob = null;
		private Task _walletJobTask = null;
		private CancellationTokenSource _walletJobTaskCts = new CancellationTokenSource();

		/// <summary>
		/// Creates a wallet on the local device.
		/// </summary>
		/// <param name="password">The user's password.</param>
		/// <param name="folderPath">The folder where the wallet will be saved.</param>
		/// <param name="name">The name of the wallet.</param>
		/// <param name="network">The network for which to create a wallet.</param>
		/// <returns>A mnemonic allowing recovery of the wallet.</returns>
		public string Create(string password, string folderPath, string name, string network)
		{			
			Mnemonic mnemonic;
			Safe wallet = Safe.Create(out mnemonic, password, Path.Combine(folderPath, $"{name}.json"), this.GetNetwork(network));
			return mnemonic.ToString();
		}

		/// <summary>
		/// Loads a wallet from the local device.
		/// </summary>
		/// <param name="password">The user's password.</param>
		/// <param name="folderPath">The folder where the wallet will be loaded.</param>
		/// <param name="name">The name of the wallet.</param>
		/// <returns>The wallet loaded from the local device</returns>
		public WalletModel Load(string password, string folderPath, string name)
		{
			Safe safe = Safe.Load(password, Path.Combine(folderPath, $"{name}.json"));

			// todo add Tor support (DotNetTor nuget) and pass HttpClientHandle to the constructor
			// the tor support should be added statically (executables shipped with the project), the tor process should be opened programatically
			// https://www.codeproject.com/Articles/1161078/WebControls/
			_walletJob = new WalletJob(safe, null, false, _aliceAccount, _bobAccount);
			_walletJobTask = _walletJob.StartAsync(_walletJobTaskCts.Token);

			//TODO review here which data should be returned
			return new WalletModel
			{
				Network = safe.Network.Name,
				Addresses = safe.GetFirstNAddresses(10).Select(a => a.ToWif()),
				FileName = safe.WalletFilePath
			};
		}

		/// <summary>
		/// Recovers a wallet from the local device.
		/// </summary>
		/// <param name="password">The user's password.</param>
		/// <param name="folderPath">The folder where the wallet will be loaded.</param>
		/// <param name="name">The name of the wallet.</param>
		/// <param name="network">The network in which to creae this wallet</param>
		/// <param name="mnemonic">The user's mnemonic for the wallet.</param>		
		/// <returns></returns>
		public WalletModel Recover(string password, string folderPath, string name, string network, string mnemonic)
		{
			Safe wallet = Safe.Recover(new Mnemonic(mnemonic), password, Path.Combine(folderPath, $"{name}.json"), this.GetNetwork(network));

			//TODO review here which data should be returned
			return new WalletModel
			{
				Network = wallet.Network.Name,
				Addresses = wallet.GetFirstNAddresses(10).Select(a => a.ToWif()),
				FileName = wallet.WalletFilePath
			};
		}

		private SafeAccount GetAccount(string aliceOrBob)
		{
			var trimmed = aliceOrBob.Trim();
			if (trimmed.Equals("alice", StringComparison.OrdinalIgnoreCase))
				return _aliceAccount;
			if(trimmed.Equals("bob", StringComparison.OrdinalIgnoreCase))
				return _bobAccount;
			throw new ArgumentException("Wrong account");
		}

		private FeeType GetFeeType(string feeType)
		{
			var trimmed = feeType.Trim();
			if (trimmed.Equals("low", StringComparison.OrdinalIgnoreCase))
				return FeeType.Low;
			if (trimmed.Equals("medium", StringComparison.OrdinalIgnoreCase))
				return FeeType.Medium;
			if (trimmed.Equals("heigh", StringComparison.OrdinalIgnoreCase))
				return FeeType.High;
			throw new ArgumentException("Wrong feeType");
		}

		private Network GetNetwork(string network)
		{
			// any network different than MainNet will default to TestNet			
			var trimmed = network.Trim();
			if (trimmed.Equals("main", StringComparison.OrdinalIgnoreCase)
				|| trimmed.Equals("mainnet", StringComparison.OrdinalIgnoreCase))
				return Network.Main;
			if (trimmed.Equals("test", StringComparison.OrdinalIgnoreCase)
				|| trimmed.Equals("testnet", StringComparison.OrdinalIgnoreCase))
				return Network.TestNet;
			throw new ArgumentException("Wrong network");
		}

		public WalletInfoModel GetInfo(string name)
		{
			throw new System.NotImplementedException();
		}

		public WalletBalanceModel GetBalance(string walletName)
		{
			throw new System.NotImplementedException();
		}

		public WalletHistoryModel GetHistory(string walletName)
		{
			var model = new WalletHistoryModel();
			foreach(var record in _walletJob.GetSafeHistory())
			{
				model.Transactions.Add(new TransactionItem
				{
					TransactionId = record.TransactionId,
					Amount = record.Amount,
					Confirmed = record.Confirmed,
					Timestamp = record.TimeStamp
				});
			}
			return model;
		}

		public WalletBuildTransactionModel BuildTransaction(string password, string address, Money amount, string feeType,
			bool allowUnconfirmed, string account)
		{
			var res = _walletJob.BuildTransactionAsync(
				BitcoinAddress.Create(address, _walletJob.Safe.Network).ScriptPubKey,
				amount,
				GetFeeType(feeType),
				GetAccount(account),
				allowUnconfirmed).Result;

			return new WalletBuildTransactionModel
			{
				Fee = res.Fee,
				Hex = res.Transaction.ToHex(),
				SpendsUnconfirmed = res.SpendsUnconfirmed,
				FeePercentOfSent = res.FeePercentOfSent
			};
		}

		public bool SendTransaction(string transactionHex)
		{
			try
			{
				var res = WalletJob.SendTransactionAsync(new NBitcoin.Transaction(transactionHex)).Result;
				if (res.Success) return true;
				else return false;
			}
			catch
			{
				return false;
			}
		}
	}
}
