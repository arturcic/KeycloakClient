using System.Threading.Tasks;
using KeycloakClient.Client;
using KeycloakClient.Group;
using KeycloakClient.Internals.Resources;
using KeycloakClient.Role;
using KeycloakClient.User;

namespace KeycloakClient.Realm
{
    public interface IRealmResource : IResource<Models.Realm>
    {
        [Path("users")]
        IUsersResource Users();
        
        [Path("groups")]
        IGroupsResource Groups();
        
        [Path("clients")]
        IClientsResource Clients();
        
        [Path("roles")]
        IRolesResource Roles();
        
        [Path("roles-by-id/{id}")]
        IRoleByIdResource RolesById([PathParam("id")]string id);

        [Delete("sessions/{session}")]
        Task<bool> LogoutSessionAsync([PathParam("session")] string session);
    }
}
