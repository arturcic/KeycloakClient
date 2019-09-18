using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KeycloakClient.Extensions
{
    public static class HttpContentExtensions
    {
        public static Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient httpClient, string url, T data, CancellationToken cancellationToken = default) 
            => httpClient.PostAsync(url, Serialize(data), cancellationToken);

        public static Task<HttpResponseMessage> PostAsync(this HttpClient httpClient, string url, CancellationToken cancellationToken = default) 
            => httpClient.PostAsync(url, EmptyContent(), cancellationToken);
        public static Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient httpClient, Uri url, T data, CancellationToken cancellationToken = default) 
            => httpClient.PostAsync(url, Serialize(data), cancellationToken);

        public static Task<HttpResponseMessage> PostAsync(this HttpClient httpClient, Uri url, CancellationToken cancellationToken = default) 
            => httpClient.PostAsync(url, EmptyContent(), cancellationToken);

        public static Task<HttpResponseMessage> PutJsonAsync<T>(this HttpClient httpClient, string url, T data, CancellationToken cancellationToken = default) 
            => httpClient.PutAsync(url, Serialize(data), cancellationToken);

        public static Task<HttpResponseMessage> PutAsync(this HttpClient httpClient, string url, CancellationToken cancellationToken = default) 
            => httpClient.PutAsync(url, EmptyContent(), cancellationToken);

        public static Task<HttpResponseMessage> PutJsonAsync<T>(this HttpClient httpClient, Uri url, T data, CancellationToken cancellationToken = default) 
            => httpClient.PutAsync(url, Serialize(data), cancellationToken);

        public static Task<HttpResponseMessage> PutAsync(this HttpClient httpClient, Uri url, CancellationToken cancellationToken = default) 
            => httpClient.PutAsync(url, EmptyContent(), cancellationToken);

        public static Task<HttpResponseMessage> DeleteJsonAsync<T>(this HttpClient httpClient, string requestUri, T data, CancellationToken cancellationToken = default)
            => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = Serialize(data) }, cancellationToken);

        public static Task<HttpResponseMessage> DeleteJsonAsync<T>(this HttpClient httpClient, Uri requestUri, T data, CancellationToken cancellationToken = default)
            => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = Serialize(data) }, cancellationToken);

        public static async Task<T> ReadJsonAsync<T>(this HttpContent content) 
            => JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());

        public static async Task<object> ReadJsonAsync(this HttpContent content, Type type) 
            => JsonConvert.DeserializeObject(await content.ReadAsStringAsync(), type);

        public static async Task<bool> IsSuccess(this HttpResponseMessage response)
        {
            var responseMessage = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }

        private static HttpContent Serialize<T>(T data) => new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        private static HttpContent EmptyContent() => new StringContent(string.Empty, Encoding.UTF8, "application/json");
    }
}
