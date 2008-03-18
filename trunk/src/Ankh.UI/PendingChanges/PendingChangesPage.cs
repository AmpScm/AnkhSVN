using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Services;

namespace Ankh.UI.PendingChanges
{
    public partial class PendingChangesPage : UserControl
    {
        IAnkhUISite _uiSite;
        public PendingChangesPage()
        {
            InitializeComponent();
        }

        public IAnkhUISite UISite
        {
            get { return _uiSite; }
            set
            {
                _uiSite = value;

                if (value != null)
                    OnUISiteChanged();
            }
        }

        protected virtual void OnUISiteChanged()
        {

        }
    }
}
