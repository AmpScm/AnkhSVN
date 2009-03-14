using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Moq;

namespace AnkhSvn_UnitTestProject.Mocks
{
    static class VsShellMock
    {
        public static IVsShell GetInstance()
        {
            var shell = new Mock<SVsShell>().As<IVsShell2>().As<IVsShell>();

            object r;
            shell.Setup(x => x.GetProperty((int)__VSSPROPID.VSSPROPID_VirtualRegistryRoot, out r)).Returns(-123123);

            return shell.Object;
        }
    }
}
