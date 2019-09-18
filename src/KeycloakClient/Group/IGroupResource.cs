using System.Collections.Generic;
using System.Threading.Tasks;
using KeycloakClient.Internals.Resources;
using KeycloakClient.RoleMapping;
using KeycloakClient.User.Models;

namespace KeycloakClient.Group
{
    public interface IGroupResource : IResource<Models.Group>
    {
        [Get("members")]
        Task<List<User.Models.User>> GetUsersAsync([Query]UserFilter filter = null);
        [Path("role-mappings")]
        IRolesMappingResource Roles();
    }
}
