/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections;
using System.Text;
using System.Reflection;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ankh.VSPackage;
using EnvDTE;
using AnkhSvn_UnitTestProject.Mocks;
using Rhino.Mocks;
using AnkhSvn_UnitTestProject.Helpers;
using Ankh;
using Ankh.Scc;

namespace UnitTestProject
{
    [TestClass()]
    public class PackageTest
    {
        [TestMethod()]
        public void CreateInstance()
        {
            AnkhSvnPackage package = new AnkhSvnPackage();
        }

        [TestMethod()]
        public void IsIVsPackage()
        {
            AnkhSvnPackage package = new AnkhSvnPackage();
            Assert.IsNotNull(package as IVsPackage, "The object does not implement IVsPackage");
        }

        [TestMethod()]
        public void SetSite()
        {
            MockRepository mocks = new MockRepository();
            // Create the package
            IVsPackage package = new AnkhSvnPackage() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

            IFileStatusCache statusCache = mocks.DynamicMock<IFileStatusCache>();

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), AnkhContextMock.GetInstance(mocks)))
            using (ServiceProviderHelper.AddService(typeof(IFileStatusCache), statusCache))
            using (ServiceProviderHelper.SetSite(package))
            {
                // Unsite the package
                Assert.AreEqual(0, package.SetSite(null), "SetSite(null) did not return S_OK");
            }
        }
    }
}
