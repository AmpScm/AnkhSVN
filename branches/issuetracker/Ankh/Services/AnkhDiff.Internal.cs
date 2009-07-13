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
using Ankh.Scc.UI;
using Ankh.UI;
using Ankh.UI.DiffWindow;
using Ankh.Selection;

namespace Ankh.Services
{
    partial class AnkhDiff
    {
        readonly List<int> _freeDiffs = new List<int>();
        int _nNext;
        
        private bool RunInternalDiff(AnkhDiffArgs args)
        {
            IAnkhPackage pkg = GetService<IAnkhPackage>();

            int nWnd;

            if (_freeDiffs.Count > 0)
            {
                nWnd = _freeDiffs[0];
                _freeDiffs.RemoveAt(0);
            }
            else
                nWnd = _nNext++;

            pkg.ShowToolWindow(AnkhToolWindow.Diff, nWnd, true);

            DiffToolWindowControl twc = GetService<ISelectionContext>().ActiveFrameControl as DiffToolWindowControl;

            if (twc != null)
                twc.Reset(nWnd, args);

            return false;
        }

        void IAnkhDiffHandler.ReleaseDiff(int frame)
        {
            if(!_freeDiffs.Contains(frame))
                _freeDiffs.Add(frame);
        }
    }
}
