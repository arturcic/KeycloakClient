using System;

namespace KeycloakClient
{
    public class KeycloakAdminClientOptions
    {
        public Uri Url { get; set; }
        public string Realm { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientScope { get; set; }
    }
}
