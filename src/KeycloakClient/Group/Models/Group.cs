using System.Collections.Generic;

namespace KeycloakClient.Group.Models
{
    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public List<string> RealmRoles { get; set; }
        public Dictionary<string, List<string>> ClientRoles { get; set; }
    }
}
