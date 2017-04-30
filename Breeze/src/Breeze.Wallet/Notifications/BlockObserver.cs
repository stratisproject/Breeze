using NBitcoin;
using Stratis.Bitcoin;

namespace Breeze.Wallet.Notifications
{
    /// <summary>
    /// Observer that receives notifications about the arrival of new <see cref="Block"/>s.
    /// </summary>
	public class BlockObserver : SignalObserver<Block>
    {
        private readonly ConcurrentChain chain;
        private readonly CoinType coinType;
        private readonly IWalletManager walletManager;

        public BlockObserver(ConcurrentChain chain, CoinType coinType, IWalletManager walletManager)
        {
            this.chain = chain;
            this.coinType = coinType;
            this.walletManager = walletManager;
        }

        /// <summary>
        /// Manages what happens when a new block is received.
        /// </summary>
        /// <param name="block">The new block</param>
        protected override void OnNextCore(Block block)
        {            
            var hash = block.Header.GetHash();
            var height = this.chain.GetBlock(hash).Height;

            this.walletManager.ProcessBlock(this.coinType, height, block);
        }
    }
}
