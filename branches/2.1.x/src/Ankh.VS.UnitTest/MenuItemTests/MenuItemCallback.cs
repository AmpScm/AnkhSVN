// $Id$
//
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

/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using AnkhSvn_UnitTestProject.Mocks;
using AnkhSvn_UnitTestProject.Helpers;
using Ankh.VSPackage;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using Ankh;
using System.ComponentModel.Design;
using Ankh.Scc;
using NUnit.Framework;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Ankh.UI;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using Microsoft.VisualStudio;

namespace UnitTestProject.MenuItemTests
{
    [TestFixture]
    public class MenuItemTest
    {
        AnkhSvnPackage package;

        [SetUp]
        public void SetUp()
        {
            // Create the package
            package = new AnkhSvnPackage();

            var statusCache = new Mock<IFileStatusCache>();
            var regEditors = new Mock<SVsRegisterEditors>().As<IVsRegisterEditors>();

            var vsShell = new Mock<SVsShell>().As<IVsShell>();
            object r = @"SOFTWARE\Microsoft\VisualStudio\8.0";
            vsShell.Setup(x => x.GetProperty((int)__VSSPROPID.VSSPROPID_VirtualRegistryRoot, out r)).Returns(VSConstants.S_OK);

            var vsTextMgr = new Mock<SVsTextManager>().As<IVsTextManager>();

            var monitorSelection = new Mock<IVsMonitorSelection>();

            var olMgr = new Mock<SOleComponentManager>().As<IOleComponentManager>();

            var outputWindow = new Mock<SVsOutputWindow>().As<IVsOutputWindow>();

            ServiceProviderHelper.AddService(typeof(IAnkhPackage), package);
            ServiceProviderHelper.AddService(typeof(SVsOutputWindow), outputWindow.Object);
            ServiceProviderHelper.AddService(typeof(SOleComponentManager), olMgr.Object);
            ServiceProviderHelper.AddService(typeof(IVsMonitorSelection), monitorSelection.Object);
            ServiceProviderHelper.AddService(typeof(SVsTextManager), vsTextMgr.Object);
            ServiceProviderHelper.AddService(typeof(SVsShell), vsShell.Object);
            ServiceProviderHelper.AddService(typeof(SVsRegisterEditors), regEditors.Object);
            ServiceProviderHelper.AddService(typeof(IFileStatusCache), statusCache.Object);

            var uiService = new Mock<IUIService>();
            uiService.Setup(x => x.ShowDialog(It.IsAny<Form>())).Returns(DialogResult.OK);

            ServiceProviderHelper.AddService(typeof(IUIService), uiService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceProviderHelper.DisposeServices();
        }
        /// <summary>
        /// Verify that a new menu command object gets added to the OleMenuCommandService. 
        /// This action takes place In the Initialize method of the Package object
        /// </summary>
        [Test]
        public void InitializeMenuCommand()
        {
            using (ServiceProviderHelper.SetSite(package))
            {
                //Verify that the menu command can be found
                OleMenuCommandService mcs = ReflectionHelper.InvokeMethod<Package, OleMenuCommandService>(package, "GetService", typeof(IMenuCommandService));
                Assert.IsNotNull(mcs.FindCommand(new CommandID(AnkhId.CommandSetGuid, (int)Ankh.AnkhCommand.Refresh)));
            }
        }
    }
}
