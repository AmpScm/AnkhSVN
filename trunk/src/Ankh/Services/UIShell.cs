// $Id$
//
// Copyright 2004-2009 The AnkhSVN Project
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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.IO;

using SharpSvn;

using Ankh.UI;
using Ankh.VS;
using Ankh.UI.SccManagement;
using Ankh.UI.PathSelector;

namespace Ankh
{
    /// <summary>
    /// Summary description for UIShell.
    /// </summary>
    [GlobalService(typeof(IUIShell))]
    sealed class UIShell : AnkhService, IUIShell
    {
        public UIShell(IAnkhServiceProvider context)
            : base(context)
        {

        }

        public PathSelectorResult ShowPathSelector(PathSelectorInfo info)
        {
            using (PathSelector selector = new PathSelector(info))
            {
                selector.Context = Context;

                bool succeeded = selector.ShowDialog(Context) == DialogResult.OK;
                PathSelectorResult result = new PathSelectorResult(succeeded, selector.CheckedItems);
                result.Depth = selector.Recursive ? SvnDepth.Infinity : SvnDepth.Empty;
                result.RevisionStart = selector.RevisionStart;
                result.RevisionEnd = selector.RevisionEnd;
                return result;
            }
        }

    }
}
