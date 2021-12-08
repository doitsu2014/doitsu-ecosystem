using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using Identity.Service.OpenIdServer.Models.DataTransferObjects;
using Identity.Service.OpenIdServer.Models.Entities;
using Identity.Service.OpenIdServer.Settings;
using Identity.Service.OpenIdServer.ViewModels.Resource;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Identity.Service.OpenIdServer.Models
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<string, Dictionary<string, JsonElement>>()
                .ConvertUsing(s => s == null ? null : JsonConvert.DeserializeObject<Dictionary<string, JsonElement>>(s));

            CreateMap<Dictionary<string, JsonElement>, string>()
                .ConvertUsing(s => s == null ? null : JsonConvert.SerializeObject(s));

            CreateMap<string, HashSet<Uri>>()
                .ConvertUsing(s => s == null ? null : JsonConvert.DeserializeObject<HashSet<Uri>>(s));

            CreateMap<HashSet<Uri>, string>()
                .ConvertUsing(s => s == null ? null : JsonConvert.SerializeObject(s));

            CreateMap<string, HashSet<string>>()
                .ConvertUsing(s => s == null ? null : JsonConvert.DeserializeObject<HashSet<string>>(s));

            CreateMap<HashSet<string>, string>()
                .ConvertUsing(s => s == null ? null : JsonConvert.SerializeObject(s));

            CreateMap<OpenIddictEntityFrameworkCoreApplication, OpenIddictApplicationViewModel>();
            CreateMap<OpenIddictApplicationViewModel, OpenIddictEntityFrameworkCoreApplication>();

            CreateMap<OpenIddictEntityFrameworkCoreApplication, OpenIddictApplicationDescriptor>();
            CreateMap<OpenIddictApplicationDescriptor, OpenIddictEntityFrameworkCoreApplication>();

            CreateMap<CreateUserWithRolesDto, ApplicationUser>()
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.NormalizedEmail, opt => opt.MapFrom(s => s.Email.ToLower()))
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.NormalizedUserName, opt => opt.MapFrom(s => s.Email.ToLower()));

            CreateMap<InitialSetting.InitialApplication, OpenIddictApplicationDescriptor>();
        }
    }
}