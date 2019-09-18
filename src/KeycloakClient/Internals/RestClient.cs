using System.Net.Http;
using Castle.DynamicProxy;

namespace KeycloakClient.Internals
{
    public class RestClient
    {
        public static T For<T>(HttpClient httpClient, string path = "") where T : class
        {
            var proxyGenerator = new ProxyGenerator();
            return proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(new Interceptor(httpClient, proxyGenerator, path));
        }
    }
}