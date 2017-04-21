using NBitcoin;

namespace Breeze.Wallet.Wrappers
{
    public interface ITrackerWrapper
    {
        void NotifyAboutBlock(int height, Block block);

        void NotifyAboutTransaction(Transaction transaction);

        uint256 GetLastProcessedBlock();
    }
}
