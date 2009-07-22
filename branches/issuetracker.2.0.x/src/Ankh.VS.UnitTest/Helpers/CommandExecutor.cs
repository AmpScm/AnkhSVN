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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.OLE.Interop;
using Ankh.Ids;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;

namespace AnkhSvn_UnitTestProject.Helpers
{
    static class CommandExecutor
    {
        public static void ExecuteCommand(IVsPackage package, AnkhCommand command)
        {
            Guid cmdId = AnkhId.CommandSetGuid;
            int hr = ((IOleCommandTarget)package).Exec(ref cmdId, (uint)command, (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, IntPtr.Zero, IntPtr.Zero);
            if (hr == (int)OLEConstants.OLECMDERR_E_DISABLED)
                Assert.Inconclusive("Command is disabled");
            else
                Assert.IsTrue(hr == VSConstants.S_OK);
        }
    }
}
