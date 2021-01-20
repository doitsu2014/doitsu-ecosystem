using AutoMapper;
using Identity.Service.OpenIdServer.ViewModels.Resource;
using OpenIddict.EntityFrameworkCore.Models;

namespace Identity.Service.OpenIdServer.Models
{
    public class OidcServerProfile : Profile
    {

        public OidcServerProfile()
        {
            CreateMap<OpenIddictEntityFrameworkCoreApplication, OpenIddictApplicationViewModel>()
                .ForMember(d => d.ClientId, opt => opt.MapFrom(s => s.ClientId))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.DisplayName))
                .ForMember(d => d.Scopes, opt => opt.MapFrom(s => s.Permissions));
        }
        
    }
}