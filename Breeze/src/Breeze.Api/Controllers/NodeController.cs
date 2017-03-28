using Microsoft.AspNetCore.Mvc;

namespace Breeze.Api.Controllers
{
	
	[Route("api/v{version:apiVersion}/[controller]")]
	public class NodeController : Controller
    {
		[HttpGet]
		[Route("status")]
		public IActionResult Status()
		{
			return this.NotFound();
		}
	}
}