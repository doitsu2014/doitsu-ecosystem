using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Storage.Service.ImageResource.Models.Validation
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSize;

        public MaxFileSizeAttribute(int maxSize)
        {
            _maxSize = maxSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var file = value as IFormFile;
            if (file != null && file.Length > _maxSize)
            {
                return new ValidationResult($"Maximum allowed file size is {_maxSize} bytes.");
            }
            return ValidationResult.Success;
        }
    }
}