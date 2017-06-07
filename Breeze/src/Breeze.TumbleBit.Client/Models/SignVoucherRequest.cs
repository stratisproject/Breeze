using NBitcoin;
using NTumbleBit.ClassicTumbler;

namespace Breeze.TumbleBit.Models
{
	public class SignVoucherRequest
	{
		public int KeyReference { get; set; }

		public PubKey TumblerEscrowPubKey { get; set; }

        public ClientEscrowInformation ClientEscrowInformation { get; set; }

        public MerkleBlock MerkleProof { get; set; }

        public Transaction Transaction { get; set; }
    }
}
