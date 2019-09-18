using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using KeycloakClient.Extensions;
using Microsoft.Extensions.Options;

namespace KeycloakClient
{
    public class KeycloakRefreshTokenDelegatingHandler : DelegatingHandler
    {
        private readonly TokenClient _tokenClient;
        private readonly KeycloakAdminClientOptions _options;

        public KeycloakRefreshTokenDelegatingHandler(IOptions<KeycloakAdminClientOptions> options)
        {
            _options = options.Value;

            var address = _options.Url.Combine($"realms/{_options.Realm}/protocol/openid-connect/token").ToString();
            _tokenClient = new TokenClient(address, _options.ClientId);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!(InnerHandler is RefreshTokenDelegatingHandler))
            {
                var tokenResponse = await _tokenClient.RequestResourceOwnerPasswordAsync(_options.Username, _options.Password, _options.ClientScope, cancellationToken: cancellationToken);
                InnerHandler = new RefreshTokenDelegatingHandler(_tokenClient, tokenResponse.RefreshToken, tokenResponse.AccessToken, InnerHandler ?? new HttpClientHandler());
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
