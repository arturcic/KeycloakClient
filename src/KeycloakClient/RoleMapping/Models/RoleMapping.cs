using System.Collections.Generic;

namespace KeycloakClient.RoleMapping.Models
{
    public class RoleMapping
    {
        public List<Role.Models.Role> RealmMappings { get; set; }
        public Dictionary<string, ClientRoleMapping> ClientMappings { get; set; }
    }
}