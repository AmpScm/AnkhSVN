using System;
using Ankh;
using Ankh.UI;
using Microsoft.VisualStudio.Shell.Interop;

namespace AnkhSvn_UnitTestProject.Mocks
{
    static class PackageMock
    {
        internal static object EmptyContext(IServiceProvider serviceProvider)
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

            public Version UIVersion
            {
                get { return new Version(2, 9, 0, 0); }
            }

            public Version PackageVersion
            {
                get { return new Version(2, 9, 0, 0); }
            }

            public void ShowToolWindow(AnkhToolWindow window)
            {
                throw new NotImplementedException();
            }

            public void ShowToolWindow(AnkhToolWindow window, int id, bool create)
            {
                throw new NotImplementedException();
            }

            public void CloseToolWindow(AnkhToolWindow toolWindow, int id, FrameCloseMode close)
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

            public Microsoft.Win32.RegistryKey ApplicationRegistryRoot
            {
                get
                {
                    return Microsoft.Win32.RegistryKey.OpenBaseKey(
                        Microsoft.Win32.RegistryHive.CurrentUser,
                        Microsoft.Win32.RegistryView.Default);
                }
            }

            public Microsoft.Win32.RegistryKey UserRegistryRoot
            {
                get
                {
                    return Microsoft.Win32.RegistryKey.OpenBaseKey(
                        Microsoft.Win32.RegistryHive.CurrentUser,
                        Microsoft.Win32.RegistryView.Default);
                }
            }

            public T GetService<T>() where T : class
            {
                return _serviceProvider.GetService(typeof(T)) as T;
            }

            public T GetService<T>(Type serviceType) where T : class
            {
                return _serviceProvider.GetService(serviceType) as T;
            }

            public object GetService(Type serviceType)
            {
                return _serviceProvider.GetService(serviceType);
            }

            public void AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback, bool promote)
            {
                throw new NotSupportedException();
            }

            public void AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback)
            {
                throw new NotSupportedException();
            }

            public void AddService(Type serviceType, object serviceInstance, bool promote)
            {
                throw new NotSupportedException();
            }

            public void AddService(Type serviceType, object serviceInstance)
            {
                throw new NotSupportedException();
            }

            public void RemoveService(Type serviceType, bool promote)
            {
                throw new NotSupportedException();
            }

            public void RemoveService(Type serviceType)
            {
                throw new NotSupportedException();
            }

            public T QueryService<T>(Guid serviceGuid) where T : class
            {
                return null;
            }

            public bool ForceLoadUserSettings(string streamName)
            {
                return false;
            }
        }
    }
}
