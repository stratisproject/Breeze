using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NTumbleBit.ClassicTumbler;

namespace Breeze.TumbleBit.Client
{
    /// <summary>
    /// An interface for managing interactions with the TumbleBit service.
    /// </summary>
    public interface ITumbleBitManager
    {
        /// <summary>
        /// Connects to the tumbler.
        /// </summary>
        /// <returns></returns>
        Task<ClassicTumblerParameters> ConnectToTumblerAsync();
    }
}
