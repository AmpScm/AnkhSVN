using System;
using Ankh;
using Rhino.Mocks;

namespace AnkhSvn_UnitTestProject.Mocks
{
    static class AnkhUIShellMock
    {
        public static IUIShell GetInstance(MockRepository mocks)
        {
            IUIShell shell = mocks.DynamicMock<IUIShell>();
            return shell;
        }
    }
}
