namespace Breeze.TumbleBit.Client
{
    public enum TransactionType : int
    {
        TumblerEscrow,
        TumblerRedeem,
        /// <summary>
        /// The transaction that cashout tumbler's escrow (go to client)
        /// </summary>
        TumblerCashout,

        ClientEscrow,
        ClientRedeem,
        ClientOffer,
        ClientEscape,
        /// <summary>
        /// The transaction that cashout client's escrow (go to tumbler)
        /// </summary>
        ClientFulfill,
        ClientOfferRedeem
    }
}
