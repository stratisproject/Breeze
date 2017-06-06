using Breeze.TumbleBit.Client;
using NBitcoin;
using Stratis.Bitcoin;

namespace Breeze.TumbleBit
{
    /// <summary>
    /// Observer that receives notifications about the arrival of new <see cref="Block"/>s.
    /// </summary>
	public class BlockObserver : SignalObserver<Block>
    {
        private readonly ConcurrentChain chain;
        private readonly ITumbleBitManager tumbleBitManager;

        public BlockObserver(ConcurrentChain chain, ITumbleBitManager tumbleBitManager)
        {
            this.chain = chain;
            this.tumbleBitManager = tumbleBitManager;
        }

        /// <summary>
        /// Manages what happens when a new block is received.
        /// </summary>
        /// <param name="block">The new block</param>
        protected override void OnNextCore(Block block)
        {
            var hash = block.Header.GetHash();
            var height = this.chain.GetBlock(hash).Height;

            this.tumbleBitManager.ProcessBlock(height, block);
        }
    }
}
