using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IO;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.DependencyInjection;
using Storage.Service.ApplicationCore.Constants;
using Storage.Service.ApplicationCore.Settings;

namespace Storage.Service.ImageResource
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private bool IsCluster()
        {
            return _configuration.GetValue<bool>("Operation:IsCluster");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationSetting>(_configuration);
            services.Configure<OidcSettings>(_configuration.GetSection(SettingKeyConstant.OidcSettingKey));

            services.AddControllers();
            services.AddCors();
            services.AddImageSharp(options =>
            {
                // You only need to set the options you want to change here
                // All properties have been listed for demonstration purposes
                // only.
                options.Configuration = Configuration.Default;
                options.MemoryStreamManager = new RecyclableMemoryStreamManager();
                options.BrowserMaxAge = TimeSpan.FromDays(7);
                options.CacheMaxAge = TimeSpan.FromDays(365);
                options.CachedNameLength = 8;
                options.OnParseCommandsAsync = _ => Task.CompletedTask;
                options.OnBeforeSaveAsync = _ => Task.CompletedTask;
                options.OnProcessedAsync = _ => Task.CompletedTask;
                options.OnPrepareResponseAsync = _ => Task.CompletedTask;
            }).Configure<PhysicalFileSystemCacheOptions>(options => { options.CacheFolder = "cache"; });

            if (IsCluster())
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                });
            }

            services.AddOpenIddict()
                .AddValidation(opt =>
                {
                    var oidcSettingSection = _configuration.GetSection(SettingKeyConstant.OidcSettingKey);
                    opt.SetIssuer(oidcSettingSection["Authority"]);
                    opt.AddAudiences(oidcSettingSection["ResourceServerName"]);
                    opt.UseSystemNetHttp();
                    opt.UseAspNetCore();
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyConstants.PolicyImageResourceRead, policy =>
                {
                    policy.AuthenticationSchemes = new string[]
                        {OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme};
                    policy.RequireClaim(OpenIddictConstants.Claims.Private.Scope,
                        new[]
                        {
                            "services.imageserver.read",
                        });
                });

                options.AddPolicy(PolicyConstants.PolicyImageResourceWrite, policy =>
                {
                    policy.AuthenticationSchemes = new string[]
                        {OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme};
                    policy.RequireClaim(OpenIddictConstants.Claims.Private.Scope,
                        new[]
                        {
                            "services.imageserver.write"
                        });
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseImageSharp();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (IsCluster())
            {
                app.Use((context, next) =>
                {
                    context.Request.Scheme = "https";
                    return next();
                });
            }

            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/error");
            app.UseRouting();

            var allowedOrigins = _configuration["AllowedHosts"].Split(";");
            app.UseCors(x =>
            {
                if (allowedOrigins.Contains("*"))
                {
                    x.AllowAnyOrigin();
                }
                else
                {
                    x.WithOrigins(allowedOrigins);
                }

                x
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(options => options.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Get}/{id?}"));
        }
    }
}