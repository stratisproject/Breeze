using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Breeze.Common.JsonErrors;
using Microsoft.AspNetCore.Mvc;
using Breeze.TumbleBit.Client;

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
        [HttpGet]
        public async Task<IActionResult> ConnectAsync()
        {
            // checks the request is valid
            if (!this.ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Formatting error", string.Join(Environment.NewLine, errors));
            }

            try
            {
                var tumblerParameters = await this.tumbleBitManager.ConnectToTumblerAsync();
                return this.Json(tumblerParameters);
            }
            catch (Exception e)
            {                
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, $"An error occured connecting to the tumbler.", e.ToString());
            }
        }
    }
}
