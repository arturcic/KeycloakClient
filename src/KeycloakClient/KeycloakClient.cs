using System.Net.Http;
using KeycloakClient.Extensions;
using KeycloakClient.Internals;
using KeycloakClient.Realm;
using Microsoft.Extensions.Options;

namespace KeycloakClient
{
    public interface IKeycloakClient
    {
        IRealmsResource Realms();
    }

    public class KeycloakClient : IKeycloakClient
    {
        private HttpClient HttpClient { get; }

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
