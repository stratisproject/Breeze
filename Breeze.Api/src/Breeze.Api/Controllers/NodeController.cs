using Microsoft.AspNetCore.Mvc;

namespace Breeze.Api.Controllers
{
	[Route("api/[controller]")]
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
