using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTumbleBit.ClassicTumbler
{
    public class OpenChannelRequest
    {
		public PubKey EscrowKey
		{
			get; set;
		}
		public byte[] Signature
		{
			get; set;
		}
		public uint160 Nonce
		{
			get; set;
		}
		public int CycleStart
		{
			get; set;
		}
	}
}
