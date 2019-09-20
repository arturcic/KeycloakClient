using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KeycloakClient.Test
{
    public class CheckPathConventions
    {
        public CheckPathConventions()
        {
            var httpClient = Substitute.For<IHttpClient>();
            client = new KeycloakClient(httpClient as HttpClient, Options.Create(new KeycloakAdminClientOptions
            {
                ClientId = "admin-cli",
                Username = "Admin",
                Password = "Admin",
                ClientScope = "testClientScope",
                Realm = "testRealm",
                Url = new Uri("http://localhost:8080/auth/")
            }));
        }

        [Fact]
        public void CheckRealmsPath()
        {
            var path = client.Realms();
            Assert.Equal("admin/realms", GetFinalPath(path));

        }

        [Fact]
        public void CheckRealmsNamePath()
        {
            var path = client.Realms().Name("testName");
            Assert.Equal("admin/realms/testName", GetFinalPath(path));

        }

        [Fact]
        public void CheckRealmsNameClientsPath()
        {
            var path = client.Realms().Name("testName").Clients();
            Assert.Equal("admin/realms/testName/clients", GetFinalPath(path));

        }

        [Fact]
        public void CheckRealmsNameClientsIdPath()
        {
            var path = client.Realms().Name("testName").Clients().Id("testClientId");
            Assert.Equal("admin/realms/testName/clients/testClientId", GetFinalPath(path));

        }

        [Fact]
        public void CheckRealmsNameClientsIdRolesPath()
        {
            var path = client.Realms().Name("testName").Clients().Id("testClientId").Roles();
            Assert.Equal("admin/realms/testName/clients/testClientId/roles", GetFinalPath(path));

        }

        [Fact]
        public void CheckRealmsNameClientsIdRolesNamePath()
        {
            var path = client.Realms().Name("testName").Clients().Id("testClientId").Roles().Name("testRoleName");
            Assert.Equal("admin/realms/testName/clients/testClientId/roles/testRoleName", GetFinalPath(path));

        }

        [Fact(Skip = "not ready yet")]
        public void CheckRealmsNameClientsIdRolesNameRoleCompositePath()
        {
            var path = client.Realms().Name("testName").Clients().Id("testClientId").Roles().Name("testRoleName").GetRoleCompositesAsync();
            Assert.Equal("admin/realms/testName/clients/testClientId/roles/testRoleName", GetFinalPath(path));

        }

        private IKeycloakClient client;

        private string GetFinalPath(object path)
        {
            try
            {
                const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
                var proxyType = path.GetType();
                var instanceField = proxyType.GetField("__interceptors", flags);
                var fieldValue = instanceField.GetValue(path) as object[];

                proxyType = fieldValue[0].GetType();
                instanceField = proxyType.GetField("<Path>k__BackingField", flags);
                return instanceField.GetValue(fieldValue[0]).ToString();
            }
            catch (RuntimeBinderException)
            {
                return "";
            }
        }

        public interface IHttpClient
        {
            Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken);
            Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken);
            Task<HttpResponseMessage> DeleteAsync(string requestUri);
            Task<HttpResponseMessage> DeleteAsync(Uri requestUri);
            Task<HttpResponseMessage> GetAsync(string requestUri);
            Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption);
            Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken);
            Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken);
            Task<HttpResponseMessage> GetAsync(Uri requestUri);
            Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption);
            Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken);
            Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken);
            Task<byte[]> GetByteArrayAsync(string requestUri);
            Task<byte[]> GetByteArrayAsync(Uri requestUri);
            Task<Stream> GetStreamAsync(string requestUri);
            Task<Stream> GetStreamAsync(Uri requestUri);
            Task<string> GetStringAsync(string requestUri);
            Task<string> GetStringAsync(Uri requestUri);
            Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
            Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken);
            Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content);
            Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken);
            Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content);
            Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken);
            Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content);
            Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken);
            Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
            Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption);
            Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken);
            Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
        }

        public class CustomHttpClient: HttpClient, IHttpClient { }
    }
}
