using System;
using System.IO;
using System.Linq;
using Breeze.Wallet.Helpers;
using Breeze.Wallet.Models;
using HBitcoin.KeyManagement;
using NBitcoin;

namespace Breeze.Wallet.Wrappers
{
	/// <summary>
	/// An implementation of the <see cref="IWalletWrapper"/> interface.
	/// </summary>
	public class WalletWrapper : IWalletWrapper
	{
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
			Safe wallet = Safe.Create(out mnemonic, password, Path.Combine(folderPath, $"{name}.json"), WalletHelpers.GetNetwork(network));
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
			Safe wallet = Safe.Load(password, Path.Combine(folderPath, $"{name}.json"));

			//TODO review here which data should be returned
			return new WalletModel
			{
				Network = wallet.Network.Name,
				Addresses = wallet.GetFirstNAddresses(10).Select(a => a.ToWif()),
				FileName = wallet.WalletFilePath
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
			Safe wallet = Safe.Recover(new Mnemonic(mnemonic), password, Path.Combine(folderPath, $"{name}.json"), WalletHelpers.GetNetwork(network));

			//TODO review here which data should be returned
			return new WalletModel
			{
				Network = wallet.Network.Name,
				Addresses = wallet.GetFirstNAddresses(10).Select(a => a.ToWif()),
				FileName = wallet.WalletFilePath
			};
		}
        
		public WalletGeneralInfoModel GetGeneralInfo(string name)
		{
			throw new System.NotImplementedException();
		}

		public WalletBalanceModel GetBalance(string walletName)
		{
			throw new System.NotImplementedException();
		}

		public WalletHistoryModel GetHistory(string walletName)
		{
			throw new System.NotImplementedException();
		}

		public WalletBuildTransactionModel BuildTransaction(string password, string address, Money amount, string feeType,
			bool allowUnconfirmed)
		{
			throw new System.NotImplementedException();
		}

		public bool SendTransaction(string transactionHex)
		{
			throw new System.NotImplementedException();
		}
	}
}
