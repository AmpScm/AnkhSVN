// $Id$
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ankh.UI;
using System.Windows.Forms;
using System.IO;

using SharpSvn;
using AnkhSvn.Ids;

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
            IContext context = e.Context.GetService<IContext>();

            SortedList<string, SvnItem> paths = new SortedList<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
            IList resources = new Collection<SvnItem>();

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned)
                    continue;
                else if (item.IsVersionable)
                {
                    if (paths.ContainsKey(item.Path))
                        continue;

                    paths.Add(item.Path, item);
                    resources.Add(item);
                }
            }

            // are we shifted?
            if (!CommandBase.Shift)
            {
                PathSelectorInfo info = new PathSelectorInfo("Select items to add",
                    resources, resources);
                info.EnableRecursive = false;

                info = context.UIShell.ShowPathSelector(info);

                if (info == null)
                    return;

                resources = info.CheckedItems;
            }

            using(context.StartOperation("Adding"))
            {
                SvnAddArgs args = new SvnAddArgs();
                args.ThrowOnError = false;
                args.Depth = SvnDepth.Empty;
                args.AddParents = true;

                foreach (SvnItem item in resources)
                {
                    context.Client.Add(item.Path, args);
                }
                context.Selection.RefreshSelection();
            }
        }
        #endregion

    }
}
