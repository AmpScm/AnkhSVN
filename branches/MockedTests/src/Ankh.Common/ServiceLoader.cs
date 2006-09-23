using System;
using System.Reflection;
using System.ComponentModel.Design;
using System.Collections;
using System.Diagnostics;

namespace Ankh
{
    /// <summary>
    /// Helper class to load services
    /// </summary>
    public sealed class ServiceLoader
    {
        /// <summary>
        /// Initializes a new <see cref="ServiceLoader"/>
        /// </summary>
        public ServiceLoader()
        {
            serviceContainer = new ServiceContainer();
        }

        /// <summary>
        /// Loads all services marked with the <see cref="ServiceAttribute"/> attribute.
        /// </summary>
        /// <param name="assembly"></param>
        public void LoadServicesFrom(Assembly assembly)
        {
            ArrayList callInits = new ArrayList();

            foreach (Type tp in assembly.GetTypes())
            {
                foreach (ServiceAttribute attr in tp.GetCustomAttributes(typeof(ServiceAttribute), false))
                {
                    ConstructorInfo ci = tp.GetConstructor(new Type[] { typeof(IServiceProvider) });
                    if (ci != null)
                    {
                        object service = ci.Invoke(new object[] { serviceContainer });
                        serviceContainer.AddService(attr.ServiceType, service);

                        Debug.WriteLine("Created: " + service.ToString());

                        if (attr.CallMyInit)
                            callInits.Add(service);
                    }
                }
            }

            foreach (object service in callInits)
            {
                Debug.WriteLine("Initializing: " + service.ToString());
                MethodInfo mi = service.GetType().GetMethod("Init", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (mi != null)
                    mi.Invoke(service, new object[] { });
            }

        }

        /// <summary>
        /// Provides a way to manually add a service. The preferred way is to use the <see cref="LoadServicesFrom"/> method
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        public void AddService(Type type, object instance)
        {
            this.serviceContainer.AddService(type, instance);
        }

        /// <summary>
        /// Returns the <see cref="IServiceProvider"/> the services are registered to.
        /// </summary>
        public IServiceProvider ServiceProvider
        {
            get { return this.serviceContainer; }
        }

        private IServiceContainer serviceContainer;
    }
}
