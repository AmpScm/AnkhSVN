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
using SharpSvn;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.Scc;
using Ankh.Scc.UI;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogCompareWithWorkingCopy, AlwaysAvailable = true)]
    public class CompareWithWorkingCopy : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            ISvnLogItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());

            if (item != null)
            {
                ILogControl logWindow = e.Selection.ActiveDialogOrFrameControl as ILogControl;

                if (logWindow != null)
                {
                    SvnOrigin origin = EnumTools.GetSingle(logWindow.Origins);

                    if (origin != null)
                    {
                        SvnPathTarget pt = origin.Target as SvnPathTarget;

                        if (pt != null)
                        {
                            SvnItem svnItem = e.GetService<IFileStatusCache>()[pt.FullPath];

                            if (svnItem != null && !svnItem.IsDirectory)
                                return;
                        }
                    }
                }
            }

            e.Enabled = false;
        }
   
        public void OnExecute(CommandEventArgs e)
        {
            // All checked in OnUpdate            
            ILogControl logWindow = (ILogControl)e.Selection.ActiveDialogOrFrameControl;
            SvnOrigin origin = EnumTools.GetSingle(logWindow.Origins);
            ISvnLogItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());

            IAnkhDiffHandler diff = e.GetService<IAnkhDiffHandler>();

            AnkhDiffArgs da = new AnkhDiffArgs();
            da.BaseFile = diff.GetTempFile(origin.Target, item.Revision, true);
            if (da.BaseFile == null)
                return; // User cancel
            da.MineFile = ((SvnPathTarget)origin.Target).FullPath;
            da.BaseTitle = string.Format("Base (r{0})", item.Revision);
            da.MineTitle = "Mine/Working";

            diff.RunDiff(da);
        }
    }
}
