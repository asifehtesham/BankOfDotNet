using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using IdentityServer4.EntityFramework;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace BankOfDotNet.IdentitySrv
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public Startup(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IConfiguration configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;
            //option=>option.EnableEndpointRouting = false
            services.AddMvc();

            var config = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appSettings.json", false)
                                .Build();

            //string connectionString = config.GetSection("ConnectionString").Value;
            string connectionString = configuration.GetConnectionString("DBConnection");

            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
             
            services.AddIdentityServer()
                .AddTestUsers(Config.GetUsers())
                .AddDeveloperSigningCredential()
                //.AddInMemoryIdentityResources(Config.GetIdentityResources())
                //.AddInMemoryApiResources(Config.GetAllApiResources())
                //.AddInMemoryClients(Config.GetClients())
                .AddInMemoryApiScopes(Config.GetApiScopes())
                .AddConfigurationStore(option => {
                    option.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly)); 
                })
                .AddOperationalStore(option => {
                    option.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                });

            services.AddDbContext<PersistedGrantDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DBConnection")));

            services.AddDbContext<ConfigurationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DBConnection")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeIdentityServerDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
            app.UseStaticFiles();

            
            app.UseHttpsRedirection();

            //app.UseMvcWithDefaultRoute();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            
        }

        public void InitializeIdentityServerDatabase(IApplicationBuilder app) {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope()) {

                //serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                //context.Database.Migrate();

                /*foreach (var client in Config.GetClients().Where(c=> c.ClientId.Contains("swaggerapiui")))
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();*/

                if (!context.Clients.Any()) {
                    foreach (var client in Config.GetClients()) {
                        context.Clients.Add(client.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var scope in Config.GetApiScopes())
                    {
                        context.ApiScopes.Add(scope.ToEntity());
                    }

                    context.SaveChanges();
                }
                
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetAllApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}
