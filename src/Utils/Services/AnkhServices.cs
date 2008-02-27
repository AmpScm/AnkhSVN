using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;

namespace Utils.Services
{
    /// <summary>
    /// General purpose service provider.
    /// </summary>
    public static class AnkhServices
    {
        static IServiceContainer _container = new ServiceContainer();
        static readonly object _lock = new object();

        /// <summary>
        /// Returns the services identified by <see cref="TService"/>
        /// </summary>
        /// <typeparam name="TService">The type of service to be returned</typeparam>
        /// <returns></returns>
        public static TService GetService<TService>()
        {
            lock (_lock)
            {
                return (TService)_container.GetService(typeof(TService));
            }
        }

        /// <summary>
        /// Adds a service, using <see cref="type"/> as key.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        public static void AddService(Type type, object instance)
        {
            lock (_lock)
            {
                _container.AddService(type, instance);
            }
        }
    }
}
