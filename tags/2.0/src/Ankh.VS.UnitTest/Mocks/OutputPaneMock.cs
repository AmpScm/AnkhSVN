using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Rhino.Mocks;

namespace AnkhSvn_UnitTestProject.Mocks
{
    static class OutputPaneMock
    {
        public static IVsOutputWindow GetServiceInstance(MockRepository mocks)
        {
            using (mocks.Record())
            {
                IVsOutputWindow outputWindow = mocks.DynamicMock<IVsOutputWindow>();
                return outputWindow;
            }
        }
    }
}
