
namespace Breeze.Wallet.Wrappers
{
	/// <summary>
	/// An interface enabling wallet operations.
	/// </summary>
	public interface ISafeWrapper
	{
		string Create(string password, string folderPath, string name, string network);

		SafeModel Load(string password, string folderPath, string name);

		SafeModel Recover(string password, string folderPath, string name, string network, string mnemonic);
	}
}
