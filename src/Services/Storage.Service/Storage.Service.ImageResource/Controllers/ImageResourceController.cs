using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using Storage.Service.ApplicationCore.Enums;
using Storage.Service.ImageResource.Models;

namespace Storage.Service.ImageResource.Controllers
{
    [Route("api/image-resource")]
    public class ImageResourceController : Controller
    {
        private readonly string PublicPath = "public";

        private readonly ILogger<ImageResourceController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public ImageResourceController(ILogger<ImageResourceController> logger,
            IWebHostEnvironment hostingEnvironment,
            IConfiguration configuration)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize(Policy = "ImageResourceWrite")]
        public async Task<ActionResult> PostFile([FromForm] PostImageRequestModel request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    var subject = HttpContext.User.GetClaim(OpenIddictConstants.Claims.Subject) ?? string.Empty;
                    var uploadLocalPath = Path.Combine(webRootPath, PublicPath, subject);
                    var fileName =
                        $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Photo.FileName)}";
                    
                    if (request.Photo.Length > 0)
                    {
                        if (!Directory.Exists(uploadLocalPath))
                        {
                            Directory.CreateDirectory(uploadLocalPath);
                        }

                        await using var fileStream =
                            new FileStream(Path.Combine(uploadLocalPath, fileName), FileMode.Create);
                        await request.Photo.CopyToAsync(fileStream);
                    }

                    var requestHost = Request.Host;
                    var port = requestHost.Port.GetValueOrDefault();
                    var url = new UriBuilder
                    {
                        Host = requestHost.Host,
                        Port = port != 0 ? port : -1,
                        Scheme = Request.Scheme,
                        Path = Path.Combine(PublicPath, subject, fileName)
                    };

                    var imageInformation = new PostImageResultModel()
                    {
                        Subject = subject,
                        OriginalFileName = request.Photo.FileName,
                        Url = url.ToString(),
                        Status = PostImageResultStatusEnum.Done 
                    };

                    return Ok(imageInformation);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{Exception}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}