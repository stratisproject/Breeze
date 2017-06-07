using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Breeze.TumbleBit.Client;
using Breeze.TumbleBit.Models;
using Stratis.Bitcoin.Common.JsonErrors;

namespace Breeze.TumbleBit.Controllers
{
    /// <summary>
    /// Controller providing TumbleBit operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TumbleBitController : Controller
    {
        private readonly ITumbleBitManager tumbleBitManager;

        public TumbleBitController(ITumbleBitManager tumbleBitManager)
        {
            this.tumbleBitManager = tumbleBitManager;
        }

        /// <summary>
        /// Connect to a tumbler.
        /// </summary>
        [Route("connect")]
        [HttpPost]
        public async Task<IActionResult> ConnectAsync([FromBody] TumblerConnectionRequest request)
        {
            // checks the request is valid
            if (!this.ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Formatting error", string.Join(Environment.NewLine, errors));
            }

            try
            {
                var tumblerParameters = await this.tumbleBitManager.ConnectToTumblerAsync(request.ServerAddress);
                return this.Json(tumblerParameters);
            }
            catch (Exception e)
            {                
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, $"An error occured connecting to the tumbler with uri {request.ServerAddress}.", e.ToString());
            }
        }

        /// <summary>
        /// Connect to a tumbler.
        /// </summary>
        [Route("tumble")]
        [HttpPost]
        public async Task<IActionResult> TumbleAsync([FromBody] TumbleRequest request)
        {
            // checks the request is valid
            if (!this.ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Formatting error", string.Join(Environment.NewLine, errors));
            }

            try
            {
                await this.tumbleBitManager.TumbleAsync(request.DestinationWalletName);
                return this.Ok();
            }
            catch (Exception e)
            {
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "An error occured starting tumbling session.", e.ToString());
            }
        }
    }
}
