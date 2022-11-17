using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlackSugar.SimpleMvp
{
    public class DependencyResolver : IDependencyResolver
    {
        private IServiceProvider? provider;

        public object? ContainerObject => provider;

        public TService? Resolve<TService>() where TService : class => provider?.GetService<TService>();

        public object? Resolve(Type serviceType) => provider?.GetService(serviceType);

        public void Set(Action<IServiceCollection> register)
        {
            var services = new ServiceCollection();
            register(services);
            provider = services.BuildServiceProvider();
        }
    }
}
