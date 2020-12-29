using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Storage.Service.ImageResource.Models.Validation
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;

        public AllowedExtensionsAttribute(params string[] allowedExtensions)
        {
            _allowedExtensions = allowedExtensions;
        }
        
        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_allowedExtensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage(extension));
                }
            }
        
            return ValidationResult.Success;
        }

        private string GetErrorMessage(string extension)
        {
            return $"This photo extension is {extension} not allowed! You should upload file with the allowed extension, that is matching in [{_allowedExtensions.Aggregate((a,b) => $"{a}, {b}")}]";
        }
        
    }
}