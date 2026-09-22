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
    [SvnCommand(AnkhCommand.Lock)]
    [SvnCommand(AnkhCommand.LockMustLock)]
    [SvnCommand(AnkhCommand.SccLock)]
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

                if (LockCommandLogic.IsLockCandidate(
                        item.IsFile,
                        item.IsVersioned,
                        item.IsNewAddition,
                        item.IsLocked))
                {
                    return;
                }
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IEnumerable<SvnItem> items = GetRequestedItems(e);
            if (items == null || EnumTools.IsEmpty(items))
                return;

            bool stealLocks = false;
            string comment = "";

            AnkhConfig config =
                e.GetService<IAnkhConfigurationService>().Instance;

            if (LockCommandLogic.ShouldPrompt(
                    e.DontPrompt,
                    e.PromptUser,
                    Shift,
                    config.SuppressLockingUI))
            {
                if (!TryPromptForLockOptions(
                        e,
                        items,
                        out items,
                        out stealLocks,
                        out comment))
                {
                    return;
                }
            }

            ICollection<string> files = SvnItem.GetPaths(items);
            if (files.Count == 0)
                return;

            SortedList<string, string> alreadyLockedFiles =
                RunLock(e, files, stealLocks, comment);

            if (alreadyLockedFiles.Count == 0)
                return;

            string message = LockCommandLogic.BuildAlreadyLockedMessage(
                CommandStrings.ItemsAlreadyLocked,
                CommandStrings.ItemFileLocked,
                alreadyLockedFiles);

            if (ConfirmStealLocks(e, message))
                RunStealLock(e, files, comment);
        }

        IEnumerable<SvnItem> GetRequestedItems(CommandEventArgs e)
        {
            IEnumerable<SvnItem> items =
                e.Argument as IEnumerable<SvnItem>;

            if (items != null)
                return items;

            if (e.Command == AnkhCommand.SccLock)
                return null;

            List<SvnItem> choices = new List<SvnItem>();
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (LockCommandLogic.IsLockCandidate(
                        item.IsFile,
                        item.IsVersioned,
                        item.IsNewAddition,
                        item.IsLocked))
                {
                    choices.Add(item);
                }
            }

            return choices;
        }

        static bool TryPromptForLockOptions(
            CommandEventArgs e,
            IEnumerable<SvnItem> items,
            out IEnumerable<SvnItem> selectedItems,
            out bool stealLocks,
            out string comment)
        {
            selectedItems = items;
            stealLocks = false;
            comment = "";

            using (LockDialog dlg = new LockDialog())
            {
                dlg.Context = e.Context;
                dlg.LoadItems(items);

                if (dlg.ShowDialog(e.Context) != DialogResult.OK)
                    return false;

                selectedItems =
                    new List<SvnItem>(dlg.GetCheckedItems());
                stealLocks = dlg.StealLocks;
                comment = dlg.Message;
                return true;
            }
        }

        SortedList<string, string> RunLock(
            CommandEventArgs e,
            ICollection<string> files,
            bool stealLocks,
            string comment)
        {
            SortedList<string, string> alreadyLockedFiles =
                new SortedList<string, string>(
                    StringComparer.OrdinalIgnoreCase);

            e.GetService<IProgressRunner>().RunModal(
                CommandStrings.LockingTitle,
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    SvnLockArgs args = new SvnLockArgs();
                    args.StealLock = stealLocks;
                    args.Comment = comment;
                    args.AddExpectedError(
                        SvnErrorCode.SVN_ERR_FS_PATH_ALREADY_LOCKED);
                    args.Notify +=
                        delegate(object nSender, SvnNotifyEventArgs notifyArgs)
                        {
                            if (notifyArgs.Action
                                != SvnNotifyAction.LockFailedLock)
                            {
                                return;
                            }

                            string owner =
                                LockCommandLogic.GetLockOwner(
                                    notifyArgs.Lock != null
                                        ? notifyArgs.Lock.Owner
                                        : null,
                                    notifyArgs.Error.Message);

                            alreadyLockedFiles.Add(
                                notifyArgs.FullPath,
                                owner);
                        };

                    ee.Client.Lock(files, args);
                });

            return alreadyLockedFiles;
        }

        static bool ConfirmStealLocks(
            CommandEventArgs e,
            string message)
        {
            AnkhMessageBox box = new AnkhMessageBox(e.Context);
            return box.Show(
                    message,
                    "",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2)
                == DialogResult.Yes;
        }

        static void RunStealLock(
            CommandEventArgs e,
            ICollection<string> files,
            string comment)
        {
            e.GetService<IProgressRunner>().RunModal(
                CommandStrings.LockingTitle,
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    SvnLockArgs args = new SvnLockArgs();
                    args.StealLock = true;
                    args.Comment = comment;
                    ee.Client.Lock(files, args);
                });
        }

    }
}
