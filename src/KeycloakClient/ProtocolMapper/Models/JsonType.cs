using System.Runtime.Serialization;

namespace KeycloakClient.ProtocolMapper.Models
{
    public enum JsonType
    {
        [EnumMember(Value = "String")]
        String,
        
        [EnumMember(Value = "long")]
        Long,

        [EnumMember(Value = "int")]
        Integer,

        [EnumMember(Value = "boolean")]
        Boolean,
    }
}