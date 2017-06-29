using System;
using System.ComponentModel.DataAnnotations;
using Breeze.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Notifications;

namespace Breeze.Api.Controllers
{	
	[Route("api/[controller]")]
	public class NodeController : Controller
	{
	    private readonly IFullNode fullNode;
	    private readonly ApiFeatureOptions apiFeatureOptions;

	    public NodeController(IFullNode fullNode, ApiFeatureOptions apiFeatureOptions)
	    {
	        this.fullNode = fullNode;
	        this.apiFeatureOptions = apiFeatureOptions;
	    }

        /// <summary>
        /// Returns some general information about the status of the underlying node.
        /// </summary>
        /// <returns></returns>
	    [HttpGet]
		[Route("status")]
		public IActionResult Status()
		{
			return this.NotFound();
		}

	    /// <summary>
	    /// Trigger a shoutdown of the current running node.
	    /// </summary>
	    /// <returns></returns>
	    [HttpPost]
	    [Route("shutdown")]
	    public IActionResult Shutdown()
	    {
            // start the node shutdown process
            this.fullNode.Stop();

	        return this.Ok();
	    }

	    /// <summary>
	    /// Set the hearbeat flag.
	    /// </summary>
	    /// <returns></returns>
	    [HttpPost]
	    [Route("heartbeat")]
	    public IActionResult Heartbeat()
	    {
	        if (this.apiFeatureOptions.HeartbeatMonitor == null)
	            return new ObjectResult("Heartbeat Disabled") {StatusCode = 405}; // (405) Method Not Allowed 

	        this.apiFeatureOptions.HeartbeatMonitor.LastBeat = DateTime.UtcNow;

	        return this.Ok();
	    }
	}
}