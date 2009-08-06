using System;
using System.Collections.Generic;
using System.Text;
using Ankh;
using System.ComponentModel.Design;

namespace AnkhSvn_UnitTestProject.Helpers
{
    class AnkhServiceProvider : IAnkhServiceProvider
    {
        internal ServiceContainer sc = new ServiceContainer();

        public void AddService(Type serviceType, object serviceInstance)
        {
            sc.AddService(serviceType, serviceInstance);
        }

        public T GetService<T>() where T : class
        {
            return (T)sc.GetService(typeof(T));
        }

        public T GetService<T>(Type serviceType) where T : class
        {
            return (T)sc.GetService(serviceType);
        }

        public object GetService(Type serviceType)
        {
            return sc.GetService(serviceType);
        }

    }
}
