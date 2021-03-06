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
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;

namespace AnkhSvn_UnitTestProject.Helpers
{
    class ServiceProviderHelper : IDisposable
    {
        internal static OleServiceProvider serviceProvider;
        static ServiceProviderHelper()
        {
            serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
            AddService(typeof(Ankh.UI.IAnkhPackage), AnkhSvn_UnitTestProject.Mocks.PackageMock.EmptyContext(serviceProvider));
        }

        Type type;
        readonly object _instance;
        private ServiceProviderHelper(Type t, object instance)
        {
            type = t;
            _instance = instance;
            serviceProvider.AddService(t, instance, false);
        }

        public void Dispose()
        {
            IDisposable i = _instance as IDisposable;
            if (i != null)
                i.Dispose();

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
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");
            return null;// new ServiceProviderHelper();
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
}
