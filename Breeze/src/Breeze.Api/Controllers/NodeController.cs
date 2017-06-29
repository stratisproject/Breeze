using System.ComponentModel.DataAnnotations;
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

	    public NodeController(IFullNode fullNode)
	    {
	        this.fullNode = fullNode;
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
    }

}