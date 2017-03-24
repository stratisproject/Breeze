using NBitcoin;
using HBitcoin.FullBlockSpv;
using HBitcoin.Models;
using System;

namespace Breeze.Wallet.Wrappers
{
    public class TrackerWrapper : ITrackerWrapper
    {
        private readonly Tracker tracker;

        public TrackerWrapper()
        {
            this.tracker = new Tracker();              
        }

		/// <summary>
		/// Get the hash of the last block that has been succesfully processed.
		/// </summary>
		/// <returns>The hash of the block</returns>
		public uint256 GetLastProcessedBlock()
		{
			// TODO use Tracker.BestHeight. Genesis hash for now.
			return uint256.Parse("000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f");
		}

		public void NotifyAboutBlock(int height, Block block)
        {
            this.tracker.AddOrReplaceBlock(new Height(height), block);
			Console.WriteLine($"height: {height}, block hash: {block.Header.GetHash()}");
        }
    }
}
