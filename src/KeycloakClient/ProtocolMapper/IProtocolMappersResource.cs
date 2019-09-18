using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeycloakClient.ProtocolMapper
{
    public interface IProtocolMappersResource
    {
        [Get("protocol/{protocol}")]
        Task<List<Models.ProtocolMapper>> GetProtocolMappersAsync([PathParam("protocol")]string protocol);

        [Post("add-models")]
        Task<bool> CreateAsync([Body]List<Models.ProtocolMapper> mappers);

        [Post("models")]
        Task<(string Id, bool Success)> CreateAsync([Body]Models.ProtocolMapper mappers);

        [Get("models")]
        Task<List<Models.ProtocolMapper>> GetAllAsync();

        [Get("models/{id}")]
        Task<Models.ProtocolMapper> GetAsync([PathParam("id")]string id);

        [Put("models/{id}")]
        Task<Models.ProtocolMapper> PutAsync([PathParam("id")]string id, [Body] Models.ProtocolMapper protocolMapper);

        [Delete("models/{id}")]
        Task<Models.ProtocolMapper> DeleteAsync([PathParam("id")]string id);
    }
}
