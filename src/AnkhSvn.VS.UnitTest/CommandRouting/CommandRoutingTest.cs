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
using Ankh.Commands.Mapper;
using AnkhSvn.Ids;
using Ankh.Selection;

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


    }
}
