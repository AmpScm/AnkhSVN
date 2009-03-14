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

using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Mocks;
using Ankh.VSPackage;
using AnkhSvn_UnitTestProject.Helpers;
using Ankh.Commands;
using Ankh;
using AnkhSvn_UnitTestProject.Mocks;
using Ankh.Ids;
using Ankh.Selection;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.UI.Services;
using Ankh.UI;
using Ankh.Scc;
using NUnit.Framework;
using Moq;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.IO;
using NUnit.Framework.SyntaxHelpers;
using Microsoft.VsSDK.UnitTestLibrary;

namespace AnkhSvn_UnitTestProject.CommandRouting
{
    
    [TestFixture]
    public class CommandRoutingTest
    {
        static class CommandTester
        {
            public static bool TestExecution(AnkhCommand commandEnum)
            {
                AnkhRuntime runtime = new AnkhRuntime(ServiceProviderHelper.serviceProvider);
                runtime.AddModule(new AnkhModule(runtime));
                runtime.Start();

                return runtime.CommandMapper.Execute(commandEnum, new CommandEventArgs(commandEnum, runtime.Context));
            }

            public static bool TestExecution(AnkhCommand commandEnum, object argument)
            {
                AnkhRuntime runtime = new AnkhRuntime(ServiceProviderHelper.serviceProvider);
                runtime.AddModule(new AnkhModule(runtime));
                runtime.Start();

                return runtime.CommandMapper.Execute(commandEnum, new CommandEventArgs(commandEnum, runtime.Context, argument, false, false));
            }
        }

        [SetUp]
        public void Initialize()
        {
            // Create the package
            IVsPackage package = new AnkhSvnPackage() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

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
        public void Cleanup()
        {
            ServiceProviderHelper.DisposeServices();
        }

        [Test]
        public void AddItem()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();

            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.IsFalse(CommandTester.TestExecution(AnkhCommand.AddItem), "Add disabled with empty selection");
            }
        }

        [Test]
        public void AddRepositoryRoot()
        {
            Assert.IsTrue(CommandTester.TestExecution(AnkhCommand.RepositoryBrowse), "Add repository root always enabled");
        }


        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void AddWorkingCopyExplorerRootCommand()
        {
            CommandTester.TestExecution(AnkhCommand.WorkingCopyBrowse);
        }

        [Test, Explicit("Broken")]
        public void AddWorkingCopyExplorerRootCommandWithPath()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.IsTrue(CommandTester.TestExecution(AnkhCommand.WorkingCopyBrowse, Path.GetTempPath()));
            }
        }


        [Test]
        public void BlameCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();

            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.ItemAnnotate), Is.False, 
                    "Blame with empty selection doesn't execute");
            }
        }

        [Test]
        public void CheckoutCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();

            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.Checkout), Is.False,
                    "Checkout doesn't execute with empty selection");
            }
        }

        [Test]
        public void CleanupCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.Cleanup), Is.False,
                    "Cleanup doesn't run without selection");
            }
        }

        [Test]
        public void CommitItemCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.CommitItem), Is.False,
                    "Commit doesn't run without selection");
            }
        }

        [Test]
        public void CopyReposExplorerUrl()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.CopyReposExplorerUrl), Is.False);
            }
        }

        [Test]
        public void CreatePatch()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.CreatePatch), Is.False);
            }
        }

        [Test]
        public void DiffLocalItem()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.DiffLocalItem), Is.False);
            }
        }

        [Test]
        public void ExportCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.Export), Is.False);
            }
        }

        [Test]
        public void LockCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();

            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.Lock), Is.False);
            }
        }

        [Test]
        public void LogCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();

            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.Log), Is.False);
            }
        }

        [Test]
        public void MakeDirectoryCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();

            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.NewDirectory), Is.False);
            }
        }

        [Test]
        public void Refresh()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();

            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.Refresh), Is.True,
                    "Refresh works with empty selection");
            }
        }

        [Test, Explicit]
        public void RemoveReposRoot()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();

            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.RemoveRepositoryRoot), Is.False);
            }
        }

        [Test, Explicit]
        public void RemoveWorkingCopyRoot()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();

            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.RemoveWorkingCopyExplorerRoot), Is.False);
            }
        }       

        [Test]
        public void RevertItemCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();

            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.RevertItem), Is.False,
                    "Cannot revert empty selection");
            }
        }

        [Test]
        public void SaveToFileCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();

            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.SaveToFile), Is.False);
            }
        }

        [Test, Explicit]
        public void ShowCommitDialog()
        {
            var state = new Mock<IAnkhCommandStates>();
            state.SetupGet(x => x.SccProviderActive).Returns(true);

            var uiShell = new Mock<SVsUIShell>().As<IVsUIShell>();

            using (ServiceProviderHelper.AddService(typeof(SVsUIShell), uiShell.Object))
            using (ServiceProviderHelper.AddService(typeof(IAnkhCommandStates), state.Object))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.ShowPendingChanges), Is.True);
            }
        }

        [Test, Explicit]
        public void ShowReposExplorer()
        {
            //IAnkhPackage package = mocks.CreateMock<IAnkhPackage>();
            //using (mocks.Record())
            //{
            //    package.ShowToolWindow(AnkhToolWindow.RepositoryExplorer);
            //    LastCall.Repeat.Once();
            //}


            //using (mocks.Playback())

            CommandTester.TestExecution(AnkhCommand.ShowRepositoryExplorer);

        }

        [Test, Explicit]
        public void ShowWorkingCopyExplorer()
        {
            //IAnkhPackage package = mocks.CreateMock<IAnkhPackage>();
            //using (mocks.Record())
            //{
            //    package.ShowToolWindow(AnkhToolWindow.WorkingCopyExplorer);
            //    LastCall.Repeat.Once();
            //}

            //using (mocks.Playback())
            //{
                CommandTester.TestExecution(AnkhCommand.ShowWorkingCopyExplorer);
            //}
        }

        [Test]
        public void SwitchItemCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.SwitchItem), Is.False);
            }
        }

        [Test]
        public void UnlockCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.Unlock), Is.False);
            }
        }

        [Test]
        public void UpdateItemCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.UpdateItemSpecific), Is.False);
            }
        }

        [Test]
        public void ViewInVSNetCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.ViewInVsNet), Is.False);
            }
        }

        [Test]
        public void ViewInWindowsCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                Assert.That(CommandTester.TestExecution(AnkhCommand.ViewInWindows), Is.False);
            }
        }
    }
}
