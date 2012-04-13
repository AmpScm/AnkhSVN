// $Id$
//
// Copyright 2005-2009 The AnkhSVN Project
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
using System.Windows.Forms;
using SharpSvn;
using Ankh.Configuration;
using Ankh.UI;
using Ankh.UI.SccManagement;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to lock the selected item.
    /// </summary>
    [Command(AnkhCommand.Lock)]
    [Command(AnkhCommand.LockMustLock)]
    [Command(AnkhCommand.SccLock)]
    class LockCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Command == AnkhCommand.SccLock)
                return; // Always enabled

            bool mustOnly = (e.Command == AnkhCommand.LockMustLock);
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (mustOnly && !item.IsReadOnlyMustLock)
                    continue;

                if (item.IsFile && item.IsVersioned && !item.IsNewAddition && !item.IsLocked)
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IEnumerable<SvnItem> items = e.Argument as IEnumerable<SvnItem>;

            if (e.Command == AnkhCommand.SccLock && items == null)
                return;

            if (items == null)
            {
                List<SvnItem> choices = new List<SvnItem>();
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                {
                    if (item.IsFile && item.IsVersioned && !item.IsNewAddition && !item.IsLocked)
                        choices.Add(item);
                }

                items = choices;
            }

            if (EnumTools.IsEmpty(items))
                return;

            bool stealLocks = false;
            string comment = "";

            AnkhConfig config = e.GetService<IAnkhConfigurationService>().Instance;

            if (!e.DontPrompt && (e.PromptUser || (!Shift || !config.SuppressLockingUI)))
            {
                using (LockDialog dlg = new LockDialog())
                {
                    dlg.Context = e.Context;
                    dlg.LoadItems(items);

                    if (dlg.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    items = new List<SvnItem>(dlg.GetCheckedItems());
                    stealLocks = dlg.StealLocks;
                    comment = dlg.Message;
                }
            }

            ICollection<string> files = SvnItem.GetPaths(items);

            if (files.Count == 0)
                return;

            SortedList<string, string> alreadyLockedFiles = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
            e.GetService<IProgressRunner>().RunModal(
                CommandStrings.LockingTitle,
                 delegate(object sender, ProgressWorkerArgs ee)
                 {
                     SvnLockArgs la = new SvnLockArgs();
                     la.StealLock = stealLocks;
                     la.Comment = comment;
                     la.AddExpectedError(SvnErrorCode.SVN_ERR_FS_PATH_ALREADY_LOCKED);
                     la.Notify += delegate(object nSender, SvnNotifyEventArgs notifyArgs)
                                      {
                                          if (notifyArgs.Action == SvnNotifyAction.LockFailedLock)
                                          {
                                              string userName = "?";

                                              if (notifyArgs.Lock != null)
                                                  userName = notifyArgs.Lock.Owner;

                                              alreadyLockedFiles.Add(notifyArgs.FullPath, userName);
                                          }
                                      };
                     ee.Client.Lock(files, la);
                 });

            if (alreadyLockedFiles.Count == 0)
                return;

            StringBuilder msg = new StringBuilder();
            msg.AppendLine(CommandStrings.ItemsAlreadyLocked);
            msg.AppendLine();

            foreach (KeyValuePair<string, string> kv in alreadyLockedFiles)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                    msg.AppendFormat(CommandStrings.ItemFileLocked, kv.Key, kv.Value);
                else
                    msg.Append(kv.Key);
                msg.AppendLine();
            }

            // TODO: Create a dialog where the user can select what locks to steal, and also what files are already locked.
            AnkhMessageBox box = new AnkhMessageBox(e.Context);
            DialogResult rslt = box.Show(
                msg.ToString().TrimEnd(),
                "",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (rslt == DialogResult.Yes)
            {
                e.GetService<IProgressRunner>().RunModal(
                    CommandStrings.LockingTitle,
                     delegate(object sender, ProgressWorkerArgs ee)
                     {
                         SvnLockArgs la = new SvnLockArgs();
                         la.StealLock = true;
                         la.Comment = comment;
                         ee.Client.Lock(files, la);
                     });
            }
        }
    }
}
