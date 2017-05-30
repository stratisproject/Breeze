using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NTumbleBit;
using NTumbleBit.ClassicTumbler;
using Refit;

namespace Breeze.TumbleBit.Client
{
    /// <summary>
    /// The tumbler service communicating with the tumbler server.
    /// </summary>
    public interface ITumblerService
    {
        /// <summary>
        /// Gets the tumbler's parameters.
        /// </summary>
        /// <returns></returns>
        [Get("/api/v1/tumblers/0/parameters")]
        Task<ClassicTumblerParameters> GetClassicTumblerParametersAsync();
    }
}
