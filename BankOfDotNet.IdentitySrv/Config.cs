using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace BankOfDotNet.IdentitySrv
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser{
                SubjectId = "1",
                Username = "asif",
                Password = "password",
                },
                new TestUser{
                SubjectId = "2",
                Username = "raza",
                Password = "password"
                }
            };
        }
        public static IEnumerable<ApiResource> GetAllApiResources() {
            return new List<ApiResource> { 
                //new ApiResource("bankOfDotNetApi", "Customer API")
                 new ApiResource()
                {
                    Name = "bankOfDotNetApi",   //This is the name of the API
                    Description = "Customer API",
                    Enabled = true,
                    DisplayName = "Customer API Service",
                    Scopes = new List<string> { "bankOfDotNetApi" },
                }
        };
        }
        public static IEnumerable<Client> GetClients() {
            return new List<Client> {
                new Client{
                    ClientId="client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {
                    new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "bankOfDotNetApi"}
                },
                new Client{
                    ClientId="ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = {
                    new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "bankOfDotNetApi"}
                },
                new Client{
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    //ClientSecrets =  new List<Secret> {
                    //    new Secret("secret".Sha256())
                    //},
                    RequirePkce = false,
                    AllowRememberConsent = false,

                    //AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    
                    //ClientUri= "https://localhost:5003/home/secure",
                    RedirectUris = new List<string>() { "https://localhost:5003/signin-oidc"},
                    PostLogoutRedirectUris = new List<string>() { "https://localhost:5003/signout-callback-oidc" },
                    //AllowedCorsOrigins =     { "https://localhost:5003" },
                    AllowedScopes = new List<string>{
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
                new Client{
                    ClientId = "swaggerapiui",
                    ClientName = "Swagger API Client",
                    RequirePkce = false,
                    AllowRememberConsent = false,

                    AllowedGrantTypes = GrantTypes.Implicit,
                    
                    RedirectUris = new List<string>() { "http://localhost:52797/swagger/oauth2-redirect.html"},
                    PostLogoutRedirectUris = new List<string>() { "http://localhost:52797/swagger" },

                    AllowedScopes = { "bankOfDotNetApi"},
                    AllowAccessTokensViaBrowser = true
                }
                /*,
                new Client
                   {
                       ClientId = "movies_mvc_client",
                       ClientName = "Movies MVC Web App",
                       AllowedGrantTypes = GrantTypes.Hybrid,
                       RequirePkce = false,
                       AllowRememberConsent = false,
                       RedirectUris = new List<string>()
                       {
                           "https://localhost:5003/signin-oidc"
                       },
                       PostLogoutRedirectUris = new List<string>()
                       {
                           "https://localhost:5003/signout-callback-oidc"
                       },
                       ClientSecrets = new List<Secret>
                       {
                           new Secret("secret".Sha256())
                       },
                       AllowedScopes = new List<string>
                       {
                           IdentityServerConstants.StandardScopes.OpenId,
                           IdentityServerConstants.StandardScopes.Profile,
                           //IdentityServerConstants.StandardScopes.Address,
                           //IdentityServerConstants.StandardScopes.Email,
                           //"movieAPI",
                           //"roles"
                       }
                   }*/
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
             {
                 new ApiScope(name: "bankOfDotNetApi")
             };
        }
    }
}