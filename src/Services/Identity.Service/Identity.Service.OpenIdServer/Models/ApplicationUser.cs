using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Identity.Service.OpenIdServer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(256)]
        public string Street { get; set; }
        [MaxLength(64)]
        public string City { get; set; }
        [MaxLength(32)]
        public string State { get; set; }
        [MaxLength(32)]
        public string Country { get; set; }
        [MaxLength(32)]
        public string ZipCode { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        [MaxLength(256)]
        public string LastName { get; set; }
    } 
}
