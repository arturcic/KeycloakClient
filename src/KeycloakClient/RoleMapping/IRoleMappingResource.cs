using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeycloakClient.RoleMapping
{
    public interface IRoleMappingResource 
    {
        [Get]
        Task<List<Role.Models.Role>> GetAllAsync();

        [Get("available")]
        Task<List<Role.Models.Role>> GetAvailableAsync();
        
        [Get("composite")]
        Task<List<Role.Models.Role>> GetEffectiveAsync();
        
        [Post]
        Task<(string, bool)> AddRolesAsync([Body]List<Role.Models.Role> roles);
        
        [Delete]
        Task<bool> RemoveRolesAsync([Body]List<Role.Models.Role> roles);
    }
}
