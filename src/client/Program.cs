using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace client
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            var discovery = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (discovery.IsError)
            {
                Console.WriteLine(discovery.Error);
                return;
            }

            var request = new ClientCredentialsTokenRequest()
            {
                Address = discovery.TokenEndpoint,
                ClientId = "restClient",
                ClientSecret = "secret",
                Scope = "SearchApi",
            };

            var userPwdRequest = new PasswordTokenRequest()
            {
                Address = discovery.TokenEndpoint,
                ClientId = "ApiClient",
                ClientSecret = "secret",

                UserName = "john",
                Password = "password",
                Scope = "SearchApi",
            };

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(request);
            // var tokenResponse = await client.RequestPasswordTokenAsync(userPwdRequest);
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}
