// Copyright 2009 The AnkhSVN Project
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

using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Moq;
using NUnit.Framework;

using Ankh;
using Ankh.Commands;
using Ankh.Collections;
using Ankh.Scc;
using Ankh.Selection;
using Ankh.UI;
using Ankh.VS;
using AnkhSvn_UnitTestProject.Helpers;
using AnkhSvn_UnitTestProject.Mocks;
using Ankh.Services;

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
            sp.AddService(typeof(IAnkhPackage), PackageMock.EmptyContext(sp));

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
            var p = new SccProject("c:\foo\bar", rawHandle.Object);
            selection.Setup(x => x.GetSelectedProjects(It.IsAny<bool>())).Returns(new[] { p });
            sp.AddService(typeof(ISelectionContext), selection.Object);


            


            var pendingChangesInner = new Mock<IKeyedNotifyCollection<string, PendingChange>>();
            pendingChangesInner.SetupGet(x => x.Count).Returns(0);

            var pcMgr = new Mock<IPendingChangesManager>();
            pcMgr.SetupGet(x => x.PendingChanges)
                .Returns(new PendingChangeCollection(pendingChangesInner.Object));
            sp.AddService(typeof(IPendingChangesManager), pcMgr.Object);



            var textMgr = new Mock<SVsTextManager>().As<IVsTextManager>();
            sp.AddService(typeof(SVsTextManager), textMgr.Object);

            var selectionMonitor = new Mock<IVsMonitorSelection>();
            sp.AddService(typeof(IVsMonitorSelection), selectionMonitor.Object);


            var r = new AnkhRuntime(sp);

            // This fixture verifies that command update handlers tolerate incomplete
            // project metadata. Loading the command assemblies is sufficient; starting
            // the full extension runtime initializes unrelated registry/network/UI
            // services and turns this unit test into an environment-dependent integration test.
            r.CommandMapper.LoadFrom(typeof(AnkhModule).Assembly);
            r.CommandMapper.LoadFrom(typeof(AnkhSccModule).Assembly);
            r.CommandMapper.LoadFrom(typeof(AnkhUIModule).Assembly);

            cm = r.CommandMapper;
        }

        [Test]
        public void TestOnUpdateProjDirNull()
        {
            var projMapper = new Mock<IProjectFileMapper>();
            var projInfo = new Mock<ISccProjectInfo>();
            projInfo.SetupGet(x => x.ProjectDirectory).Returns((string)null);
            projMapper.Setup(x => x.GetProjectInfo(It.IsAny<SccProject>())).Returns(projInfo.Object);
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
            projMapper.Setup(x => x.GetProjectInfo(It.IsAny<SccProject>())).Returns((ISccProjectInfo)null);
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
