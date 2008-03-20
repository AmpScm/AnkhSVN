using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System.ComponentModel.Design;

namespace Ankh.UI.PendingChanges
{
    partial class PendingCommitsPage : PendingChangesPage
    {
        public PendingCommitsPage()
        {
            InitializeComponent();
        }    

        bool _createdEditor;
        protected override void OnUISiteChanged()
        {
            base.OnUISiteChanged();

            if (!_createdEditor)
            {
                //UISite.
                IOleServiceProvider sp = UISite.GetService<IOleServiceProvider>();

                if (sp != null)
                {
                    logMessageEditor.Init(sp);
                    UISite.CommandTarget = logMessageEditor.CommandTarget;
                    _createdEditor = true;
                }
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            _createdEditor = false;
        }

        protected override Type PageType
        {
            get
            {
                return typeof(PendingCommitsPage);
            }
        }

        public bool LogMessageVisible
        {
            get { return !splitContainer.Panel1Collapsed; }
            set { splitContainer.Panel1Collapsed = !value; }
        }
    }
}
