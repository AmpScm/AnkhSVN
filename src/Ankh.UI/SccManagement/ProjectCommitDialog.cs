using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;
using Ankh.UI.PendingChanges;

namespace Ankh.UI.SccManagement
{
    public partial class ProjectCommitDialog : VSContainerForm
    {
        public ProjectCommitDialog()
        {
            InitializeComponent();
        }

        IEnumerable<PendingChange> _changeEnumerator;

        public void LoadChanges(IEnumerable<Ankh.Scc.PendingChange> changeWalker)
        {
            if (changeWalker == null)
                throw new ArgumentNullException("changeWalker");

            if (pendingList.Context == null && Context != null)
            {
                pendingList.Context = Context;
                pendingList.SelectionPublishServiceProvider = Context;
            }

            _changeEnumerator = changeWalker;
            Reload();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            logMessage.Select();
        }

        private void Reload()
        {
            Dictionary<PendingChange, PendingCommitItem> chk = null;

            if (pendingList.Items.Count > 0)
            {
                chk = new Dictionary<PendingChange, PendingCommitItem>();

                foreach (PendingCommitItem it in pendingList.Items)
                {
                    chk.Add(it.PendingChange, it);
                    it.Group = null;
                }
            }

            pendingList.Items.Clear();
            pendingList.Groups.Clear();

            foreach (PendingChange pc in _changeEnumerator)
            {
                PendingCommitItem pi;
                if (chk != null && chk.TryGetValue(pc, out pi))
                {
                    pendingList.Items.Add(pi);
                    pi.RefreshText(Context);
                }
                else
                    pendingList.Items.Add(new PendingCommitItem(pendingList, pc));
            }
        }

        public IEnumerable<PendingChange> GetSelection()
        {
            foreach (PendingCommitItem it in pendingList.Items)
            {
                if (it.Checked)
                    yield return it.PendingChange;
            }
        }

        public void FillArgs(PendingChangeCommitArgs pca)
        {
            pca.LogMessage = logMessage.Text;
            pca.KeepLocks = keepLocksBox.Checked;
            pca.KeepChangeLists = keepChangelistsBox.Checked;
        }
    }
}
