using NBitcoin;
using Stratis.Bitcoin;
using Breeze.Wallet.Wrappers;
using Stratis.Bitcoin.Builder;

namespace Breeze.Wallet.Notifications
{
    /// <summary>
    /// Observer that receives notifications about the arrival of new <see cref="Block"/>s.
    /// </summary>
	public class BlockObserver : SignalObserver<Block>
	{
	    private readonly ConcurrentChain chain;

        private readonly ITrackerWrapper trackerWrapper;

	    public BlockObserver(ConcurrentChain chain, ITrackerWrapper trackerWrapper)
	    {
	        this.chain = chain; 
            this.trackerWrapper = trackerWrapper;
	    }
        
        /// <summary>
        /// Manages what happens when a new block is received.
        /// </summary>
        /// <param name="block">The new block</param>
	    protected override void OnNextCore(Block block)
	    {
            var hash = block.Header.GetHash();
            var height = this.chain.GetBlock(hash).Height;

            this.trackerWrapper.NotifyAboutBlock(height, block);	        
	    }
	}
}
