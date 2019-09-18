using System.Collections.Generic;

namespace KeycloakClient.RoleMapping.Models
{
    public class ClientRoleMapping
    {
        public string Id { get; set; }
        public string Client { get; set; }
        public List<Role.Models.Role> Mappings { get; set; }
    }
}