using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using Castle.DynamicProxy;
using KeycloakClient.Extensions;

namespace KeycloakClient.Internals
{
    public class Interceptor : IInterceptor
    {
        private static readonly Regex PathParamMatch = new Regex(@"\{(.+?)\}");
        private string Path { get; }
        private readonly HttpClient _httpClient;
        private readonly ProxyGenerator _proxyGenerator;

        public Interceptor(HttpClient httpClient, ProxyGenerator proxyGenerator, string path = "")
        {
            Path = path;
            _httpClient = httpClient;
            _proxyGenerator = proxyGenerator;
        }
        
        public void Intercept(IInvocation invocation)
        {
            var requestAttribute = invocation.Method.GetCustomAttribute<HttpMethodAttribute>();
            var pathAttribute = invocation.Method.GetCustomAttribute<PathAttribute>();

            if (requestAttribute != null)
            {
                ExecuteHttpRequest(invocation, requestAttribute);
            }
            else if (pathAttribute != null)
            {
                CreateProxy(invocation, pathAttribute);
            }
        }

        private void ExecuteHttpRequest(IInvocation invocation, HttpMethodAttribute requestAttribute)
        {
            var routePath = GetRoutePath(invocation, Path, requestAttribute.Path);

            var method = GetMethodInfo(invocation, requestAttribute);

            var result = InvokeHttpRequest(invocation, routePath, method);
            invocation.ReturnValue = result;
        }

        private static string GetMethodName(HttpMethodAttribute requestAttribute)
        {
            string methodName;
            switch (requestAttribute)
            {
                case GetAttribute _:
                    methodName = nameof(HttpExtensions.GetAsync);
                    break;
                case PostAttribute _:
                    methodName = nameof(HttpExtensions.PostAsync);
                    break;
                case PutAttribute _:
                    methodName = nameof(HttpExtensions.PutAsync);
                    break;
                case DeleteAttribute _:
                    methodName = nameof(HttpExtensions.DeleteAsync);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(requestAttribute));
            }

            return methodName;
        }

        private static MethodInfo GetMethodInfo(IInvocation invocation, HttpMethodAttribute requestAttribute)
        {
            Type type = null;
            switch (requestAttribute)
            {
                case GetAttribute _:
                    type = invocation.Method.ReturnType.GetGenericArguments()[0];
                    break;
                case PostAttribute _:
                case PutAttribute _:
                case DeleteAttribute _:
                    var bodyAttribute = ParamAttribute<BodyAttribute>(invocation);
                    if (bodyAttribute != (null, null))
                    {
                        type = bodyAttribute.Value.GetType();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(requestAttribute));
            }

            var methodName = GetMethodName(requestAttribute);

            var method = type == null ? GetMethodInfo(methodName) : GetGenericMethodInfo(methodName, type);

            return method;
        }

        private object InvokeHttpRequest(IInvocation invocation, string routePath, MethodBase method)
        {
            object[] parameters;
            var bodyAttribute = ParamAttribute<BodyAttribute>(invocation);
            var queryAttribute = ParamAttribute<QueryAttribute>(invocation);

            if (bodyAttribute.Parameter != null)
            {
                parameters = new[] { _httpClient, routePath, bodyAttribute.Value };
            }
            else if (queryAttribute.Parameter != null && queryAttribute.Value != null)
            {
                var queryMap = QueryBuilderExtension.BuildQueryMap(queryAttribute.Value);
                var query = new QueryBuilder(queryMap);
                routePath += query;
                parameters = new object[] { _httpClient, routePath };
            }
            else
            {
                parameters = new object[] { _httpClient, routePath };
            }

            return method.Invoke(this, parameters);
        }

        private void CreateProxy(IInvocation invocation, PathAttribute pathAttribute)
        {
            var routePath = GetRoutePath(invocation, Path, pathAttribute.Path);

            var proxy = _proxyGenerator.CreateInterfaceProxyWithoutTarget(invocation.Method.ReturnType, new Interceptor(_httpClient, _proxyGenerator, routePath));

            invocation.ReturnValue = proxy;
        }

        private static MethodInfo GetMethodInfo(string name)
        {
            var methods = typeof(HttpExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public);
            return methods.SingleOrDefault(x => x.Name == name && !x.IsGenericMethod);
        }

        private static MethodInfo GetGenericMethodInfo(string name, Type type)
        {
            var methods = typeof(HttpExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public);
            return methods.SingleOrDefault(x => x.Name == name && x.IsGenericMethod)?.MakeGenericMethod(type);
        }

        private static string GetRoutePath(IInvocation invocation, string path, string routeTemplatePath)
        {
            var parameters = ArgumentsToDictionary(invocation);

            var routePath = ParseTemplate(routeTemplatePath ?? string.Empty, parameters);

            return string.IsNullOrWhiteSpace(routePath) ? path : $"{path}/{routePath.TrimStart('/')}".TrimStart('/');
        }

        private static string ParseTemplate(string routeTemplatePath, IReadOnlyDictionary<string, object> parameters)
        {
            // simple template replacement
            var placeholders = PathParamMatch.Matches(routeTemplatePath).OfType<Match>().Select(x => x.Groups[1].Value).ToList();

            foreach (var placeholder in placeholders)
            {
                if (parameters.ContainsKey(placeholder))
                {
                    var newValue = parameters[placeholder].ToString();
                    var template = $"{{{placeholder}}}";
                    routeTemplatePath = routeTemplatePath.Replace(template, newValue);
                }
            }

            return routeTemplatePath;
        }

        private static Dictionary<string, object> ArgumentsToDictionary(IInvocation invocation) =>
            (from parameter in Parameters(invocation)
                let attr = parameter.Parameter
                where attr.GetCustomAttribute<BodyAttribute>() == null
                where attr.GetCustomAttribute<QueryAttribute>() == null
                let pathParam = parameter.Parameter.GetCustomAttribute<PathParamAttribute>()
                let name = string.IsNullOrWhiteSpace(pathParam.Name) ? attr.Name : pathParam.Name
                select (key: name, value: parameter.Value))
            .ToDictionary(x => x.key, x => x.value);

        private static IEnumerable<(ParameterInfo Parameter, object Value)> Parameters(IInvocation invocation) 
            => invocation.Method.GetParameters().Select((x, index) => (Parameter: x, Value: invocation.Arguments[index]));

        private static (ParameterInfo Parameter, object Value) ParamAttribute<T>(IInvocation invocation) where T : Attribute 
            => Parameters(invocation).SingleOrDefault(x => x.Parameter.GetCustomAttribute<T>() != null);
    }
}