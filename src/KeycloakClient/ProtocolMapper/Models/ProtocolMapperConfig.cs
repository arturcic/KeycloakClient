using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KeycloakClient.ProtocolMapper.Models
{
    public class ProtocolMapperConfig
    {
        [JsonProperty("userinfo.token.claim")]
        public bool AddToUserInfoToken { get; set; }
        
        [JsonProperty("id.token.claim")]
        public bool AddToIdToken { get; set; }
        
        [JsonProperty("access.token.claim")]
        public bool AddToAccessToken { get; set; }
        
        [JsonProperty("claim.name")]
        public string ClaimName { get; set; }

        [JsonProperty("jsonType.label")]
        [JsonConverter(typeof(StringEnumConverter))]
        public JsonType JsonType { get; set; } = JsonType.String;

        [JsonProperty("usermodel.clientRoleMapping.clientId")]
        public string MappingModel { get; set; }
    }
}