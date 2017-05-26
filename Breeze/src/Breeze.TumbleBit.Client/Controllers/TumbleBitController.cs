using Breeze.TumbleBit.Models;
using Microsoft.AspNetCore.Mvc;

namespace Breeze.TumbleBit.Controllers
{
    /// <summary>
    /// Controller providing TumbleBit operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TumbleBitController : Controller
    {
        /// <summary>
        /// Connect to a tumbler.
        /// </summary>
        /// <param name="request">The object containing the parameters used to connect to a tumbler.</param>        
        [Route("connect")]
        [HttpPost]
        public IActionResult Connect([FromBody]TumblerConnectionRequest request)
        {
            return this.Ok();
        }
    }
}
