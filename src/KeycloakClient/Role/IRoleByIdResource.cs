using System.Collections.Generic;
using System.Threading.Tasks;
using KeycloakClient.Internals.Resources;

namespace KeycloakClient.Role
{
    public interface IRoleByIdResource : IResource<Models.Role>
    {
        [Get("composite")]
        Task<List<Role.Models.Role>> GetRoleCompositesAsync();
        
        [Get("composites/realm")]
        Task<List<Role.Models.Role>> GetRealmRoleCompositesAsync();

        [Get("composites/clients/{clientId}")]
        Task<List<Role.Models.Role>> GetClientRoleCompositesAsync([PathParam("clientId")]string clientId);

        [Post("composites")]
        Task<bool> AddAsync([Body]List<Models.Role> roles);

        [Delete("composites")]
        Task<bool> RemoveAsync([Body]List<Models.Role> roles);
    }
}
