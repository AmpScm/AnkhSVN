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
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Ankh;
using Ankh.Commands;
using Ankh.Scc;
using Ankh.Selection;
using Ankh.Services;
using Ankh.UI;
using Ankh.UI.Services;
using Ankh.VSPackage;
using AnkhSvn_UnitTestProject.Helpers;
using AnkhSvn_UnitTestProject.Mocks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Moq;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.CommandRouting
{
    [TestFixture]
    public class CommandRoutingTest
    {
        static class CommandTester
        {
            public static bool TestExecution(AnkhCommand commandEnum)
            {
                AnkhRuntime runtime = CreateCommandRuntime();

                return runtime.CommandMapper.Execute(commandEnum, new CommandEventArgs(commandEnum, runtime.Context));
            }

            public static bool TestExecution(AnkhCommand commandEnum, object argument)
            {
                AnkhRuntime runtime = CreateCommandRuntime();

                return runtime.CommandMapper.Execute(commandEnum, new CommandEventArgs(commandEnum, runtime.Context, argument, false, false));
            }

            public static bool TestEnabled(AnkhCommand commandEnum)
            {
                AnkhRuntime runtime = CreateCommandRuntime();
                CommandUpdateEventArgs args = new CommandUpdateEventArgs(commandEnum, runtime.Context);

                Assert.That(runtime.CommandMapper.PerformUpdate(commandEnum, args), Is.True,
                    "Expected the command to be registered");

                return args.Enabled;
            }

            static AnkhRuntime CreateCommandRuntime()
            {
                // These are command-routing unit tests, not package/runtime integration tests.
                // Load command handlers directly instead of starting all Ankh services
                // (update checks, registry-backed services, schedulers, etc.).
                AnkhRuntime runtime = new AnkhRuntime(ServiceProviderHelper.serviceProvider);
                runtime.CommandMapper.LoadFrom(typeof(AnkhModule).Assembly);
                runtime.CommandMapper.LoadFrom(typeof(AnkhUIModule).Assembly);
                return runtime;
            }
        }

        [SetUp]
        public void Initialize()
        {
            // Command-routing tests only require the Ankh package service contract.
            // Constructing the real AsyncPackage outside Visual Studio requires
            // ThreadHelper.JoinableTaskContext and is therefore an integration concern.
            IAnkhPackage package = (IAnkhPackage)PackageMock.EmptyContext(ServiceProviderHelper.serviceProvider);

            var statusCache = new Mock<ISvnStatusCache>();
            var regEditors = new Mock<SVsRegisterEditors>().As<IVsRegisterEditors>();

            var vsShell = new Mock<SVsShell>().As<IVsShell>();
            object r = @"SOFTWARE\Microsoft\VisualStudio\17.0";
            vsShell.Setup(x => x.GetProperty((int)__VSSPROPID.VSSPROPID_VirtualRegistryRoot, out r)).Returns(VSErr.S_OK);

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
            ServiceProviderHelper.AddService(typeof(ISvnStatusCache), statusCache.Object);

            var commandStates = new Mock<IAnkhCommandStates>();
            commandStates.SetupGet(x => x.SccProviderActive).Returns(true);
            ServiceProviderHelper.AddService(typeof(IAnkhCommandStates), commandStates.Object);

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
                Assert.IsFalse(CommandTester.TestExecution(AnkhCommand.AddItem), "Add disabled with empty selection");
        }

        [Test]
        public void AddRepositoryRoot()
        {
            Assert.That(CommandTester.TestEnabled(AnkhCommand.RepositoryBrowse), Is.True,
                "Repository Browse is declared AlwaysAvailable");
        }

        [Test]
        public void AddWorkingCopyExplorerRootCommand()
        {
            Assert.Throws<InvalidOperationException>(() => CommandTester.TestExecution(AnkhCommand.WorkingCopyBrowse));
        }

        [Test]
        public void AddWorkingCopyExplorerRootCommandWithPath()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            var package = new Mock<IAnkhPackage>();

            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            using (ServiceProviderHelper.AddService(typeof(IAnkhPackage), package.Object))
                Assert.IsTrue(CommandTester.TestExecution(AnkhCommand.WorkingCopyBrowse, Path.GetTempPath()));

            package.Verify(p => p.ShowToolWindow(AnkhToolWindow.WorkingCopyExplorer), Times.Once);
        }

        [Test]
        public void BlameCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.ItemAnnotate), Is.False,
                    "Blame with empty selection doesn't execute");
        }

        [Test]
        public void CheckoutCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.Checkout), Is.False,
                    "Checkout doesn't execute with empty selection");
        }

        [Test]
        public void CleanupCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.Cleanup), Is.False,
                    "Cleanup doesn't run without selection");
        }

        [Test]
        public void CommitItemCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.CommitItem), Is.False,
                    "Commit doesn't run without selection");
        }

        [Test]
        public void CopyReposExplorerUrl()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.CopyReposExplorerUrl), Is.False);
        }

        [Test]
        public void CreatePatch()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.CreatePatch), Is.False);
        }

        [Test]
        public void DiffLocalItem()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.DiffLocalItem), Is.False);
        }

        [Test]
        public void ExportCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.Export), Is.False);
        }

        [Test]
        public void LockCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.Lock), Is.False);
        }

        [Test]
        public void LogCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.Log), Is.False);
        }

        [Test]
        public void MakeDirectoryCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.NewFolder), Is.False);
        }

        [Test]
        public void Refresh()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestEnabled(AnkhCommand.Refresh), Is.False,
                    "Refresh is disabled when there are no selected files");
        }

        [Test]
        public void RemoveReposRoot()
        {
            Assert.That(CommandTester.TestEnabled(AnkhCommand.RemoveRepositoryRoot), Is.False,
                "Remove Repository Root must be disabled when no repository explorer root is selected.");
        }

        [Test]
        public void RemoveWorkingCopyRoot()
        {
            Assert.That(CommandTester.TestEnabled(AnkhCommand.RemoveWorkingCopyExplorerRoot), Is.False,
                "Remove Working Copy Root must be disabled when no working-copy explorer root is selected.");
        }

        [Test]
        public void RevertItemCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.RevertItem), Is.False,
                    "Cannot revert empty selection");
        }

        [Test]
        public void SaveToFileCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.SaveToFile), Is.False);
        }

        [Test]
        public void ShowCommitDialog()
        {
            var state = new Mock<IAnkhCommandStates>();
            state.SetupGet(x => x.SccProviderActive).Returns(true);
            var package = new Mock<IAnkhPackage>();

            using (ServiceProviderHelper.AddService(typeof(IAnkhCommandStates), state.Object))
            using (ServiceProviderHelper.AddService(typeof(IAnkhPackage), package.Object))
                Assert.That(CommandTester.TestExecution(AnkhCommand.ShowPendingChanges), Is.True);

            package.Verify(p => p.ShowToolWindow(AnkhToolWindow.PendingChanges), Times.Once);
        }

        [Test]
        public void ShowReposExplorer()
        {
            var package = new Mock<IAnkhPackage>();
            var settings = new Mock<IAnkhSolutionSettings>();
            settings.SetupGet(x => x.ProjectRootUri).Returns((Uri)null);

            using (ServiceProviderHelper.AddService(typeof(IAnkhPackage), package.Object))
            using (ServiceProviderHelper.AddService(typeof(IAnkhSolutionSettings), settings.Object))
                Assert.That(CommandTester.TestExecution(AnkhCommand.ShowRepositoryExplorer), Is.True);

            package.Verify(p => p.ShowToolWindow(AnkhToolWindow.RepositoryExplorer), Times.Once);
        }

        [Test]
        public void ShowWorkingCopyExplorer()
        {
            var package = new Mock<IAnkhPackage>();

            using (ServiceProviderHelper.AddService(typeof(IAnkhPackage), package.Object))
                Assert.That(CommandTester.TestExecution(AnkhCommand.ShowWorkingCopyExplorer), Is.True);

            package.Verify(p => p.ShowToolWindow(AnkhToolWindow.WorkingCopyExplorer), Times.Once);
        }

        [Test]
        public void SwitchItemCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.SwitchItem), Is.False);
        }

        [Test]
        public void UnlockCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.Unlock), Is.False);
        }

        [Test]
        public void UpdateItemCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.UpdateItemSpecific), Is.False);
        }

        [Test]
        public void ViewInVSNetCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.ViewInVsNet), Is.False);
        }

        [Test]
        public void ViewInWindowsCommand()
        {
            ISelectionContext selC = SelectionContextMock.EmptyContext();
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
                Assert.That(CommandTester.TestExecution(AnkhCommand.ViewInWindows), Is.False);
        }
    }
}
