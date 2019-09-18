using KeycloakClient.Group.Models;
using KeycloakClient.Internals.Resources;

namespace KeycloakClient.Group
{
    public interface IGroupsResource : IIdResources<Models.Group, IGroupResource, GroupFilter>
    {
    }
}
