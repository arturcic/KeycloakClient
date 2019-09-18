namespace KeycloakClient.Client.Models
{
    public class ClientFilter : IFilter
    {
        [AliasAs("clientId")]
        public string ClientId { get; set; }

        [AliasAs("viewableOnly")]
        public bool? ViewableOnly { get; set; }
    }
}