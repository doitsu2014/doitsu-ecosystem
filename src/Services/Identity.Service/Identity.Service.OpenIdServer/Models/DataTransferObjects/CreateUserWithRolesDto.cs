using System.ComponentModel.DataAnnotations;

namespace Identity.Service.OpenIdServer.Models.DataTransferObjects;

public class CreateUserWithRolesDto
{
    public string Email { get; set; }
    
    public string Avatar { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string ZipCode { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }

    public string Password { get; set; }
    public string[] Roles { get; set; }
}