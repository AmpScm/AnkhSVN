using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

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

        bool _createdEditor;
        protected override void OnUISiteChanged()
        {
            base.OnUISiteChanged();

            if (!_createdEditor)
            {
                //UISite.
                IOleServiceProvider sp = (IOleServiceProvider)UISite.GetService(typeof(IOleServiceProvider));

                if (sp != null)
                    logMessageEditor.Init(sp);
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _createdEditor = false;
            base.OnHandleDestroyed(e);
        }
    }
}
