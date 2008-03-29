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

        public PendingCommitItem(PendingChange change)
        {
            if (change == null)
                throw new ArgumentNullException("change");

            _change = change;
            this.Text = _change.FullPath;
            this.SubItems.Add(change.Project);
            //this.SubItems[1] 
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
