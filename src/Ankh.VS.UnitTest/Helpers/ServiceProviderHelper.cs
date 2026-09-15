// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Helpers
{
    class ServiceProviderHelper : IDisposable
    {
        internal static readonly TestOleServiceProvider serviceProvider;

        static ServiceProviderHelper()
        {
            serviceProvider = new TestOleServiceProvider();
            AddService(typeof(Ankh.UI.IAnkhPackage), AnkhSvn_UnitTestProject.Mocks.PackageMock.EmptyContext(serviceProvider));
        }

        Type type;
        readonly object _instance;

        private ServiceProviderHelper(Type t, object instance)
        {
            type = t;
            _instance = instance;
            serviceProvider.AddService(t, instance);
        }

        public void Dispose()
        {
            IDisposable disposable = _instance as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            if (type != null && serviceProvider.GetService(type) != null)
                serviceProvider.RemoveService(type);
        }

        public static IDisposable AddService(Type t, object instance)
        {
            _types.Add(t);
            return new ServiceProviderHelper(t, instance);
        }

        static readonly List<Type> _types = new List<Type>();

        public static IDisposable SetSite(IVsPackage package)
        {
            Assert.AreEqual(VSConstants.S_OK, package.SetSite(serviceProvider), "SetSite did not return S_OK");
            return null;
        }

        internal static void DisposeServices()
        {
            foreach (Type t in _types)
            {
                if (serviceProvider.GetService(t) != null)
                    serviceProvider.RemoveService(t);
            }

            _types.Clear();
        }
    }

    /// <summary>
    /// Minimal managed OLE service provider for package unit tests.
    /// Replaces the retired Microsoft.VsSDK.UnitTestLibrary OleServiceProvider
    /// while preserving IVsPackage.SetSite behavior.
    /// </summary>
    sealed class TestOleServiceProvider : System.IServiceProvider, Microsoft.VisualStudio.OLE.Interop.IServiceProvider
    {
        readonly Dictionary<Type, object> _servicesByType = new Dictionary<Type, object>();
        readonly Dictionary<Guid, object> _servicesByGuid = new Dictionary<Guid, object>();

        public void AddService(Type serviceType, object instance)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            _servicesByType[serviceType] = instance;
            _servicesByGuid[serviceType.GUID] = instance;
        }

        public void RemoveService(Type serviceType)
        {
            if (serviceType == null)
                return;

            _servicesByType.Remove(serviceType);
            _servicesByGuid.Remove(serviceType.GUID);
        }

        public object GetService(Type serviceType)
        {
            object service;
            return serviceType != null && _servicesByType.TryGetValue(serviceType, out service)
                ? service
                : null;
        }

        public int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject)
        {
            ppvObject = IntPtr.Zero;

            object service;
            if (!_servicesByGuid.TryGetValue(guidService, out service) || service == null)
                return VSConstants.E_NOINTERFACE;

            IntPtr unknown = IntPtr.Zero;
            try
            {
                unknown = Marshal.GetIUnknownForObject(service);
                return Marshal.QueryInterface(unknown, ref riid, out ppvObject);
            }
            finally
            {
                if (unknown != IntPtr.Zero)
                    Marshal.Release(unknown);
            }
        }
    }
}
