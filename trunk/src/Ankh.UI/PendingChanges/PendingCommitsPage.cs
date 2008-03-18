using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.PendingChanges
{
    public partial class PendingCommitsPage : PendingChangesPage
    {
        public PendingCommitsPage()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        protected override void OnUISiteChanged()
        {
            base.OnUISiteChanged();

            logMessageEditor.Init(UISite);
        }
    }
}
