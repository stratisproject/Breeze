
using Breeze.Wallet.Models;
using HBitcoin.Models;
using NBitcoin;

namespace Breeze.Wallet.Wrappers
{
	/// <summary>
	/// An interface enabling wallet operations.
	/// </summary>
	public interface IWalletWrapper
	{
		
		string Create(string password, string folderPath, string name, string network);

		void Load(string password, string folderPath, string name);

		void Recover(string password, string folderPath, string name, string network, string mnemonic);

		WalletGeneralInfoModel GetGeneralInfo();

		WalletSensitiveInfoModel GetSensitiveInfo(string password);

		WalletStatusInfoModel GetStatusInfo();

		WalletBalanceModel GetBalance(string account);

		WalletHistoryModel GetHistory(string account);

		WalletBuildTransactionModel BuildTransaction(string account, string password, string address, Money amount, string feeType, bool allowUnconfirmed);

		bool SendTransaction(string transactionHex);
	}
}
