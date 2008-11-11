using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;

namespace Ankh.UI.PathSelector
{
    public partial class ChangeSetSelector : UserControl
    {
        public ChangeSetSelector()
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

        }
    }
}
