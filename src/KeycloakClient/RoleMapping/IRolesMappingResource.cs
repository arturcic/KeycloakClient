using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeycloakClient.RoleMapping
{
    public interface IRolesMappingResource
    {
        [Get]
        Task<List<Models.RoleMapping>> GetAllAsync();
        
        [Path("realm")]
        IRoleMappingResource Realm();
        
        [Path("clients/{clientId}")]
        IRoleMappingResource Client([PathParam("clientId")]string clientId);
    }
}
