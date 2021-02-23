using ACOMSaaS.NetCore.Abstractions.Model;
using FileConversion.Abstraction;
using FileConversion.Api.ModelBinders;
using FileConversion.Api.Models;
using FileConversion.Core;
using FileConversion.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.OpenApi.Models;

namespace FileConversion.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            // Config identity
            var keyVaultOptionsKey = Environment.GetEnvironmentVariable(Constants.EnvironmentKeyVaultOption) ?? "KeyVault";
            var securityOptionsKey = Environment.GetEnvironmentVariable(Constants.EnvironmentSecurityOption) ?? "Security";

            services.Configure<ApplicationOptions>(Configuration);
            services.Configure<ClientCredentialsSecurityOptions>(Configuration.GetSection(securityOptionsKey));
            services.Configure<ClientCredentialKeyVaultOptions>(Configuration.GetSection(keyVaultOptionsKey));

            services.AddAuthentication()
                .AddIdentityServerAuthentication(Security.Scheme, options =>
                {
                    options.Authority = Configuration.GetValue<string>($"{securityOptionsKey}:Authority");
                    options.ApiName = Configuration.GetValue<string>($"{securityOptionsKey}:ApiName");
                    options.RequireHttpsMetadata = false;
                });

            services.AddCors();
            services.AddMvcCore(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.ModelBinderProviders.Insert(0, new OptionModelBinderProvider());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<Startup>>();
                        var validationErrors = context.ModelState.SelectMany(x => x.Value.Errors)
                            .Select(error => error.ErrorMessage).Aggregate((x, y) => $"{x}\n{y}");
                        logger.LogDebug("Validation error(s):\n{ValidationErrors}", validationErrors);
                        return new BadRequestObjectResult(new ModelStateErrorResponse { Value = context.ModelState })
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                })
                .AddAuthorization(options =>
                {
                    options.AddPolicy(Security.PolicyFileConversion, builder =>
                    {
                        builder.RequireScope(Security.ScopeFileConversion);
                    });
                })
                .AddApiExplorer()
                .AddDataAnnotations();

            #region DI Services 
            services.AddScoped(typeof(IFileConversionEntityService<>), typeof(FileConversionEntityService<>));
            services.RegisterDefaultParserDependencies();
            #endregion

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "File Conversion Api Information", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<ApplicationOptions> options)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Conversion Api Information V1");
            });

            app.ConfigureGlobalExceptionHandler(loggerFactory.CreateLogger(GetType()));
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
