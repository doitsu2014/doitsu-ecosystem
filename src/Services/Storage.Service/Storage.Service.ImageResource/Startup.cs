using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.DependencyInjection;
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
            return _configuration.GetValue<bool>("IsCluster");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
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
            }).Configure<PhysicalFileSystemCacheOptions>(options =>
            {
                options.CacheFolder = "cache";
            });
            
            if (IsCluster())
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                });
            }
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

            var allowedOrigins = _configuration["AllowedOrigins"].Split(";");
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

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/",
                    async context => { await context.Response.WriteAsync(":v Image Resource Server :v"); });
            });
        }
    }
}