namespace KeycloakClient.User.Models
{
    public class Credential
    {
        public string Value { get; set; }
        public string Type { get; set; } = "password";
        public bool Temporary { get; set; }
    }
}
