using KeycloakClient.Internals.Resources;
using KeycloakClient.ProtocolMapper;
using KeycloakClient.Role;

namespace KeycloakClient.Client
{
    public interface IClientResource : IResource<Models.Client>
    {
        [Path("roles")]
        IRolesResource Roles();
        
        [Path("protocol-mappers")]
        IProtocolMappersResource ProtocolMappers();
    }
}
