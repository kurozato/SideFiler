namespace BlackSugar.SimpleMvp
{

    /// <summary>
    /// this is light wrapper of something di container.
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// this is wrappered container object
        /// </summary>
        object? ContainerObject { get; }

        TService? Resolve<TService>() where TService : class;

        object? Resolve(Type serviceType);
    }
}