namespace KeycloakClient.Role.Models
{
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Composite { get; set; } = false;
        public bool ClientRole { get; set; } = false;
        public string ContainerId { get; set; }
    }
}
