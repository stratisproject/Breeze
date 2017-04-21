using Breeze.Wallet.Models;
using NBitcoin;

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

		WalletGeneralInfoModel GetGeneralInfo(string walletName);

		WalletBalanceModel GetBalance(string walletName);

		WalletHistoryModel GetHistory(string walletName);

		WalletBuildTransactionModel BuildTransaction(string password, string address, Money amount, string feeType, bool allowUnconfirmed);

		bool SendTransaction(string transactionHex);
	}
}
