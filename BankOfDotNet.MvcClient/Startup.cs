using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;

namespace BankOfDotNet.MvcClient
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
            services.AddControllersWithViews();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            IdentityModelEventSource.ShowPII = true;
            services.AddAuthentication(option =>
            {
                option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, option =>
            {
                option.Authority = "https://localhost:5000";
                option.ClientId = "mvc";
                //option.ClientSecret = "secret";
                option.ResponseType = "code id_token";

                //option.SignInScheme = "Cookies";

                //option.RequireHttpsMetadata = false;

                option.SaveTokens = true;
                option.GetClaimsFromUserInfoEndpoint = true;
            });
            
            /*
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
             .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
             .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
             {
                 options.Authority = "https://localhost:5000";

                 options.ClientId = "movies_mvc_client";
                 options.ClientSecret = "secret";
                 options.ResponseType = "code id_token";
                 //options.RequireHttpsMetadata = false;
                 options.Scope.Add("openid");
                 options.Scope.Add("profile");
                 //   options.Scope.Add("address");
                 //options.Scope.Add("email");
                 //options.Scope.Add("roles");

                 options.ClaimActions.DeleteClaim("sid");
                 options.ClaimActions.DeleteClaim("idp");
                 options.ClaimActions.DeleteClaim("s_hash");
                 options.ClaimActions.DeleteClaim("auth_time");
                 options.ClaimActions.MapUniqueJsonKey("role", "role");

                 //options.Scope.Add("movieAPI");

                 options.SaveTokens = true;
                 options.GetClaimsFromUserInfoEndpoint = true;

                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     //NameClaimType = JwtClaimTypes.GivenName,
                     //RoleClaimType = JwtClaimTypes.Role
                 };
             });*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
