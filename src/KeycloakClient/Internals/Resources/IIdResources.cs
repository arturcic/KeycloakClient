namespace KeycloakClient.Internals.Resources
{
    public interface IIdResources<T, out TResource> : IResources<T> 
        where TResource : IResource<T> 
    {
        [Path("{id}")]
        TResource Id([PathParam("id")]string id);
    }

    public interface IIdResources<T, out TResource, in TFilter> : IResources<T, TFilter> 
        where TResource : IResource<T> 
        where TFilter : IFilter
    {
        [Path("{id}")]
        TResource Id([PathParam("id")]string id);
    }
}