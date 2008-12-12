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
using Ankh.Ids;
using AnkhSvn_UnitTestProject.Mocks;
using Rhino.Mocks;
using AnkhSvn_UnitTestProject.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ankh.VSPackage;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using Ankh;
using System.ComponentModel.Design;
using Ankh.Scc;

namespace UnitTestProject.MenuItemTests
{
    [TestClass()]
    public class MenuItemTest
    {
        /// <summary>
        /// Verify that a new menu command object gets added to the OleMenuCommandService. 
        /// This action takes place In the Initialize method of the Package object
        /// </summary>
        [TestMethod]
        public void InitializeMenuCommand()
        {
            MockRepository mocks = new MockRepository();
            AnkhSvnPackage package = new AnkhSvnPackage();

            IFileStatusCache statusCache = mocks.DynamicMock<IFileStatusCache>();

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(DTE), DteMock.GetDteInstance(mocks)))
            using (ServiceProviderHelper.AddService(typeof(IFileStatusCache), statusCache))
            using (ServiceProviderHelper.SetSite(package))
            {
                //Verify that the menu command can be found
                OleMenuCommandService mcs = ReflectionHelper.InvokeMethod<Package, OleMenuCommandService>(package, "GetService", typeof(IMenuCommandService));
                Assert.IsNotNull(mcs.FindCommand(new CommandID(AnkhId.CommandSetGuid, (int)Ankh.Ids.AnkhCommand.Refresh)));
            }
        }

        [TestMethod]
        public void TestRefreshCommand()
        {
            MockRepository mocks = new MockRepository();
            // Create the package
            AnkhSvnPackage package = new AnkhSvnPackage();

            IFileStatusCache statusCache = mocks.DynamicMock<IFileStatusCache>();

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IFileStatusCache), statusCache))
            using (ServiceProviderHelper.SetSite(package))
            {
                CommandExecutor.ExecuteCommand(package, AnkhCommand.Refresh);
            }
        }
    }
}
