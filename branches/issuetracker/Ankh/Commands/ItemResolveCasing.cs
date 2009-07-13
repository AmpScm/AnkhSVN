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
using Ankh.Scc;
using SharpSvn;
using System.IO;
using Microsoft.VisualStudio.Shell;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemResolveCasing)]
    class ItemResolveCasing : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsCasingConflicted)
                {
                    // Ok, something we can fix!
                    return;
                }
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            List<SvnItem> toResolve = new List<SvnItem>();

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsCasingConflicted)
                {
                    toResolve.Add(item);
                }
            }
            try
            {
                foreach (SvnItem item in toResolve)
                {
                    string svnPath = GetSvnCasing(item);
                    string actualPath = SvnTools.GetFullTruePath(item.FullPath);

                    if (svnPath == null || actualPath == null)
                        continue; // not found
                    if (!string.Equals(svnPath, actualPath, StringComparison.OrdinalIgnoreCase))
                        continue; // More than casing rename

                    string svnName = Path.GetFileName(svnPath);
                    string actualName = Path.GetFileName(actualPath);

                    if (svnName == actualName)
                        continue; // Can't fix directories!

                    IAnkhOpenDocumentTracker odt = e.GetService<IAnkhOpenDocumentTracker>();
                    using (odt.LockDocument(svnPath, DocumentLockType.NoReload))
                    using (odt.LockDocument(actualPath, DocumentLockType.NoReload))
                    {
                        try
                        {
                            // Try the actual rename
                            File.Move(actualPath, svnPath);
                        }
                        catch { }

                        try
                        {
                            // And try to fix the project+document system
                            VsShellUtilities.RenameDocument(e.Context, actualPath, svnPath);
                        }
                        catch
                        { }
                    }
                }
            }
            finally
            {
                e.GetService<IFileStatusMonitor>().ScheduleSvnStatus(SvnItem.GetPaths(toResolve));
            }
        }

        static string GetSvnCasing(SvnItem item)
        {
            string name = null;
            // Find the correct casing
            using (SvnWorkingCopyClient wcc = new SvnWorkingCopyClient())
            {
                SvnWorkingCopyEntriesArgs ea = new SvnWorkingCopyEntriesArgs();
                ea.ThrowOnCancel = false;
                ea.ThrowOnError = false;

                wcc.ListEntries(item.Directory, ea,
                    delegate(object sender, SvnWorkingCopyEntryEventArgs e)
                    {
                        if (string.Equals(e.FullPath, item.FullPath, StringComparison.OrdinalIgnoreCase))
                        {
                            name = e.FullPath;
                        }
                    });
            }

            return name;
        }

    }
}
