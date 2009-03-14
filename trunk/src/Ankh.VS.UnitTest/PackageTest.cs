// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.VSPackage;
using EnvDTE;
using AnkhSvn_UnitTestProject.Mocks;
using AnkhSvn_UnitTestProject.Helpers;
using Ankh;
using Ankh.Scc;
using NUnit.Framework;
using Moq;

namespace UnitTestProject
{
    [TestFixture]
    public class PackageTest
    {
        [Test]
        public void CreateInstance()
        {
            AnkhSvnPackage package = new AnkhSvnPackage();
        }

        [Test]
        public void IsIVsPackage()
        {
            AnkhSvnPackage package = new AnkhSvnPackage();
            Assert.IsNotNull(package as IVsPackage, "The object does not implement IVsPackage");
        }

        [Test, Explicit("Init problems")]
        public void SetSite()
        {
            // Create the package
            IVsPackage package = new AnkhSvnPackage() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");
            
            var statusCache = new Mock<IFileStatusCache>();
            var regEditors = new Mock<SVsRegisterEditors>().As<IVsRegisterEditors>();

            using (ServiceProviderHelper.AddService(typeof(SVsRegisterEditors), regEditors.Object))
            using (ServiceProviderHelper.AddService(typeof(IFileStatusCache), statusCache.Object))
            using (ServiceProviderHelper.SetSite(package))
            {
                // Unsite the package
                Assert.AreEqual(0, package.SetSite(null), "SetSite(null) did not return S_OK");
            }
        }
    }
}
