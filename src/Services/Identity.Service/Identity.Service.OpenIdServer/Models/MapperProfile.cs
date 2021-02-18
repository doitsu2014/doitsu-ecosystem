using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using Identity.Service.OpenIdServer.ViewModels.Resource;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        }
    }
}