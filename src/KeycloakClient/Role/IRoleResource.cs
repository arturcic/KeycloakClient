using System.Collections.Generic;
using System.Threading.Tasks;
using KeycloakClient.User.Models;

namespace KeycloakClient.Role
{
    public interface IRoleResource : IRoleByIdResource
    {
        [Get("users")]
        Task<List<User.Models.User>> GetUsersAsync([Query]UserFilter filter = null);
    }
}