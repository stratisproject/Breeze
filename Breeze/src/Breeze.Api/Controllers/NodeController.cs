using Microsoft.AspNetCore.Mvc;

namespace Breeze.Api.Controllers
{
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/[controller]")]
	public class NodeController : Controller
    {			 
		[Route("connect")]
		public IActionResult Connect(string[] args)
		{
			return NotFound();
		}

		[Route("status")]
		public IActionResult Status()
		{
			return this.NotFound();
		}
	}
}
