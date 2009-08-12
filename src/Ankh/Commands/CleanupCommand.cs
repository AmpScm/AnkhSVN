// $Id$
//
// Copyright 2003-2009 The AnkhSVN Project
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
using SharpSvn;
using Ankh.Scc;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to cleanup the working copy.
    /// </summary>
    [Command(AnkhCommand.Cleanup)]
    class Cleanup : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned)
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            List<SvnItem> items = new List<SvnItem>(e.Selection.GetSelectedSvnItems(true));

            e.GetService<IProgressRunner>().RunModal("Running Cleanup",
                delegate(object sender, ProgressWorkerArgs a)
                {
                    HybridCollection<string> wcs = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

                    foreach (SvnItem item in items)
                    {
                        if (!item.IsVersioned)
                            continue;

                        SvnWorkingCopy wc = item.WorkingCopy;

                        if (wc != null && !wcs.Contains(wc.FullPath))
                            wcs.Add(wc.FullPath);
                    }

                    SvnCleanUpArgs args = new SvnCleanUpArgs();
                    args.ThrowOnError = false;
                    foreach (string path in wcs)
                        a.Client.CleanUp(path, args);
                });
        }
    }
}
