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

using System.Collections.Generic;
using Ankh.Scc;
using SharpSvn;
using Ankh.UI.Commands;
using System.Windows.Forms;

namespace Ankh.Commands
{
    [Command(AnkhCommand.MoveToNewChangeList)]
    [Command(AnkhCommand.MoveToIgnoreChangeList)]
    [Command(AnkhCommand.RemoveFromChangeList)]
    [Command(AnkhCommand.MoveToExistingChangeList0, LastCommand=AnkhCommand.MoveToExistingChangeListMax)]
    class ItemMoveToChangeList : CommandBase
    {
        const string IgnoreOnCommit = "ignore-on-commit";
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Command == AnkhCommand.RemoveFromChangeList)
            {
                OnUpdateRemove(e);
                return;
            }
            List<string> names = (List<string>)e.Selection.Cache[typeof(ItemMoveToChangeList)];

            bool found = false;

            if(names != null)
                found = true; // We have cached names -> We have a selection
            else
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                {
                    if (item.IsFile && item.IsVersioned && (item.IsModified || item.IsDocumentDirty))
                    {
                        found = true;
                        break;
                    }
                }

            bool inRange = (e.Command >= AnkhCommand.MoveToExistingChangeList0 && e.Command < AnkhCommand.MoveToExistingChangeListMax);

            if (!found)
            {
                e.Enabled = false;
                if(inRange)
                    e.DynamicMenuEnd = true;

                return;
            }

            if (!inRange)
                return;

            if (names == null)
                names = GetRecentNames(e);

            int n = e.Command - AnkhCommand.MoveToExistingChangeList0;

            if(n >= names.Count)
            {
                e.Enabled = e.Visible = false;
                e.DynamicMenuEnd = true;
            }
            else
            {
                e.Text = names[n].Replace("&", "&&");
            }
        }

        private static List<string> GetRecentNames(BaseCommandEventArgs e)
        {
            List<string> names = (List<string>)e.Selection.Cache[typeof(ItemMoveToChangeList)];

            if (names != null)
                return names;

            SortedList<string, string> nm = new SortedList<string, string>();

            foreach (PendingChange pc in e.GetService<IPendingChangesManager>().GetAll())
            {
                string cl = pc.Item.Status.ChangeList;

                if (!string.IsNullOrEmpty(cl) && !string.Equals(cl, IgnoreOnCommit))
                {
                    if (!nm.ContainsKey(cl))
                        nm[cl] = cl;

                    if (nm.Count >= 10)
                        break; // We have enough items
                }
            }

            e.Selection.Cache[typeof(ItemMoveToChangeList)] = names = new List<string>(nm.Values);

            return names;
        }

        private static void OnUpdateRemove(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsVersioned && !string.IsNullOrEmpty(item.Status.ChangeList))
                {
                    return;
                }
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            string name;

            if (e.Command == AnkhCommand.MoveToIgnoreChangeList)
                name = IgnoreOnCommit;
            else if (e.Command == AnkhCommand.RemoveFromChangeList)
                name = null;
            else if (e.Command >= AnkhCommand.MoveToExistingChangeList0 && e.Command < AnkhCommand.MoveToExistingChangeListMax)
            {
                List<string> names = GetRecentNames(e);

                int n = e.Command - AnkhCommand.MoveToExistingChangeList0;

                if (n >= names.Count)
                    return;

                name = names[n];
            }
            else using(CreateChangeListDialog dlg = new CreateChangeListDialog())
            {
                if(DialogResult.OK != dlg.ShowDialog(e.Context))
                    return;

                name = dlg.ChangeListName;
            }

            if(string.IsNullOrEmpty(name))
                name = null;

            List<string> paths = new List<string>();
            
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsVersioned && (name != null) ? (item.IsFile || item.IsModified || item.IsDocumentDirty) : !string.IsNullOrEmpty(item.Status.ChangeList))
                {
                    paths.Add(item.FullPath);
                }
            }

            if (paths.Count == 0)
                return;

            e.Selection.Cache.Remove(typeof(ItemMoveToChangeList)); // Remove cached list of items

            using (SvnClient cl = e.GetService<ISvnClientPool>().GetNoUIClient())
            {
                if (name == null)
                {
                    SvnRemoveFromChangeListArgs ra = new SvnRemoveFromChangeListArgs();
                    ra.ThrowOnError = false;
                    cl.RemoveFromChangeList(paths, ra);
                }
                else
                {
                    SvnAddToChangeListArgs ca = new SvnAddToChangeListArgs();
                    ca.ThrowOnError = false;
                    cl.AddToChangeList(paths, name, ca);
                }

                // The svn client broadcasts glyph updates to fix our UI
            }
        }
    }
}
