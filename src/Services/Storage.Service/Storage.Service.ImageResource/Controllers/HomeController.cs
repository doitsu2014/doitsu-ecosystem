using Microsoft.AspNetCore.Mvc;

namespace Storage.Service.ImageResource.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Image Server is working");
        }
        
    }
}