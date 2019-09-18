using System.Threading.Tasks;

namespace KeycloakClient.Internals.Resources
{
    public interface IResource<T>
    {
        [Get]
        Task<T> GetAsync();
        
        [Put]
        Task<bool> UpdateAsync([Body]T resourceModel);
        
        [Delete]
        Task<bool> DeleteAsync();
    }
}