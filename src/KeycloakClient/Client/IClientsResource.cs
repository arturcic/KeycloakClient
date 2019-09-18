using KeycloakClient.Client.Models;
using KeycloakClient.Internals.Resources;

namespace KeycloakClient.Client
{
    public interface IClientsResource : IIdResources<Models.Client, IClientResource, ClientFilter>
    {
    }
}
