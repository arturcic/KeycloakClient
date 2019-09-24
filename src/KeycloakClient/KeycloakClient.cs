using System.Net.Http;
using KeycloakClient.Extensions;
using KeycloakClient.Internals;
using KeycloakClient.Realm;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KeycloakClient
{
    public interface IKeycloakClient
    {
        IRealmsResource Realms();
    }

    public class KeycloakClient : IKeycloakClient
    {
        private HttpClient HttpClient { get; }

        static KeycloakClient()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
        }

        public KeycloakClient(HttpClient httpClient, IOptions<KeycloakAdminClientOptions> clientOptions)
        {
            var options = clientOptions.Value;

            httpClient.BaseAddress = options.Url.Combine("/");
            HttpClient = httpClient;
        }

        public KeycloakClient(KeycloakAdminClientOptions options) :
            this(Options.Create(options))
        {
        }

        private KeycloakClient(IOptions<KeycloakAdminClientOptions> options) :
            this(new HttpClient(new KeycloakRefreshTokenDelegatingHandler(options)), options)
        {
        }

        public IRealmsResource Realms() => RestClient.For<IRealmsResource>(HttpClient, "admin/realms");
    }
}
