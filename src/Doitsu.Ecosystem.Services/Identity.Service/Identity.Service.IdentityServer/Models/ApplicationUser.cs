using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Identity.Service.IdentityServer.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(256)]
        public string Street { get; set; }
        [Required]
        [MaxLength(64)]
        public string City { get; set; }
        [Required]
        [MaxLength(32)]
        public string State { get; set; }
        [Required]
        [MaxLength(32)]
        public string Country { get; set; }
        [Required]
        [MaxLength(32)]
        public string ZipCode { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        [Required]
        [MaxLength(256)]
        public string LastName { get; set; }
    }
}