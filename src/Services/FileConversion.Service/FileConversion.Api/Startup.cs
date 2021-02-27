using FileConversion.Abstraction;
using FileConversion.Api.ModelBinders;
using FileConversion.Api.Models;
using FileConversion.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;
using FileConversion.Core.Interface;
using FileConversion.Infrastructure;
using FileConversion.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using Shared.Abstraction.Settings;

namespace FileConversion.Api
{
    public class Startup
    {
        private const string OIDC_SETTINGS = "Oidc";
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _oidcSection;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _oidcSection = _configuration.GetSection(OIDC_SETTINGS);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Config identity
            var keyVaultOptionsKey =
                Environment.GetEnvironmentVariable(Constants.EnvironmentKeyVaultOption) ?? "KeyVault";
            var securityOptionsKey =
                Environment.GetEnvironmentVariable(Constants.EnvironmentSecurityOption) ?? "Security";

            services.Configure<ApplicationOptions>(_configuration);
            services.Configure<OidcSettings>(_oidcSection);
            
            services.AddDbContext<FileConversionContext>(options =>
            {
                // Configure the context to use Microsoft SQL Server.
                options.UseNpgsql(_configuration.GetConnectionString(Constants.ConnectionStringKey), options =>
                    options.MigrationsAssembly(typeof(FileConversionContext).GetTypeInfo().Assembly.FullName)
                        .EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null));
            });

            services.AddOpenIddict()
                .AddValidation(opt =>
                {
                    opt.SetIssuer(_oidcSection["Authority"]);
                    opt.AddAudiences(_oidcSection["ResourceServerName"]);
                    opt.UseSystemNetHttp();
                    opt.UseAspNetCore();
                });

            services.AddCors();
            services.AddControllers()
                .AddNewtonsoftJson();
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyConstants.PolicyFileConversionRead, policy =>
                {
                    policy.AuthenticationSchemes = new string[]
                        {OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme};
                    policy.RequireClaim(OpenIddictConstants.Claims.Private.Scope,
                        new[]
                        {
                            PolicyConstants.ScopeFileConversionAll,
                            PolicyConstants.ScopeFileConversionRead
                        });
                });

                options.AddPolicy(PolicyConstants.PolicyFileConversionParse, policy =>
                {
                    policy.AuthenticationSchemes = new string[]
                        {OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme};
                    policy.RequireClaim(OpenIddictConstants.Claims.Private.Scope,
                        new[]
                        {
                            PolicyConstants.ScopeFileConversionAll,
                            PolicyConstants.ScopeFileConversionParse
                        });
                });

                options.AddPolicy(PolicyConstants.PolicyFileConversionWrite, policy =>
                {
                    policy.AuthenticationSchemes = new string[]
                        {OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme};

                    policy.RequireRole("admin");

                    policy.RequireClaim(OpenIddictConstants.Claims.Private.Scope,
                        new[]
                        {
                            PolicyConstants.ScopeFileConversionAll,
                            PolicyConstants.ScopeFileConversionWrite
                        });
                });
            });

            #region DI Services

            services.AddScoped(typeof(IFileConversionRepository<>), typeof(FileConversionRepository<>));
            services.RegisterDefaultParserDependencies();

            #endregion

            // Register the Swagger generator, defining 1 or more Swagger documents
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo {Title = "File Conversion Api Information", Version = "v1"});
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IOptions<ApplicationOptions> options)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            // app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            // app.UseSwaggerUI(c =>
            // {
            //     c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Conversion Api Information V1");
            // });

            app.ConfigureGlobalExceptionHandler(loggerFactory.CreateLogger(GetType()));
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=InputMapping}/{action=GetAllInputMappings}/{id?}");
            });
        }
    }
}