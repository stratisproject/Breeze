using NBitcoin;
using HBitcoin.FullBlockSpv;
using HBitcoin.Models;

namespace Breeze.Api.Wrappers
{
    public class TrackerWrapper : ITrackerWrapper
    {
        private readonly Tracker tracker;

        public TrackerWrapper(Network network)
        {
            this.tracker = new Tracker(network);              
        }
        
        public void NotifyAboutBlock(int height, Block block)
        {
            this.tracker.AddOrReplaceBlock(new Height(height), block);           
        }
    }
}
