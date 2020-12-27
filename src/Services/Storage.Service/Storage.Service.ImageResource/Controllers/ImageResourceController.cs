using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Storage.Service.ImageResource.Controllers
{
    [Route("api/image-resource")]
    public class ImageResourceController : Controller
    {
        [HttpGet]
        [Authorize(Policy = "ImageResourceRead")] 
        public IActionResult Get()
        {
            var claims = JsonSerializer.Serialize(HttpContext.User.Claims);

            return Ok(new
            {
                Message = "Get Image Resource Successfully!",
                Data = claims
            });
        }
    }
}