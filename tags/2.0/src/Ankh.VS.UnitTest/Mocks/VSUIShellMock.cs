using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Rhino.Mocks;
using Microsoft.VisualStudio;

namespace AnkhSvn_UnitTestProject.Mocks
{
    static class VSUIShellMock
    {
        public static IVsUIShell GetInstance(MockRepository mocks)
        {
            Guid solutionExplorerGuid = new Guid("3ae79031-e1bc-11d0-8f78-00a0c9110057"); // GUID_SolutionExplorer from vsshell.h
            IVsWindowFrame solutionExplorer;

            IVsUIShell shell = mocks.DynamicMock<IVsUIShell>();
            IVsWindowFrame slnFrame = mocks.DynamicMock<IVsWindowFrame>();
            using (mocks.Record())
            {
                Expect.Call(shell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref solutionExplorerGuid, out solutionExplorer))
                    .OutRef(slnFrame)
                    .Return(VSConstants.S_OK)
                    .Repeat.Any();
            }
            return shell;
        }
    }
}
