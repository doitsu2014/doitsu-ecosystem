using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Storage.Service.ApplicationCore.Enums;
using Storage.Service.ImageResource.Models.Validation;

namespace Storage.Service.ImageResource.Models
{
    public class PostImageRequestModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(10 * 1024 * 1024)]
        [AllowedExtensions(".jpg", ".png")]
        public IFormFile Photo { get; set; }
    }
}