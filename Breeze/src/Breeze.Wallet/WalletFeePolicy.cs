using System;
using NBitcoin;
using Stratis.Bitcoin.MemoryPool;
using Stratis.Bitcoin.MemoryPool.Fee;
using Stratis.Bitcoin.Wallet;

namespace Breeze.Wallet
{

    public class LightWalletFeePolicy : IWalletFeePolicy
    {
        private readonly BlockPolicyEstimator blockPolicyEstimator;

        private readonly Money maxTxFee;

        /// <summary>
        ///  Fees smaller than this (in satoshi) are considered zero fee (for transaction creation)
        ///  Override with -mintxfee
        /// </summary>
        private readonly FeeRate minTxFee;

        /// <summary>
        ///  If fee estimation does not have enough data to provide estimates, use this fee instead.
        ///  Has no effect if not using fee estimation
        ///  Override with -fallbackfee
        /// </summary>
        private readonly FeeRate fallbackFee;

        /// <summary>
        /// Transaction fee set by the user
        /// </summary>
        private readonly FeeRate payTxFee;

        public LightWalletFeePolicy(BlockPolicyEstimator blockPolicyEstimator)
        {
            this.blockPolicyEstimator = blockPolicyEstimator;

            this.minTxFee = new FeeRate(1000);
            this.fallbackFee = new FeeRate(20000);
            this.payTxFee = new FeeRate(0);
            this.maxTxFee = new Money(0.1M, MoneyUnit.BTC);
        }

        public Money GetRequiredFee(int txBytes)
        {
            return Math.Max(this.minTxFee.GetFee(txBytes), MempoolValidator.MinRelayTxFee.GetFee(txBytes));
        }

        public Money GetMinimumFee(int txBytes, int confirmTarget)
        {
            // payTxFee is the user-set global for desired feerate
            return this.GetMinimumFee(txBytes, confirmTarget, this.payTxFee.GetFee(txBytes));
        }

        public Money GetMinimumFee(int txBytes, int confirmTarget, Money targetFee)
        {
            Money nFeeNeeded = targetFee;
            // User didn't set: use -txconfirmtarget to estimate...
            if (nFeeNeeded == 0)
            {
                int estimateFoundTarget = confirmTarget;
                nFeeNeeded = this.blockPolicyEstimator.EstimateSmartFee(confirmTarget, null, out estimateFoundTarget).GetFee(txBytes);
                // ... unless we don't have enough mempool data for estimatefee, then use fallbackFee
                if (nFeeNeeded == 0)
                    nFeeNeeded = this.fallbackFee.GetFee(txBytes);
            }
            // prevent user from paying a fee below minRelayTxFee or minTxFee
            nFeeNeeded = Math.Max(nFeeNeeded, this.GetRequiredFee(txBytes));
            // But always obey the maximum
            if (nFeeNeeded > this.maxTxFee)
                nFeeNeeded = this.maxTxFee;
            return nFeeNeeded;
        }
    }
}
