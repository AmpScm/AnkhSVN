using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;
using SharpSvn;
using Ankh.UI.RepositoryExplorer;

namespace Ankh.UI.Commands
{
    public partial class AnnotateDialog : VSDialogForm
    {
        public AnnotateDialog()
        {
            InitializeComponent();
        }

        private void AnnotateDialog_Load(object sender, EventArgs e)
        {
            whitespaceBox.Items.Add(CommandStrings.WhitespaceCompare);
            whitespaceBox.Items.Add(CommandStrings.WhitespaceIgnoreChanges);
            whitespaceBox.Items.Add(CommandStrings.WhitespaceIgnoreAllWhitespace);
            whitespaceBox.SelectedIndex = 1;
        }

        public void SetTargets(IEnumerable<SvnItem> targets)
        {
            List<SvnOrigin> origins = new List<SvnOrigin>();

            foreach (SvnItem i in targets)
                origins.Add(new SvnOrigin(i));

            SetTargets(origins);
        }

        public void SetTargets(List<SvnOrigin> origins)
        {
            foreach (SvnOrigin i in origins)
                targetBox.Items.Add(i);

            if (targetBox.Items.Count > 0)
                targetBox.SelectedIndex = 0;
        }

        public SvnOrigin SelectedTarget
        {
            get { return targetBox.SelectedItem as SvnOrigin; }
        }

        public SvnRevision StartRevision
        {
            get { return startRevision.Revision; }
            set { startRevision.Revision = value; }
        }

        public SvnRevision EndRevision
        {
            get { return toRevision.Revision; }
            set { toRevision.Revision = value; }
        }

        private void targetBox_SelectedValueChanged(object sender, EventArgs e)
        {
            startRevision.SvnOrigin = SelectedTarget;
            toRevision.SvnOrigin = SelectedTarget;
        }

        public bool IgnoreEols
        {
            get { return ignoreEols.Checked; }
            set { ignoreEols.Checked = value; }
        }

        public SvnIgnoreSpacing IgnoreSpacing
        {
            get
            {
                switch (whitespaceBox.SelectedIndex)
                {
                    default:
                    case 0:
                        return SvnIgnoreSpacing.None;
                    case 1:
                        return SvnIgnoreSpacing.IgnoreSpace;
                    case 2:
                        return SvnIgnoreSpacing.IgnoreAll;
                }
            }
        }

        public bool RetrieveMergeInfo
        {
            get { return includeMergeInfo.Checked; }
            set { includeMergeInfo.Checked = value; }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (RepositoryFolderBrowserDialog dlg = new RepositoryFolderBrowserDialog())
            {
                SvnOrigin from = SelectedTarget;

                if (from == null)
                    return;

                dlg.ShowFiles = true;

                SvnUriTarget ut = from.Target as SvnUriTarget;
                if (ut != null)
                    dlg.SelectedUri = ut.Uri;
                else
                {
                    SvnItem file = GetService<IFileStatusCache>()[((SvnPathTarget)from.Target).FullPath];

                    if (file.Uri == null)
                        dlg.SelectedUri = from.RepositoryRoot;
                    else
                        dlg.SelectedUri = file.Uri;
                }

                if (dlg.ShowDialog(Context) == DialogResult.OK)
                {
                    Uri selectedUri = dlg.SelectedUri;

                    SvnOrigin o = new SvnOrigin(Context, selectedUri, null);

                    targetBox.Items.Add(o);
                    targetBox.SelectedItem = o;
                }
            }
        }
    }
}
