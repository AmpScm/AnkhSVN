using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;
using System.Globalization;

namespace Ankh.UI.PathSelector
{
    partial class RevisionSelector : UserControl
    {
        public RevisionSelector()
        {
            InitializeComponent();
        }

        IAnkhServiceProvider _context;
        SvnOrigin _origin;

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; EnableBrowse(); }
        }

        /// <summary>
        /// Gets or sets the SVN origin.
        /// </summary>
        /// <value>The SVN origin.</value>
        public SvnOrigin SvnOrigin
        {
            get { return _origin; }
            set { _origin = value; EnableBrowse(); }
        }

        void EnableBrowse()
        {
            browseButton.Enabled = (SvnOrigin != null) && (Context != null);
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (LogViewerDialog lvd = new LogViewerDialog(SvnOrigin, Context))
            {
                if (DialogResult.OK != lvd.ShowDialog(Context))
                    return;

                ISvnLogItem li = EnumTools.GetSingle(lvd.SelectedItems);

                if (li == null)
                    return;

                Revision = li.Revision;
            }
        }

        public event EventHandler Changed;

        public long? Revision
        {
            get
            {
                long rev;
                string text = revisionBox.Text;
                if (string.IsNullOrEmpty(text))
                    return null;

                if (long.TryParse(text.Trim(), out rev))
                    return rev;
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                    revisionBox.Text = value.Value.ToString(CultureInfo.InvariantCulture);
                else
                    revisionBox.Text = "";
            }
        }

        private void revisionBox_TextChanged(object sender, EventArgs e)
        {
            OnChanged(e);
        }

        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }
    }
}
