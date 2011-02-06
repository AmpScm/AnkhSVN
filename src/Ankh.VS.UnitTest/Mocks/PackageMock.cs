using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ankh;
using Ankh.UI;
using Microsoft.VisualStudio.Shell.Interop;

namespace AnkhSvn_UnitTestProject.Mocks
{
    static class PackageMock
    {
        internal static object EmptyContext(Microsoft.VsSDK.UnitTestLibrary.OleServiceProvider serviceProvider)
        {
            return new EmptyPackage(serviceProvider);
        }

        class EmptyPackage : IAnkhPackage
        {
            readonly IServiceProvider _serviceProvider;

            public EmptyPackage(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }
            #region IAnkhPackage Members

            public Version UIVersion
            {
                get { throw new NotImplementedException(); }
            }

            public Version PackageVersion
            {
                get { throw new NotImplementedException(); }
            }

            public void ShowToolWindow(Ankh.AnkhToolWindow window)
            {
                throw new NotImplementedException();
            }

            public void ShowToolWindow(Ankh.AnkhToolWindow window, int id, bool create)
            {
                throw new NotImplementedException();
            }

            public void CloseToolWindow(AnkhToolWindow toolWindow, int id, __FRAMECLOSE frameClose)
            {
                throw new NotImplementedException();
            }

            public void RegisterIdleProcessor(Ankh.VS.IAnkhIdleProcessor processor)
            {
                throw new NotImplementedException();
            }

            public void UnregisterIdleProcessor(Ankh.VS.IAnkhIdleProcessor processor)
            {
                throw new NotImplementedException();
            }

            public System.Windows.Forms.AmbientProperties AmbientProperties
            {
                get { throw new NotImplementedException(); }
            }

            public bool LoadUserProperties(string streamName)
            {
                throw new NotImplementedException();
            }

            public Microsoft.Win32.RegistryKey ApplicationRegistryRoot
            {
                get { throw new NotImplementedException(); }
            }

            public Microsoft.Win32.RegistryKey UserRegistryRoot
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region IAnkhServiceProvider Members

            public T GetService<T>() where T : class
            {
                throw new NotImplementedException();
            }

            public T GetService<T>(Type serviceType) where T : class
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IServiceProvider Members

            public object GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IServiceContainer Members

            public void AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback, bool promote)
            {
                throw new NotImplementedException();
            }

            public void AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback)
            {
                throw new NotImplementedException();
            }

            public void AddService(Type serviceType, object serviceInstance, bool promote)
            {
                throw new NotImplementedException();
            }

            public void AddService(Type serviceType, object serviceInstance)
            {
                throw new NotImplementedException();
            }

            public void RemoveService(Type serviceType, bool promote)
            {
                throw new NotImplementedException();
            }

            public void RemoveService(Type serviceType)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IAnkhQueryService Members

            public T QueryService<T>(Guid serviceGuid) where T : class
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}