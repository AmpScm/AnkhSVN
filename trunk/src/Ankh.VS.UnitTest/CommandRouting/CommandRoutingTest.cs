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
using AnkhSvn.Ids;
using Ankh.Selection;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.UI.Services;
using Ankh.UI;

namespace AnkhSvn_UnitTestProject.CommandRouting
{
    static class CommandTester
    {
        public static void TestExecution<TCommand>(AnkhCommand commandEnum)
        {
            AnkhSvnPackage package = new AnkhSvnPackage();
            using (ServiceProviderHelper.SetSite(package))
            {
                CommandMapItem cmi = package.CommandMapper[commandEnum];
                Assert.IsNotNull(cmi);
                Assert.AreEqual<AnkhCommand>(commandEnum, cmi.Command);

                bool executed = false;
                bool updated = false;
                cmi.Execute += delegate { executed = true; };
                cmi.Update += delegate
                {
                    Assert.IsNotNull(cmi.ICommand);
                    Assert.IsInstanceOfType(cmi.ICommand, typeof(TCommand));
                    updated = true;
                };

                CommandExecutor.ExecuteCommand(package, commandEnum);
                Assert.IsTrue(executed);
                Assert.IsTrue(updated);
            }
        }
    }
    [TestClass]
    public class CommandRoutingTest
    {
        [TestMethod]
        public void AddItem()
        {
            MockRepository mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                // TODO: set-up fake selection
                CommandTester.TestExecution<AddItemCommand>(AnkhCommand.AddItem);
            }
        }

