using System;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Shell.Interop;

namespace AnkhSvn_UnitTestProject.Helpers
{
    class ServiceProviderHelper : IDisposable
    {
        internal static OleServiceProvider serviceProvider;
        static ServiceProviderHelper()
        {
            serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
        }

        Type type;

        private ServiceProviderHelper(Type t, object instance)
        {
            type = t;
            serviceProvider.AddService(t, instance, true);
        }

        private ServiceProviderHelper()
        {
        }

        public void Dispose()
        {
            if (type != null && serviceProvider.GetService(type) != null)
                serviceProvider.RemoveService(type);
        }

        public static IDisposable AddService(Type t, object instance)
        {
            return new ServiceProviderHelper(t, instance);
        }

        public static IDisposable SetSite(IVsPackage package)
        {
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");
            return new ServiceProviderHelper();
        }
    }
}
