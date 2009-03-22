using System;
using NUnit.Framework;
using Ankh.Commands;
using AnkhSvn_UnitTestProject.Helpers;
using Ankh.Ids;
using Ankh;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using Ankh.Selection;
using Ankh.VS;
using Ankh.Scc;
using Microsoft.VisualStudio.TextManager.Interop;
using Ankh.UI;
using Ankh.Diff;

namespace AnkhSvn_UnitTestProject.CommandRouting
{
    [TestFixture]
    public class CommandTests
    {
        AnkhServiceProvider sp;
        CommandMapper cm;

        [SetUp]
        public void SetUp()
        {
            sp = new AnkhServiceProvider();

            object pvar;
            var shell = new Mock<SVsShell>().As<IVsShell>();
            shell.Setup(x => x.GetProperty(It.IsAny<int>(), out pvar)).Returns(-1);
            sp.AddService(typeof(SVsShell), shell.Object);

            var state = new Mock<IAnkhCommandStates>();
            state.SetupGet(x => x.SccProviderActive).Returns(true);
            state.SetupGet(x => x.SolutionExists).Returns(true);

            sp.AddService(typeof(IAnkhCommandStates), state.Object);

            var selection = new Mock<ISelectionContext>();
            selection.Setup(x => x.Cache[It.IsAny<object>()]).Returns(null);

            var rawHandle = new Mock<IVsSccProject2>();
            var p = new SvnProject("c:\foo\bar", rawHandle.Object);
            selection.Setup(x => x.GetSelectedProjects(It.IsAny<bool>())).Returns(new[] { p });
            sp.AddService(typeof(ISelectionContext), selection.Object);


            


            var pcMgr = new Mock<IPendingChangesManager>();
            sp.AddService(typeof(IPendingChangesManager), pcMgr.Object);



            var textMgr = new Mock<SVsTextManager>().As<IVsTextManager>();
            sp.AddService(typeof(SVsTextManager), textMgr.Object);

            var selectionMonitor = new Mock<IVsMonitorSelection>();
            sp.AddService(typeof(IVsMonitorSelection), selectionMonitor.Object);


            var r = new AnkhRuntime(sp);
            r.AddModule(new AnkhModule(r));
            r.AddModule(new AnkhSccModule(r));
            //r.AddModule(new AnkhVSModule(r));
            r.AddModule(new AnkhUIModule(r));
            r.AddModule(new AnkhDiffModule(r));
            r.Start();

            cm = r.GetService<CommandMapper>();
        }

        [Test]
        public void TestOnUpdateProjDirNull()
        {
            var projMapper = new Mock<IProjectFileMapper>();
            var projInfo = new Mock<ISvnProjectInfo>();
            projInfo.SetupGet(x => x.ProjectDirectory).Returns((string)null);
            projMapper.Setup(x => x.GetProjectInfo(It.IsAny<SvnProject>())).Returns(projInfo.Object);
            sp.AddService(typeof(IProjectFileMapper), projMapper.Object);

            // The sln projectroot also returns null
            var slnSettings = new Mock<IAnkhSolutionSettings>();
            slnSettings.SetupGet(x => x.ProjectRoot).Returns((string)null);
            sp.AddService(typeof(IAnkhSolutionSettings), slnSettings.Object);

            TestAllCommands();
        }

        [Test]
        public void TestOnUpdateProjInfoNull()
        {
            var projMapper = new Mock<IProjectFileMapper>();
            projMapper.Setup(x => x.GetProjectInfo(It.IsAny<SvnProject>())).Returns((ISvnProjectInfo)null);
            sp.AddService(typeof(IProjectFileMapper), projMapper.Object);

            // sln settings unavailable
            //var slnSettings = new Mock<IAnkhSolutionSettings>();
            //slnSettings.SetupGet(x => x.ProjectRoot).Returns((string)null);
            //sp.AddService(typeof(IAnkhSolutionSettings), slnSettings.Object);

            TestAllCommands();
        }

        void TestAllCommands()
        {
            AnkhContext context = AnkhContext.Create(sp);

            foreach (AnkhCommand command in Enum.GetValues(typeof(AnkhCommand)))
            {
                var e = new CommandUpdateEventArgs(command, context);

                cm.PerformUpdate(command, e);
            }

            foreach (AnkhCommandMenu m in Enum.GetValues(typeof(AnkhCommandMenu)))
            {
                var e = new CommandUpdateEventArgs((AnkhCommand)m, context);

                cm.PerformUpdate((AnkhCommand)m, e);
            }
        }
    }
}
