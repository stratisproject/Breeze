using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using Stratis.Bitcoin.Notifications;

namespace Breeze.Api.Controllers
{	
	[Route("api/v{version:apiVersion}/[controller]")]
	public class NodeController : Controller
	{
		private readonly BlockNotification blockNotification;

		public NodeController(BlockNotification blockNotification)
		{
			this.blockNotification = blockNotification;
		}

	    [HttpGet]
		[Route("status")]
		public IActionResult Status()
		{
			return this.NotFound();
		}

		[HttpPost]
		[Route("sync")]
		public IActionResult Sync([FromBody] HashModel model)
		{
			if (!ModelState.IsValid)
			{
				return this.BadRequest();
			}
			this.blockNotification.SyncFrom(uint256.Parse(model.Hash));
			return this.Ok();
		}
	}

	public class HashModel
	{
		[Required(AllowEmptyStrings = false)]
		public string Hash { get; set; }
	}
}