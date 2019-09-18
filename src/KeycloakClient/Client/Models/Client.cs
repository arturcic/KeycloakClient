namespace KeycloakClient.Client.Models
{
    public class Client
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClientId { get; set; }
        public bool Enabled { get; set; }
        public string[] RedirectUris { get; set; }
        public string[] WebOrigins { get; set; }
        public bool ConsentRequired { get; set; }
        public bool StandardFlowEnabled { get; set; }
        public bool ImplicitFlowEnabled { get; set; }
        public bool DirectAccessGrantsEnabled { get; set; }
        public bool ServiceAccountsEnabled { get; set; }
        public bool PublicClient { get; set; }
        public string Protocol { get; set; }
    }
}
