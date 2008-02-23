using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.UI
{
    public partial class LogDialogTest : Form
    {
        public LogDialogTest()
        {
            InitializeComponent();
        }

        public Uri RemoteTarget
        {
            get { return logRevisionControl.RemoteTarget; }
            set { logRevisionControl.RemoteTarget = value; }
        }

        public string LocalTarget
        {
            get { return logRevisionControl.LocalTarget; }
            set { logRevisionControl.LocalTarget = value; }
        }

        public static void Main(string[] args)
        {
            LogDialogTest dialog = new LogDialogTest();
            dialog.RemoteTarget = new Uri("http://ankhsvn.open.collab.net/svn/ankhsvn/");
            dialog.ShowDialog();
            
        }

        private void toolStripButton1_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer2.Panel2Collapsed = !cbLogMessageViewer.Checked;
            splitContainer1.Panel2Collapsed = !(cbLogMessageViewer.Checked || cbChangedPathsViewer.Checked);
        }

        private void cbChangedPathsViewer_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel1Collapsed = !cbChangedPathsViewer.Checked;
            splitContainer1.Panel2Collapsed = !(cbLogMessageViewer.Checked || cbChangedPathsViewer.Checked);
        }

        private void cbIncludeMerged_Click(object sender, EventArgs e)
        {
            logRevisionControl.IncludeMergedRevisions = cbIncludeMerged.Checked;
        }

        private void cbStopOnCopy_Click(object sender, EventArgs e)
        {
            logRevisionControl.StrictNodeHistory = cbStopOnCopy.Checked;
        }
    }
}