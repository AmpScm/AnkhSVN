using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.PendingChanges
{
    partial class RecentChangesPage : PendingChangesPage
    {
        public RecentChangesPage()
        {
            InitializeComponent();
        }

        protected override Type PageType
        {
            get
            {
                return typeof(RecentChangesPage);
            }
        }
    }
}
