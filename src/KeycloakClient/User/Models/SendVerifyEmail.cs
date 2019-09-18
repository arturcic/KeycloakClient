namespace KeycloakClient.User.Models
{
    public class SendVerifyEmail
    {
        [AliasAs("client_id")]
        public string ClientId { get; set; }
        [AliasAs("redirect_uri")]
        public string RedirectUri { get; set; }
    }
}