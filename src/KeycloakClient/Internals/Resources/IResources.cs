using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeycloakClient.Internals.Resources
{
    public interface IResources<T>
    {
        [Get]
        Task<List<T>> GetAllAsync();

        [Post]
        Task<(string Id, bool Success)> CreateAsync([Body]T resourceModel);
    }

    public interface IResources<T, in TFilter> : IResources<T> where TFilter : IFilter
    {
        [Get]
        Task<List<T>> FindAsync([Query]TFilter filter);
    }
}
