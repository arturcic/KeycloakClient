namespace KeycloakClient.User.Models
{
    public class UserFilter : IFilter
    {
        [AliasAs("email")]
        public string Email { get; set; }

        [AliasAs("firstName")]
        public string FirstName { get; set; }

        [AliasAs("lastName")]
        public string LastName { get; set; }

        [AliasAs("username")]
        public string Username { get; set; }

        [AliasAs("search")]
        public string Search { get; set; }

        [AliasAs("first")]
        public int? First { get; set; }

        [AliasAs("max")]
        public int? Max { get; set; }
    }
}