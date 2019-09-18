using KeycloakClient.Internals.Resources;
using KeycloakClient.User.Models;

namespace KeycloakClient.User
{
    public interface IUsersResource : IIdResources<Models.User, IUserResource, UserFilter>
    {
    }
}
