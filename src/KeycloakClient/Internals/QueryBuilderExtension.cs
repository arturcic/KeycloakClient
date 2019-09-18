using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;

namespace KeycloakClient.Internals
{
    public class QueryBuilder : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IList<KeyValuePair<string, string>> _params;

        public QueryBuilder()
        {
            _params = new List<KeyValuePair<string, string>>();
        }

        public QueryBuilder(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            _params = new List<KeyValuePair<string, string>>(parameters.ToDictionary(x => x.Key, x => x.Value.ToString()));
        }

        public QueryBuilder(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            _params = new List<KeyValuePair<string, string>>(parameters);
        }

        public void Add(string key, IEnumerable<string> values)
        {
            foreach (var str in values)
                _params.Add(new KeyValuePair<string, string>(key, str));
        }

        public void Add(string key, string value)
        {
            _params.Add(new KeyValuePair<string, string>(key, value));
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            var flag = true;
            foreach (var keyValuePair in _params)
            {
                stringBuilder.Append(flag ? "?" : "&");
                flag = false;
                stringBuilder.Append(UrlEncoder.Default.Encode(keyValuePair.Key));
                stringBuilder.Append("=");
                stringBuilder.Append(UrlEncoder.Default.Encode(keyValuePair.Value));
            }
            return stringBuilder.ToString();
        }

        public QueryString ToQueryString()
        {
            return new QueryString(ToString());
        }

        public override int GetHashCode()
        {
            return ToQueryString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ToQueryString().Equals(obj);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _params.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _params.GetEnumerator();
        }
    }

    public class QueryBuilderExtension
    {
        public static IEnumerable<KeyValuePair<string, object>> BuildQueryMap(object @object, string delimiter = null)
        {
            if (@object is IDictionary dictionary)
            {
                return BuildQueryMap(dictionary, delimiter);
            }

            var queryMap = new List<KeyValuePair<string, object>>();

            var props = @object.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && p.GetMethod.IsPublic);

            foreach (var propertyInfo in props)
            {
                var obj = propertyInfo.GetValue(@object);
                if (obj == null)
                    continue;

                var key = propertyInfo.Name;

                var aliasAttribute = propertyInfo.GetCustomAttribute<AliasAsAttribute>();
                if (aliasAttribute != null)
                    key = aliasAttribute.Name;

                if (DoNotConvertToQueryMap(obj))
                {
                    queryMap.Add(new KeyValuePair<string, object>(key, obj));
                    continue;
                }

                switch (obj)
                {
                    case IDictionary dict:
                        queryMap.AddRange(BuildQueryMap(dict, delimiter).Select(keyValuePair =>
                            new KeyValuePair<string, object>($"{key}{delimiter}{keyValuePair.Key}",
                                keyValuePair.Value)));

                        break;

                    case IEnumerable enumerable:
                        queryMap.AddRange(from object o in enumerable select new KeyValuePair<string, object>(key, o));

                        break;

                    default:
                        queryMap.AddRange(BuildQueryMap(obj, delimiter).Select(keyValuePair =>
                            new KeyValuePair<string, object>($"{key}{delimiter}{keyValuePair.Key}",
                                keyValuePair.Value)));

                        break;
                }
            }

            return queryMap;
        }

        private static IEnumerable<KeyValuePair<string, object>> BuildQueryMap(IDictionary dictionary, string delimiter = null)
        {
            var queryMap = new List<KeyValuePair<string, object>>();

            var props = dictionary.Keys;
            foreach (string key in props)
            {
                var obj = dictionary[key];
                if (obj == null)
                    continue;

                if (DoNotConvertToQueryMap(obj))
                {
                    queryMap.Add(new KeyValuePair<string, object>(key, obj));
                }
                else
                {
                    queryMap.AddRange(BuildQueryMap(obj, delimiter).Select(keyValuePair =>
                        new KeyValuePair<string, object>($"{key}{delimiter}{keyValuePair.Key}", keyValuePair.Value)));
                }
            }

            return queryMap;
        }

        private static bool DoNotConvertToQueryMap(object value)
        {
            if (value == null)
                return false;

            var type = value.GetType();

            bool ShouldReturn() => type == typeof(string) ||
                                   type == typeof(bool) ||
                                   type == typeof(char) ||
                                   typeof(IFormattable).IsAssignableFrom(type) ||
                                   type == typeof(Uri);

            // Bail out early & match string
            if (ShouldReturn())
                return true;

            // Get the element type for enumerable
            if (value is IEnumerable enu)
            {
                var enumerableType = typeof(IEnumerable<>);
                // We don't want to enumerate to get the type, so we'll just look for IEnumerable<T>
                var intType = type.GetInterfaces()
                    .FirstOrDefault(i => i.GetTypeInfo().IsGenericType &&
                                         i.GetGenericTypeDefinition() == enumerableType);

                if (intType != null)
                {
                    type = intType.GetGenericArguments()[0];
                }
            }

            return ShouldReturn();
        }
    }
}