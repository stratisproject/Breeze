using NBitcoin;

namespace Breeze.TumbleBit.Models
{
    public class TumblerEscrowKeyResponse
    {
        public int KeyIndex { get; set; }

        public PubKey PubKey { get; set; }
    }
}
