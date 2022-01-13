using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BankOfDotNet.ConsoleClient
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync() {

            var discoRO = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (discoRO.IsError)
            {
                Console.WriteLine(discoRO.Error);
                return;
            }

            var tokenClientRO = new TokenClient(discoRO.TokenEndpoint, "ro.client", "secret");
            var tokenResponseRO = await tokenClientRO.RequestResourceOwnerPasswordAsync("asif", "password", "bankOfDotNetApi");

            if (tokenResponseRO.IsError)
            {
                Console.WriteLine(tokenResponseRO.Error);
                return;
            }


            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("bankOfDotNetApi");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var customerInfo = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    Id = 13,
                    FirstName = "Sarah",
                    LastName = "Asif"

                }), Encoding.UTF8, "application/json"
                );
            var createCustomerResponse = await client.PostAsync("http://localhost:52797/api/customers", customerInfo);
            if (!createCustomerResponse.IsSuccessStatusCode) {
                Console.Write(createCustomerResponse.StatusCode);
            }

            var getCustomerResponse = await client.GetAsync("http://localhost:52797/api/customers");

                if (!getCustomerResponse.IsSuccessStatusCode)
            {
                Console.Write(getCustomerResponse.StatusCode);
            }
            else {
                var content  = await getCustomerResponse.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));

            }

            Console.Read();
        }
    }
}
