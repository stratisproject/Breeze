using System;
using System.Threading.Tasks;
using NBitcoin;

namespace Breeze.Wallet
{
    public interface ITracker
    {
        /// <summary>
        /// Initializes the tracker.
        /// </summary>
        /// <returns></returns>
        Task Initialize();

        /// <summary>
        /// Waits for the chain to download.
        /// </summary>
        /// <returns></returns>
        Task WaitForChainDownloadAsync();

        /// <summary>
        /// Synchronize the wallet starting from the date passed as a parameter.
        /// </summary>
        /// <param name="date">The date from which to start the sync process.</param>
        /// <returns></returns>
        void SyncFrom(DateTime date);
    }
}
