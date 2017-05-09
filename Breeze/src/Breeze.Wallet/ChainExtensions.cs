using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace Breeze.Wallet
{
    public static class ChainExtensions
    {
        /// <summary>
        /// Determines whether the chain is downloaded and up to date.
        /// </summary>
        /// <param name="chain">The chain.</param>        
        public static bool IsDownloaded(this ConcurrentChain chain)
        {
            return chain.Tip.Header.BlockTime.ToUnixTimeSeconds() > (DateTimeOffset.Now.ToUnixTimeSeconds() - TimeSpan.FromHours(1).TotalSeconds);
        }

        /// <summary>
        /// Gets the type of the coin this chain relates to.
        /// Obviously this method and how we figure out what coin we're on needs to be revisited.
        /// </summary>
        /// <param name="chain">The chain.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">No support for this coin.</exception>
        public static CoinType GetCoinType(this ConcurrentChain chain)
        {
            uint256 genesis = chain.Genesis.Header.GetHash();

            switch (genesis.ToString())
            {
                case "000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f":
                    return CoinType.Bitcoin;
                case "b0e511e965aeb40614ca65a1b79bd6e4e7ef299fa23e575a64b079691e9d4690":
                    return CoinType.Stratis;
                case "000000000933ea01ad0ee984209779baaec3ced90fa3f408719526f8d77f4943":
                    return CoinType.Testnet;
                default:
                    throw new Exception("No support for this coin.");
            }
        }

        /// <summary>
        /// Gets the height of the first block created after this date.
        /// </summary>
        /// <param name="chain">The chain of blocks.</param>
        /// <param name="date">The date.</param>
        /// <returns>The height of the first block created after the date.</returns>
        public static int GetHeightAtTime(this ConcurrentChain chain, DateTime date)
        {
            int blockSyncStart = 0;
            int upperLimit = chain.Tip.Height;
            int lowerLimit = 0;
            bool found = false;
            while (!found)
            {
                int check = lowerLimit + (upperLimit - lowerLimit) / 2;
                if (chain.GetBlock(check).Header.BlockTime >= date)
                {
                    upperLimit = check;
                }
                else if (chain.GetBlock(check).Header.BlockTime < date)
                {
                    lowerLimit = check;
                }

                if (upperLimit - lowerLimit <= 1)
                {
                    blockSyncStart = upperLimit;
                    found = true;
                }
            }

            return blockSyncStart;
        }
    }
}