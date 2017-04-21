using NBitcoin;
using Stratis.Bitcoin;
using Breeze.Wallet.Wrappers;

namespace Breeze.Wallet.Notifications
{
    /// <summary>
    /// Observer that receives notifications about the arrival of new <see cref="Transaction"/>s.
    /// </summary>
	public class TransactionObserver : SignalObserver<Transaction>
	{
	    private readonly ITrackerWrapper trackerWrapper;

	    public TransactionObserver(ITrackerWrapper trackerWrapper)
	    {
	        this.trackerWrapper = trackerWrapper;
	    }
        
        /// <summary>
        /// Manages what happens when a new transaction is received.
        /// </summary>
        /// <param name="transaction">The new transaction</param>
	    protected override void OnNextCore(Transaction transaction)
	    {            
            this.trackerWrapper.NotifyAboutTransaction(transaction);	        
	    }
	}
}
