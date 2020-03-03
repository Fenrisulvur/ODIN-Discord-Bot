using Discord;
using IslaBot.Discord;
using IslaBot.Storage;
using IslaBot.Storage.Implementations;
using Unity;
using Unity.Lifetime;
using Unity.Resolution;

namespace IslaBot
{
    public static class Unity
    {
        private static UnityContainer _container;

        public static UnityContainer Container
        {
            get
            {
                if (_container == null)
                    RegisterTypes();
                return _container;
            }
        }

        public static void RegisterTypes()
        {
            _container = new UnityContainer();
            _container.RegisterType<InMemoryStorage>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ILogger, Logger>(new ContainerControlledLifetimeManager());
            _container.RegisterType<Discord.Connection>(new ContainerControlledLifetimeManager());
        }

        public static T Resolve<T>()
        {
            return (T)_container.Resolve<T>();
        }
    }

}
