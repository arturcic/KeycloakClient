using Newtonsoft.Json;

namespace KeycloakClient.ProtocolMapper.Models
{
    public class ProtocolMapper
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Protocol { get; set; } = "openid-connect";
        public bool ConsentRequired { get; set; }

        [JsonProperty("protocolMapper")] 
        public string Mapper { get; set; } = "oidc-usermodel-client-role-mapper";

        public ProtocolMapperConfig Config { get; set; }
    }
}