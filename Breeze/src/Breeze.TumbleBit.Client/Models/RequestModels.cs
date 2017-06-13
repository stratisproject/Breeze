using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Breeze.TumbleBit.Models
{
    /// <summary>
    /// Base class for request objects received to the controllers
    /// </summary>
    public class RequestModel
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    /// <summary>
    /// Object used to connect to a tumbler.
    /// </summary>
    public class TumblerConnectionRequest : RequestModel
    {
        [Required(ErrorMessage = "A server address is required.")]
        public Uri ServerAddress { get; set; }

        public string Network { get; set; }
    }

    public class TumbleRequest
    {
        [Required(ErrorMessage = "The name of the origin wallet is required.")]
        public string OriginWalletName { get; set; }

        [Required(ErrorMessage = "The name of the destination wallet is required.")]
        public string DestinationWalletName { get; set; }
    }
}
