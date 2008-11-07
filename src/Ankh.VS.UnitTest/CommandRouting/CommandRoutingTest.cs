using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

namespace AnkhSvn_UnitTestProject.CommandRouting
{
    static class CommandTester
    {
        public static void TestExecution(AnkhCommand commandEnum)
        {
            AnkhRuntime runtime = new AnkhRuntime(ServiceProviderHelper.serviceProvider);
            runtime.AddModule(new AnkhModule(runtime));
            runtime.Start();

            bool executed = runtime.CommandMapper.Execute(commandEnum, new CommandEventArgs(commandEnum, runtime.Context));
            if (!executed)
                Assert.Inconclusive("Command disabled");
            Assert.IsTrue(executed);
        }
    }
    [TestClass]
    public class CommandRoutingTest
    {
        MockRepository mocks;

        [TestInitialize]
        public void Initialize()
        {
            mocks = new MockRepository();

            IFileStatusCache statusCache = mocks.DynamicMock<IFileStatusCache>();
            ServiceProviderHelper.AddService(typeof(IFileStatusCache), statusCache);
        }

        [TestCleanup]
        public void Cleanup()
        {
            ServiceProviderHelper.serviceProvider.RemoveService(typeof(IFileStatusCache));

            mocks = null;
        }

        [TestMethod]
        public void AddItem()
        {
            mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                // TODO: set-up fake selection
                CommandTester.TestExecution(AnkhCommand.AddItem);
            }
        }

        [TestMethod]
        public void AddRepositoryRoot()
        {
            mocks = new MockRepository();

            IUIShell uiShell = mocks.DynamicMock<IUIShell>();

            //RepositoryRootInfo reposInfo = new RepositoryRootInfo("http://fakeurl/path", 100);

            using (mocks.Record())
            {
                //Expect.Call(uiShell.ShowAddRepositoryRootDialog()).Return(reposInfo).Repeat.Any();
            }

            using (mocks.Playback())
            {
                CommandTester.TestExecution(AnkhCommand.RepositoryBrowse);
            }
        }

        /*[TestMethod]
        public void AddSolutionToRepositoryCommand()
        {
            mocks = new MockRepository();
            
            IContext context = AnkhContextMock.GetInstance(mocks);
            using (mocks.Record())
            {
                // TODO: Selection context
            }

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution(AnkhCommand.AddSolutionToRepository);
            }
        }*/

        [TestMethod]
        public void AddWorkingCopyExplorerRootCommand()
        {
            mocks = new MockRepository();

            IUIShell uiShell = mocks.DynamicMock<IUIShell>();

            using (mocks.Record())
            {
                //Expect.Call(uiShell.ShowAddWorkingCopyExplorerRootDialog()).Return("C:\\something").Repeat.Any();
            }


            using (mocks.Playback())
            {
                CommandTester.TestExecution(AnkhCommand.WorkingCopyBrowse);
            }
        }

        [TestMethod]
        public void BlameCommand()
        {
            mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {

                CommandTester.TestExecution(AnkhCommand.ItemAnnotate);
            }
        }

        [TestMethod, Ignore] // Command shows dialog directly
        public void CheckoutCommand()
        {
            mocks = new MockRepository();

            using (mocks.Playback())
            {
                // TODO: set-up fake selection
                CommandTester.TestExecution(AnkhCommand.Checkout);
            }
        }

        public void CheckoutFolderCommand()
        {
        }

        public void CheckoutSolutionCommand()
        {
        }

