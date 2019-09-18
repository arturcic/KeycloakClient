using System.Collections.Generic;

namespace KeycloakClient.User.Models
{
    public class ExecuteActionsEmail
    {
        [AliasAs("client_id")]
        public string ClientId { get; set; }
        [AliasAs("redirect_uri")]
        public string RedirectUri { get; set; }
        [AliasAs("lifespan")]
        public int? Lifespan { get; set; }
        [AliasAs("actions")]
        public List<string> Actions { get; set; }
    }
}