using System.IO;
using System.Linq;
using HBitcoin.KeyManagement;
using NBitcoin;
using Breeze.Api.Models;

namespace Breeze.Api.Wrappers
{
	/// <summary>
	/// An implementation of the <see cref="ISafeWrapper"/> interface.
	/// </summary>
	public class SafeWrapper : ISafeWrapper
	{
		/// <summary>
		/// Creates a safe on the local device.
		/// </summary>
		/// <param name="password">The user's password.</param>
		/// <param name="folderPath">The folder where the safe will be saved.</param>
		/// <param name="name">The name of the safe.</param>
		/// <param name="network">The network for which to create a safe.</param>
		/// <returns>A mnemonic allowing recovery of the safe.</returns>
		public string Create(string password, string folderPath, string name, string network)
		{			
			Mnemonic mnemonic;
			Safe safe = Safe.Create(out mnemonic, password, Path.Combine(folderPath, $"{name}.json"), this.GetNetwork(network));
			return mnemonic.ToString();
		}

		/// <summary>
		/// Loads a safe from the local device.
		/// </summary>
		/// <param name="password">The user's password.</param>
		/// <param name="folderPath">The folder where the safe will be loaded.</param>
		/// <param name="name">The name of the safe.</param>
		/// <returns>The safe loaded from the local device</returns>
		public SafeModel Load(string password, string folderPath, string name)
		{
			Safe safe = Safe.Load(password, Path.Combine(folderPath, $"{name}.json"));

			//TODO review here which data should be returned
			return new SafeModel
			{
				Network = safe.Network.Name,
				Addresses = safe.GetFirstNAddresses(10).Select(a => a.ToWif()),
				FileName = safe.WalletFilePath
			};
		}

		/// <summary>
		/// Recovers a safe from the local device.
		/// </summary>
		/// <param name="password">The user's password.</param>
		/// <param name="folderPath">The folder where the safe will be loaded.</param>
		/// <param name="name">The name of the safe.</param>
		/// <param name="network">The network in which to creae this wallet</param>
		/// <param name="mnemonic">The user's mnemonic for the safe.</param>		
		/// <returns></returns>
		public SafeModel Recover(string password, string folderPath, string name, string network, string mnemonic)
		{
			Safe safe = Safe.Recover(new Mnemonic(mnemonic), password, Path.Combine(folderPath, $"{name}.json"), this.GetNetwork(network));

			//TODO review here which data should be returned
			return new SafeModel
			{
				Network = safe.Network.Name,
				Addresses = safe.GetFirstNAddresses(10).Select(a => a.ToWif()),
				FileName = safe.WalletFilePath
			};
		}

		private Network GetNetwork(string network)
		{
			// any network different than MainNet will default to TestNet			
			switch (network.ToLowerInvariant())
			{
				case "main":
				case "mainnet":
					return Network.Main;					
				default:
					return Network.TestNet;
			}
		}
	}
}