        [TestMethod]
        public void AddRepositoryRoot()
        {
            MockRepository mocks = new MockRepository();

            IUIShell uiShell = mocks.DynamicMock<IUIShell>();

            RepositoryRootInfo reposInfo = new RepositoryRootInfo("http://fakeurl/path", 100);

            using (mocks.Record())
            {
                Expect.Call(uiShell.ShowAddRepositoryRootDialog()).Return(reposInfo).Repeat.Any();
            }

            IContext context = AnkhContextMock.GetInstance(mocks, uiShell);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<AddRepositoryRootCommand>(AnkhCommand.AddRepositoryRoot);
            }
        }

        [TestMethod]
        public void AddSolutionToRepositoryCommand()
        {
            MockRepository mocks = new MockRepository();
            
            IContext context = AnkhContextMock.GetInstance(mocks);
            using (mocks.Record())
            {
                // TODO: Selection context
            }

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<AddSolutionToRepositoryCommand>(AnkhCommand.AddSolutionToRepository);
            }
        }

        [TestMethod]
        public void AddWorkingCopyExplorerRootCommand()
        {
            MockRepository mocks = new MockRepository();

            IUIShell uiShell = mocks.DynamicMock<IUIShell>();

            using (mocks.Record())
            {
                Expect.Call(uiShell.ShowAddWorkingCopyExplorerRootDialog()).Return("C:\\something").Repeat.Any();
            }

            IContext context = AnkhContextMock.GetInstance(mocks, uiShell);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<AddWorkingCopyExplorerRootCommand>(AnkhCommand.AddWorkingCopyExplorerRoot);
            }
        }

        [TestMethod]
        public void BlameCommand()
        {
            MockRepository mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                
                CommandTester.TestExecution<BlameCommand>(AnkhCommand.Blame);
            }
        }

        [TestMethod, Ignore] // Command shows dialog directly
        public void CheckoutCommand()
        {
            MockRepository mocks = new MockRepository();

            IContext context = AnkhContextMock.GetInstance(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                // TODO: set-up fake selection
                CommandTester.TestExecution<CheckoutCommand>(AnkhCommand.Checkout);
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
            MockRepository mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);
            
            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<Cleanup>(AnkhCommand.Cleanup);
            }
        }

        [TestMethod]
        public void CommitItemCommand()
        {
            MockRepository mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using(mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<CommitItemCommand>(AnkhCommand.CommitItem);
            }
        }

        [TestMethod]
        public void CopyReposExplorerUrl()
        {
            MockRepository mocks = new MockRepository();

            IContext context = AnkhContextMock.GetInstance(mocks);
            using(mocks.Playback())
            using(ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<CopyReposExplorerUrl>(AnkhCommand.CopyReposExplorerUrl);
            }
        }

        [TestMethod, Ignore] // shows MessageBox
        public void CreatePatch()
        {
            MockRepository mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IUIShell uiShell = AnkhUIShellMock.GetInstance(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, uiShell, selC);
            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<CreatePatchCommand>(AnkhCommand.CreatePatch);
            }
        }

        [TestMethod]
        public void DiffExternalLocalItem()
        {
            MockRepository mocks = new MockRepository();

            IContext context = AnkhContextMock.GetInstance(mocks);
            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<DiffExternalLocalItem>(AnkhCommand.DiffExternalLocalItem);
            }
        }

        [TestMethod]
        public void DiffLocalItem()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IUIShell uiShell = AnkhUIShellMock.GetInstance(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, uiShell, selC);
            
            Assert.Inconclusive("Diff not verified");

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<DiffLocalItem>(AnkhCommand.DiffLocalItem);
            }
        }

        [TestMethod, Ignore] // Shows dialog
        public void ExportCommand()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IUIShell uiShell = AnkhUIShellMock.GetInstance(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, uiShell, selC);


            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<ExportCommand>(AnkhCommand.Export);
            }
        }

        [TestMethod] 
        public void ExportFolderCommand()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IUIShell uiShell = AnkhUIShellMock.GetInstance(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, uiShell, selC);


            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<ExportFolderCommand>(AnkhCommand.ExportFolder);
            }
        }

        [TestMethod]
        public void LockCommand()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<LockCommand>(AnkhCommand.Lock);
            }
        }

        [TestMethod]
        public void LogCommand()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<LogCommand>(AnkhCommand.Log);
            }
        }

        [TestMethod]
        public void MakeDirectoryCommand()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<MakeDirectoryCommand>(AnkhCommand.NewDirectory);
            }
        }

        [TestMethod]
        public void Refresh()
        {
            MockRepository mocks = new MockRepository();

            IContext context = AnkhContextMock.GetInstance(mocks);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                // TODO: set-up fake selection
                CommandTester.TestExecution<RefreshCommand>(AnkhCommand.Refresh);
            }
        }

        [TestMethod]
        public void RelocateCommand()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<RelocateCommand>(AnkhCommand.Relocate);
            }
        }

        [TestMethod]
        public void RemoveReposRoot()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<RemoveRepositoryRootCommand>(AnkhCommand.RemoveRepositoryRoot);
            }
        }

        [TestMethod]
        public void RemoveWorkingCopyRoot()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<RemoveWorkingCopyExplorerRootCommand>(AnkhCommand.RemoveWorkingCopyExplorerRoot);
            }
        }

        [TestMethod]
        public void ResolveConflictCommand()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<ResolveConflictCommand>(AnkhCommand.ResolveConflict);
            }
        }

        [TestMethod]
        public void ResolveConflictExternalCommand()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<ResolveConflictExternalCommand>(AnkhCommand.ResolveConflictExternal);
            }
        }

        [TestMethod]
        public void ReverseMergeCommand()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<ReverseMergeCommand>(AnkhCommand.RevertToRevision);
            }
        }

        [TestMethod]
        public void RevertItemCommand()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<RevertItemCommand>(AnkhCommand.RevertItem);
            }
        }

        [TestMethod]
        public void SaveToFileCommand()
        {
            MockRepository mocks = new MockRepository();
            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<SaveToFileCommand>(AnkhCommand.SaveToFile);
            }
        }

        [TestMethod]
        public void SendErrorReportCommand()
        {
            MockRepository mocks = new MockRepository();

            IErrorHandler errorHandler = mocks.DynamicMock<IErrorHandler>();
            using (mocks.Record())
            {
                errorHandler.SendReport();
                LastCall.Repeat.Once();
            }

            IContext context = AnkhContextMock.GetInstance(mocks);
            mocks.BackToRecord(context, BackToRecordOptions.None);
            Expect.Call(context.ErrorHandler).Return(errorHandler).Repeat.AtLeastOnce();
            mocks.Replay(context);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<SendErrorReportCommand>(AnkhCommand.SendFeedback);
            }
        }

        [TestMethod]
        public void ShowCommitDialog()
        {
            
            MockRepository mocks = new MockRepository();
            IAnkhPackage package = mocks.CreateMock<IAnkhPackage>();
            using (mocks.Record())
            {
                package.ShowToolWindow(AnkhToolWindow.PendingChanges);
                LastCall.Repeat.Once();
            }
            
            IContext context = AnkhContextMock.GetInstance(mocks);
            mocks.BackToRecord(context);
            Expect.Call(context.Package).Return(package).Repeat.AtLeastOnce();
            mocks.Replay(context);
            
            
            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<ShowCommitDialogCommand>(AnkhCommand.ShowCommitDialog);
            }
        }

        [TestMethod]
        public void ShowReposExplorer()
        {
            MockRepository mocks = new MockRepository();
            IAnkhPackage package = mocks.CreateMock<IAnkhPackage>();
            using (mocks.Record())
            {
                package.ShowToolWindow(AnkhToolWindow.RepositoryExplorer);
                LastCall.Repeat.Once();
            }

            IContext context = AnkhContextMock.GetInstance(mocks);
            mocks.BackToRecord(context);
            Expect.Call(context.Package).Return(package).Repeat.AtLeastOnce();
            mocks.Replay(context);


            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<ShowRepositoryExplorerCommand>(AnkhCommand.ShowRepositoryExplorer);
            }
        }

        [TestMethod]
        public void ShowWorkingCopyExplorer()
        {
            MockRepository mocks = new MockRepository();
            IAnkhPackage package = mocks.CreateMock<IAnkhPackage>();
            using (mocks.Record())
            {
                package.ShowToolWindow(AnkhToolWindow.WorkingCopyExplorer);
                LastCall.Repeat.Once();
            }

            IContext context = AnkhContextMock.GetInstance(mocks);
            mocks.BackToRecord(context);
            Expect.Call(context.Package).Return(package).Repeat.AtLeastOnce();
            mocks.Replay(context);


            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<ShowWorkingCopyExplorerCommand>(AnkhCommand.ShowWorkingCopyExplorer);
            }
        }

        [TestMethod]
        public void SwitchItemCommand()
        {
            MockRepository mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<SwitchItemCommand>(AnkhCommand.SwitchItem);
            }
        }

        [TestMethod]
        public void UnlockCommand()
        {
            MockRepository mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<UnlockCommand>(AnkhCommand.Unlock);
            }
        }

        [TestMethod]
        public void UpdateItemCommand()
        {
            MockRepository mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<UpdateItem>(AnkhCommand.UpdateItem);
            }
        }

        [TestMethod]
        public void ViewInVSNetCommand()
        {
            MockRepository mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<ViewInVSNetCommand>(AnkhCommand.ViewInVsNet);
            }
        }

        [TestMethod]
        public void ViewInWindowsCommand()
        {
            MockRepository mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                CommandTester.TestExecution<ViewInWindowsCommand>(AnkhCommand.ViewInWindows);
            }
        }

        [TestMethod]
        public void ViewRepositoryFileCommand()
        {
            MockRepository mocks = new MockRepository();

            ISelectionContext selC = SelectionContextMock.EmptyContext(mocks);
            IContext context = AnkhContextMock.GetInstance(mocks, selC);

            using (mocks.Playback())
            using (ServiceProviderHelper.AddService(typeof(IContext), context))
            {
                //CommandTester.TestExecution<ViewRepositoryFileCommand>(AnkhCommand.ViewRepositoryFile);
            }
        }
    }
}
