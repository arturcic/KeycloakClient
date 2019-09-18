using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KeycloakClient.Extensions
{
    public class HttpExtensions
    {
        public static async Task<T> GetAsync<T>(HttpClient httpClient, string route)
        {
            var response = await httpClient.GetAsync(route);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadJsonAsync<T>();
        }

        public static async Task<bool> PutAsync<T>(HttpClient httpClient, string route, T data)
        {
            var response = await httpClient.PutJsonAsync(route, data);
            return await response.IsSuccess();
        }

        public static async Task<bool> PutAsync(HttpClient httpClient, string resourcePath)
        {
            var response = await httpClient.PutAsync(resourcePath);
            return await response.IsSuccess();
        }

        public static async Task<(string Id, bool Success)> PostAsync(HttpClient httpClient, string resourcePath)
        {
            var response = await httpClient.PostAsync(resourcePath);
            if (await response.IsSuccess())
            {
                var id = response.Headers.Location.Segments.Last();
                return (Id: id, Success: true);
            }

            return (Id: null, Success: false);
        }

        public static async Task<(string Id, bool Success)> PostAsync<T>(HttpClient httpClient, string resourcePath, T data)
        {
            var response = await httpClient.PostJsonAsync(resourcePath, data);

            if (await response.IsSuccess())
            {
                var id = response.Headers.Location?.Segments?.Last();
                return (Id: id, Success: true);
            }

            return (Id: null, Success: false);
        }

        public static async Task<bool> DeleteAsync(HttpClient httpClient, string resourcePath)
        {
            var response = await httpClient.DeleteAsync(resourcePath);
            return await response.IsSuccess();
        }

        public static async Task<bool> DeleteAsync<T>(HttpClient httpClient, string resourcePath, T data)
        {
            var response = await httpClient.DeleteJsonAsync(resourcePath, data);
            return await response.IsSuccess();
        }
    }
}
