using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VsSDK.UnitTestLibrary;
using EnvDTE;
using Rhino.Mocks;

namespace AnkhSvn_UnitTestProject.Mocks
{
    static class DteMock
    {
        /// <summary>
		/// Returns an IVsUiShell that does not implement any methods
		/// </summary>
		/// <returns></returns>
		internal static DTE GetDteInstance(MockRepository mocks)
		{
            DTE dte;
            using(mocks.Record())
            {
                dte = mocks.DynamicMock<DTE>();

                Window window = mocks.DynamicMock<Window>();
                Expect.Call(dte.MainWindow).Return(window).Repeat.Any();

                Events events = mocks.DynamicMock<Events>();

                TaskListEvents tlEvents = mocks.DynamicMock<TaskListEvents>();
                Expect.Call(events.get_TaskListEvents("Conflict")).Return(tlEvents).Repeat.Any();
                Expect.Call(dte.Events).Return(events).Repeat.Any();
                //Version v = new Version(9.0);
                //SetupResult.For(dte.Version).Return(v).Repeat.Any();
            }

            return dte;
		}
    }
}
