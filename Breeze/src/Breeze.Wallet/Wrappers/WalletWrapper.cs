﻿using System;
using System.IO;
using System.Linq;
using System.Security;
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
		private readonly SafeAccount aliceAccount = new SafeAccount(1);
		private readonly SafeAccount bobAccount = new SafeAccount(2);
		private WalletJob walletJob = null;
		private Task walletJobTask = null;
		private CancellationTokenSource walletJobTaskCts = new CancellationTokenSource();
		private string password = null;

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
		public void Load(string password, string folderPath, string name)
		{
			Safe safe = Safe.Load(password, Path.Combine(folderPath, $"{name}.json"));
			this.password = password;

			// todo add Tor support (DotNetTor nuget) and pass HttpClientHandle to the constructor
			// the tor support should be added statically (executables shipped with the project), the tor process should be opened programatically
			// https://www.codeproject.com/Articles/1161078/WebControls/
			walletJob = new WalletJob(safe, null, false, aliceAccount, bobAccount);
			walletJobTask = walletJob.StartAsync(walletJobTaskCts.Token);
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
		public void Recover(string password, string folderPath, string name, string network, string mnemonic)
		{
			Safe wallet = Safe.Recover(new Mnemonic(mnemonic), password, Path.Combine(folderPath, $"{name}.json"),
					this.GetNetwork(network));
		}

		private SafeAccount GetAccount(string aliceOrBob)
		{
			var val = aliceOrBob.Trim().ToLowerInvariant();
			if(val == "alice"
				|| val == "account1"
				|| val == "walletaccount1"
				|| val == "safeaccount1")
				return aliceAccount;
			if(val == "bob"
				|| val == "account2"
				|| val == "walletaccount2"
				|| val == "safeaccount3")
				return bobAccount;
			throw new ArgumentException("Wrong account");
		}

		private FeeType GetFeeType(string feeType)
		{
			var val = feeType.Trim().ToLowerInvariant();
			if (val == "low")
				return FeeType.Low;
			if (val == "medium")
				return FeeType.Medium;
			if (val == "heigh")
				return FeeType.High;
			throw new ArgumentException("Wrong feeType");
		}

		private Network GetNetwork(string network)
		{
			// any network different than MainNet will default to TestNet			
			var val = network.Trim().ToLowerInvariant();
			if(val == "main" || val == "mainnet")
				return Network.Main;
			if (val == "test" || val == "testnet")
				return Network.TestNet;
			throw new ArgumentException("Wrong network");
		}

		public WalletGeneralInfoModel GetGeneralInfo()
		{
			throw new System.NotImplementedException();
		}
		public WalletSensitiveInfoModel GetSensitiveInfo(string password)
		{
			if(this.password == null || password != this.password)
				throw new SecurityException("Wrong password or wallet is not decrypted yet");

			throw new System.NotImplementedException();
		}
		public WalletStatusInfoModel GetStatusInfo()
		{
			throw new System.NotImplementedException();
		}

		public WalletBalanceModel GetBalance(string account)
		{
			throw new System.NotImplementedException();
		}

		public WalletHistoryModel GetHistory(string account)
		{
			var model = new WalletHistoryModel();
			foreach(var record in walletJob.GetSafeHistory(GetAccount(account)))
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

		public WalletBuildTransactionModel BuildTransaction(string account, string password, string address, Money amount, string feeType,
			bool allowUnconfirmed)
		{
			if(this.password == null || password != this.password)
				throw new SecurityException("Wrong password or wallet is not decrypted yet");

			var res = walletJob.BuildTransactionAsync(
				BitcoinAddress.Create(address, walletJob.Safe.Network).ScriptPubKey,
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
