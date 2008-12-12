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
