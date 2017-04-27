using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HBitcoin.Models;
using NBitcoin;

namespace HBitcoin.Models
{
    public class SmartTransaction: IEquatable<SmartTransaction>
	{
		#region Members
		
		public Height Height { get; }
		public Transaction Transaction { get; }

		public bool Confirmed => Height.Type == HeightType.Chain;
		public uint256 GetHash() => Transaction.GetHash();

		#endregion

		#region Constructors

		public SmartTransaction()
		{

		}
		
		public SmartTransaction(Transaction transaction, Height height)
		{
			Height = height;
			Transaction = transaction;
		}

		#endregion

		#region Equality

		public bool Equals(SmartTransaction other) => GetHash().Equals(other.GetHash());
		public bool Equals(Transaction other) => GetHash().Equals(other.GetHash());

		public override bool Equals(object obj)
		{
			bool rc = false;
			if (obj is SmartTransaction)
			{
				var transaction = (SmartTransaction)obj;
				rc = GetHash().Equals(transaction.GetHash());
			}
			else if (obj is Transaction)
			{
				var transaction = (Transaction)obj;
				rc = GetHash().Equals(transaction.GetHash());
			}
			return rc;
		}

		public override int GetHashCode()
		{
			return GetHash().GetHashCode();
		}

		public static bool operator !=(SmartTransaction tx1, SmartTransaction tx2)
		{
			return !(tx1 == tx2);
		}
		public static bool operator ==(SmartTransaction tx1, SmartTransaction tx2)
		{
			bool rc;

			if(ReferenceEquals(tx1, tx2)) rc = true;

			else if((object) tx1 == null || (object) tx2 == null)
			{
				rc = false;
			}
			else
			{
				rc = tx1.GetHash().Equals(tx2.GetHash());
			}

			return rc;
		}
		public static bool operator ==(Transaction tx1, SmartTransaction tx2)
		{
			bool rc;

			if ((object)tx1 == null || (object)tx2 == null)
			{
				rc = false;
			}
			else
			{
				rc = tx1.GetHash().Equals(tx2.GetHash());
			}

			return rc;
		}

		public static bool operator !=(Transaction tx1, SmartTransaction tx2)
		{
			return !(tx1 == tx2);
		}

		public static bool operator ==(SmartTransaction tx1, Transaction tx2)
		{
			bool rc;

			if ((object)tx1 == null || (object)tx2 == null)
			{
				rc = false;
			}
			else
			{
				rc = tx1.GetHash().Equals(tx2.GetHash());
			}

			return rc;
		}

		public static bool operator !=(SmartTransaction tx1, Transaction tx2)
		{
			return !(tx1 == tx2);
		}
		
		#endregion
	}
}
