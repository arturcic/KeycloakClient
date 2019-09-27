using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeycloakClient.RoleMapping
{
    public interface IRolesMappingResource
    {
        [Get]
        Task<Models.RoleMapping> GetAsync();
        
        [Path("realm")]
        IRoleMappingResource Realm();
        
        [Path("clients/{clientId}")]
        IRoleMappingResource Client([PathParam("clientId")]string clientId);
    }
}
