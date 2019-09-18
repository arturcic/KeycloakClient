using Newtonsoft.Json;

namespace KeycloakClient.Realm.Models
{
    public class Realm
    {
        public string Id { get; set; }
        
        [JsonProperty("realm")]
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
    }
}
