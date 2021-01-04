using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Identity.Service.OpenIdServer.ViewModels.Resource
{
    public class OpenIddictApplicationViewModel
    {
        
        public OpenIddictApplicationViewModel()
        {
        }

        public string Id { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ConsentType { get; set; }    
        public string DisplayName { get; set; }
        public HashSet<string> Permissions { get; set; }
        public HashSet<Uri> PostLogoutRedirectUris { get; set; }
        public HashSet<Uri> RedirectUris { get; set; }
        public HashSet<string> Requirements { get; set; }
        public Dictionary<string, JsonElement> Properties { get; set; }
        public string Type { get; set; }
    }
}