using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.OLE.Interop;
using AnkhSvn.Ids;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace AnkhSvn_UnitTestProject.Helpers
{
    static class CommandExecutor
    {
        public static void ExecuteCommand(IVsPackage package, AnkhCommand command)
        {
            Guid cmdId = AnkhId.CommandSetGuid;
            int hr = ((IOleCommandTarget)package).Exec(ref cmdId, (uint)command, (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, IntPtr.Zero, IntPtr.Zero);
            Assert.IsTrue(hr == VSConstants.S_OK);
        }
    }
}
