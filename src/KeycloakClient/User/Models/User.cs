namespace KeycloakClient.User.Models
{
    public class User
    {
        public string Id { get; set; }
        public long CreatedTimestamp { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool Enabled { get; set; }
        public bool EmailVerified { get; set; }
    }
}