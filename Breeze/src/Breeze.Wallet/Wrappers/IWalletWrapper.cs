﻿
namespace Breeze.Wallet.Wrappers
{
	/// <summary>
	/// An interface enabling wallet operations.
	/// </summary>
	public interface IWalletWrapper
	{
		string Create(string password, string folderPath, string name, string network);

		WalletModel Load(string password, string folderPath, string name);

		WalletModel Recover(string password, string folderPath, string name, string network, string mnemonic);
	}
}