        [TestMethod]
        public void CleanupCommand()
        {
            mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.Cleanup);
            }
        }

        [TestMethod]
        public void CommitItemCommand()
        {
            mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.CommitItem);
            }
        }

        [TestMethod]
        public void CopyReposExplorerUrl()
        {
            mocks = new MockRepository();

            using (mocks.Playback())
            {
                CommandTester.TestExecution(AnkhCommand.CopyReposExplorerUrl);
            }
        }

        [TestMethod, Ignore] // shows MessageBox
        public void CreatePatch()
        {
            mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IUIShell uiShell = AnkhUIShellMock.GetInstance(mocks);
            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.CreatePatch);
            }
        }

        [TestMethod]
        public void DiffLocalItem()
        {
            mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IUIShell uiShell = AnkhUIShellMock.GetInstance(mocks);

            Assert.Inconclusive("Diff not verified");

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.DiffLocalItem);
            }
        }

        [TestMethod, Ignore] // Shows dialog
        public void ExportCommand()
        {
            mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IUIShell uiShell = AnkhUIShellMock.GetInstance(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.Export);
            }
        }

        [TestMethod]
        public void ExportFolderCommand()
        {
            mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IUIShell uiShell = AnkhUIShellMock.GetInstance(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.ExportFolder);
            }
        }

        [TestMethod]
        public void LockCommand()
        {
            mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.Lock);
            }
        }

        [TestMethod]
        public void LogCommand()
        {
            mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.Log);
            }
        }

        [TestMethod]
        public void MakeDirectoryCommand()
        {
            mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.NewDirectory);
            }
        }

        [TestMethod]
        public void Refresh()
        {
            mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                // TODO: set-up fake selection
                CommandTester.TestExecution(AnkhCommand.Refresh);
            }
        }

        [TestMethod]
        public void RemoveReposRoot()
        {
            mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.RemoveRepositoryRoot);
            }
        }

        [TestMethod]
        public void RemoveWorkingCopyRoot()
        {
            mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.RemoveWorkingCopyExplorerRoot);
            }
        }       

        [TestMethod]
        public void RevertItemCommand()
        {
            mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.RevertItem);
            }
        }

        [TestMethod]
        public void SaveToFileCommand()
        {
            mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.SaveToFile);
            }
        }

        [TestMethod]
        public void ShowCommitDialog()
        {

            mocks = new MockRepository();
            IAnkhPackage package = mocks.CreateMock<IAnkhPackage>();
            using (mocks.Record())
            {
                package.ShowToolWindow(AnkhToolWindow.PendingChanges);
                LastCall.Repeat.Once();
            }

            using (mocks.Playback())
            {
                CommandTester.TestExecution(AnkhCommand.ShowPendingChanges);
            }
        }

        [TestMethod]
        public void ShowReposExplorer()
        {
            mocks = new MockRepository();
            IAnkhPackage package = mocks.CreateMock<IAnkhPackage>();
            using (mocks.Record())
            {
                package.ShowToolWindow(AnkhToolWindow.RepositoryExplorer);
                LastCall.Repeat.Once();
            }


            using (mocks.Playback())
            {
                CommandTester.TestExecution(AnkhCommand.ShowRepositoryExplorer);
            }
        }

        [TestMethod]
        public void ShowWorkingCopyExplorer()
        {
            mocks = new MockRepository();
            IAnkhPackage package = mocks.CreateMock<IAnkhPackage>();
            using (mocks.Record())
            {
                package.ShowToolWindow(AnkhToolWindow.WorkingCopyExplorer);
                LastCall.Repeat.Once();
            }

            using (mocks.Playback())
            {
                CommandTester.TestExecution(AnkhCommand.ShowWorkingCopyExplorer);
            }
        }

        [TestMethod]
        public void SwitchItemCommand()
        {
            mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.SwitchItem);
            }
        }

        [TestMethod]
        public void UnlockCommand()
        {
            mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.Unlock);
            }
        }

        [TestMethod]
        public void UpdateItemCommand()
        {
            mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.UpdateItem);
            }
        }

        [TestMethod]
        public void ViewInVSNetCommand()
        {
            mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.ViewInVsNet);
            }
        }

        [TestMethod]
        public void ViewInWindowsCommand()
        {
            mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(ISelectionContext), selC))
            {
                CommandTester.TestExecution(AnkhCommand.ViewInWindows);
            }
        }
    }
}
