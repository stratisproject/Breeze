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
    }
}
