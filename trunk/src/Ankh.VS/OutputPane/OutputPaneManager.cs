// $Id$
//
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
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using Microsoft.VisualStudio;

namespace Ankh.VS.OutputPane
{
    [GlobalService(typeof(IOutputPaneManager))]
    class OutputPaneManager : AnkhService, IOutputPaneManager
    {
        IVsOutputWindow _window;
        Guid g = new Guid(AnkhId.AnkhOutputPaneId);

        public OutputPaneManager(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IVsOutputWindow Window
        {
            get { return _window ?? (_window = GetService<IVsOutputWindow>(typeof(SVsOutputWindow))); }
        }

        public void WriteToPane(string s)
        {           
            IVsOutputWindowPane pane;
            ErrorHandler.ThrowOnFailure(Window.GetPane(ref g, out pane));            
            ErrorHandler.ThrowOnFailure(pane.OutputString(s));
        }
    }
}
