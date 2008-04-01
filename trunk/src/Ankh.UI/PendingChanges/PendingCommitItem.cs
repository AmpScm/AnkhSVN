﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;
using Ankh.VS;

namespace Ankh.UI.PendingChanges
{
    class PendingCommitItem : ListViewItem
    {
        readonly PendingChange _change;

        public PendingCommitItem(IAnkhServiceProvider context, PendingChange change, FileIconMap iconMap)
        {
            if (change == null)
                throw new ArgumentNullException("change");

            _change = change;

            while (SubItems.Count < 6)
                SubItems.Add("");

            Checked = true;

            RefreshText(context, iconMap);
        }

        public void RefreshText(IAnkhServiceProvider context, FileIconMap iconMap)
        {
            IAnkhSolutionSettings solSet = context.GetService<IAnkhSolutionSettings>();
            IFileStatusCache cache = context.GetService<IFileStatusCache>();


            string start = solSet.ProjectRoot;

            if (FullPath.StartsWith(start, StringComparison.OrdinalIgnoreCase))
                Text = FullPath.Substring(start.Length).Replace('\\', '/');
            else
                Text = FullPath;

            SubItems[1].Text = PendingChange.Project;

            SvnItem item = cache[FullPath];

            if (item == null)
                throw new InvalidOperationException(); // Item no longer valued

            SharpSvn.SvnStatus ps = item.Status.LocalContentStatus;

            if (ps == SharpSvn.SvnStatus.NotVersioned)
                SubItems[2].Text = "New";
            else if (ps == SharpSvn.SvnStatus.Normal)
                SubItems[2].Text = "";
            else
                SubItems[2].Text = item.Status.LocalContentStatus.ToString();
            ps = item.Status.LocalPropertyStatus;

            if (ps == SharpSvn.SvnStatus.Normal || ps == SharpSvn.SvnStatus.None)
                SubItems[3].Text = "";
            else
                SubItems[3].Text = item.Status.LocalPropertyStatus.ToString();
            SubItems[4].Text = item.FullPath;

            ImageIndex = iconMap.GetIcon(FullPath);

            System.Drawing.Color clr = System.Drawing.Color.Black;

            if(item.IsConflicted)
                clr = System.Drawing.Color.Red;
            else if(item.IsDeleteScheduled)
                clr = System.Drawing.Color.DarkRed;
            else if(item.Status.IsCopied || item.Status.CombinedStatus == SharpSvn.SvnStatus.Added || !item.IsVersioned)
                clr = System.Drawing.Color.FromArgb(255,100,0,100);
            else if(item.IsModified)
                clr = System.Drawing.Color.DarkBlue;

            ForeColor = clr;
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <value>The full path.</value>
        public PendingChange PendingChange
        {
            get { return _change; }
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath
        {
            get { return _change.FullPath; }
        }
    }
}
