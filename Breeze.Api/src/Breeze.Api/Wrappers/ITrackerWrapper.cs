using NBitcoin;

namespace Breeze.Api.Wrappers
{
    public interface ITrackerWrapper
    {
        void NotifyAboutBlock(int height, Block block);
    }
}
