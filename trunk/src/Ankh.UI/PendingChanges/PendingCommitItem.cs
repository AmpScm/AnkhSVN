using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;

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
            Text = change.FullPath;
            SubItems.Add(change.Project);
            SubItems.Add(change.Name);
            Checked = true;

            ImageIndex = iconMap.GetIcon(change.FullPath);
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
