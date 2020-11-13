using System.ComponentModel.DataAnnotations;

namespace Identity.Service.OpenIdServer.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
