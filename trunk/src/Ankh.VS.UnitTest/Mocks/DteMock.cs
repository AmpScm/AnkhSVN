// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
