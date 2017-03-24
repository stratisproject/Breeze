using NBitcoin;

namespace Breeze.Wallet.Wrappers
{
    public interface ITrackerWrapper
    {
        void NotifyAboutBlock(int height, Block block);

		uint256 GetLastProcessedBlock();
    }
}
