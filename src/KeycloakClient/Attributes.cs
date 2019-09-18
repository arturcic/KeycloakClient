using System;
using System.Net.Http;

namespace KeycloakClient
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class BodyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class QueryAttribute : Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class AliasAsAttribute : Attribute
    {
        public AliasAsAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public sealed class PathAttribute : Attribute
    {
        public string Path { get; }

        public PathAttribute(string path = "")
        {
            Path = path;
        }
    }
    
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class PathParamAttribute : Attribute
    {
        public string Name { get; }

        public PathParamAttribute(string name = "")
        {
            Name = name;
        }
    }

    public abstract class HttpMethodAttribute : Attribute
    {
        protected HttpMethodAttribute(string path = "")
        {
            Path = path;
        }

        public abstract HttpMethod Method { get; }

        public virtual string Path { get; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : HttpMethodAttribute
    {
        public GetAttribute(string path = "") : base(path) { }

        public override HttpMethod Method => HttpMethod.Get;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : HttpMethodAttribute
    {
        public PostAttribute(string path= "") : base(path) { }

        public override HttpMethod Method => HttpMethod.Post;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PutAttribute : HttpMethodAttribute
    {
        public PutAttribute(string path= "") : base(path) { }

        public override HttpMethod Method => HttpMethod.Put;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DeleteAttribute : HttpMethodAttribute
    {
        public DeleteAttribute(string path= "") : base(path) { }

        public override HttpMethod Method => HttpMethod.Delete;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PatchAttribute : HttpMethodAttribute
    {
        public PatchAttribute(string path= "") : base(path) { }

        public override HttpMethod Method => new HttpMethod("PATCH");
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HeadAttribute : HttpMethodAttribute
    {
        public HeadAttribute(string path= "") : base(path) { }

        public override HttpMethod Method => HttpMethod.Head;
    }
}