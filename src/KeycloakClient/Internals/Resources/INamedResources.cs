namespace KeycloakClient.Internals.Resources
{
    public interface INamedResources<T, out TResource> : IResources<T> 
        where TResource : IResource<T> 
    {
        [Path("{name}")]
        TResource Name([PathParam("name")]string name);
    }

    public interface INamedResources<T, out TResource, in TFilter> : IResources<T, TFilter> 
        where TResource : IResource<T> 
        where TFilter : IFilter
    {
        [Path("{name}")]
        TResource Name([PathParam("name")]string name);
    }
}