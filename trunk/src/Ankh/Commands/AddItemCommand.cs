// $Id$
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ankh.UI;
using System.Windows.Forms;
using System.IO;

using SharpSvn;
using Ankh.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to add selected items to the working copy.
    /// </summary>
    [Command(AnkhCommand.AddItem)]
    public class AddItemCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned)
                    continue;
                else if (item.IsVersionable)
                    return; // We found an add item
            }

            e.Visible = e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.GetService<IContext>();
            IUIShell uiShell = e.GetService<IUIShell>();

            SortedList<string, SvnItem> paths = new SortedList<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
            Collection<SvnItem> resources = new Collection<SvnItem>();
            ICollection<SvnItem> addItems = resources;

            PathSelectorInfo info = new PathSelectorInfo("Select items to add",
                e.Selection.GetSelectedSvnItems(true));
            info.CheckedFilter += delegate(SvnItem item) { return !item.IsVersioned; };
            info.VisibleFilter += delegate(SvnItem item) { return true; };

            PathSelectorResult result = null;
            // are we shifted?
            if (!CommandBase.Shift)
            {
                info.EnableRecursive = false;

                result = uiShell.ShowPathSelector(info);

                if (info == null)
                    return;
            }
            else
                result = info.DefaultResult;
                
            addItems = info.CheckedItems;

            if (!result.Succeeded)
                return;

            e.GetService<IProgressRunner>().Run("Adding",
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    SvnAddArgs args = new SvnAddArgs();
                    args.ThrowOnError = false;
                    args.Depth = SvnDepth.Empty;
                    args.AddParents = true;

                    foreach (SvnItem item in addItems)
                    {
                        ee.Client.Add(item.FullPath, args);
                    }
                });
        }
        #endregion

    }
}
