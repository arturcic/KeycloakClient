using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.OidcClient;
using KeycloakClient.Extensions;
using Microsoft.Extensions.Options;

namespace KeycloakClient
{
    public class KeycloakRefreshTokenDelegatingHandler : DelegatingHandler
    {
        private readonly OidcClient _tokenClient;

        public KeycloakRefreshTokenDelegatingHandler(IOptions<KeycloakAdminClientOptions> options)
        {
            var adminClientOptions = options.Value;

            var address = adminClientOptions.Url.Combine($"realms/{adminClientOptions.Realm}/protocol/openid-connect/token").ToString();
            var oidcClientOptions = new OidcClientOptions
            {
                Flow = OidcClientOptions.AuthenticationFlow.Hybrid,
                Scope = adminClientOptions.ClientScope,
                Authority = address,
                ClientId = adminClientOptions.Username,
                ClientSecret = adminClientOptions.Password
            };
            _tokenClient = new OidcClient(oidcClientOptions);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!(InnerHandler is RefreshTokenDelegatingHandler))
            {
                var tokenResponse = await _tokenClient.LoginAsync(new LoginRequest(), cancellationToken);
                InnerHandler = new RefreshTokenDelegatingHandler(_tokenClient, tokenResponse.RefreshToken, tokenResponse.AccessToken, InnerHandler ?? new HttpClientHandler());
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
