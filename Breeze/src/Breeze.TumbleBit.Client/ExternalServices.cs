using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using NTumbleBit;

namespace Breeze.TumbleBit.Client
{
    public class ExternalServices
    {
        public Transaction FundTransaction(TxOut txOut, FeeRate feeRate)
        {
            return null;
        }

        public void Track(Script scriptPubkey)
        {
        }

        public bool Broadcast(Transaction tx)
        {
            return true;
        }

        public void TrustedBroadcast(int cycleStart, TransactionType transactionType, uint correlation, TrustedBroadcastRequest broadcast)
        {
        }

        public TransactionInformation[] GetTransactions(Script scriptPubKey, bool withProof)
        {
            return null;
        }

        public FeeRate GetFeeRate()
        {
            return null;
        }
    }
}
