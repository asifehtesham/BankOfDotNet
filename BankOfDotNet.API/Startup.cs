using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankOfDotNet.API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;

namespace BankOfDotNet.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("Bearer").AddIdentityServerAuthentication(options => {
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;
                options.ApiName = "bankOfDotNetApi";

            });

            services.AddDbContext<BankContext>(opts => opts.UseInMemoryDatabase("BankingDb"));
            services.AddControllers();

            services.AddSwaggerGen(option => {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "BankofNet API", Version = "v1" });
                option.OperationFilter<CheckAuthorizationOperationFilter>();

                option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {

                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://localhost:5000/connect/authorize"),
                            TokenUrl = new Uri("https://localhost:5000/oauth2/v2.0/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "bankOfDotNetApi", "BankOfDotNet API" }
                            }
                        }
                        /*ClientCredentials = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri("https://localhost:5000/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "bankOfDotNetApi", "NameOfYourApiName" }
                            }
                        }*/
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint("/swagger/v1/swagger.json", "BankofDotNet API V1");
                option.OAuthClientId("swaggerapiui");
                option.OAuthAppName("Swagger API UI");
            });
        }
    }

    internal class CheckAuthorizationOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context) {

            var authorizeAtrributeExists = context.ApiDescription.CustomAttributes().OfType<AuthorizeAttribute>().Any();
                

            if (authorizeAtrributeExists) {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                var req = new OpenApiSecurityRequirement();
                req.Add(

                     new OpenApiSecurityScheme()
                     {
                         Reference = new OpenApiReference
                         {
                             Type = ReferenceType.SecurityScheme,
                             Id = "oauth2"
                         },
                         Scheme = "oauth2",
                         Name = "oauth2",
                         In = ParameterLocation.Header
                     },
                    new List<string>() { "bankOfDotNetApi" }
                );

                operation.Security = new List<OpenApiSecurityRequirement>();
                operation.Security.Add(req);
                
            }
        }
    }
}
