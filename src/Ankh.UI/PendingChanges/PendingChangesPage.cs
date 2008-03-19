using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Services;
using System.ComponentModel.Design;

namespace Ankh.UI.PendingChanges
{
    partial class PendingChangesPage : UserControl
    {
        IAnkhUISite _uiSite;
        bool _registered;
        public PendingChangesPage()
        {
            InitializeComponent();
        }

        protected virtual Type PageType
        {
            get { return null; }
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
            Register(true);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
 	        base.OnHandleCreated(e);
            Register(true);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            Register(false);
        }

        IServiceContainer _container;
        private void Register(bool register)
        {
            if(_registered == register)
                return;

            if(register)
            {
                if(_container == null && UISite == null)
                    return;

                _container = UISite.GetService<IServiceContainer>();

                if(_container == null)
                    return;

                if(null == _container.GetService(PageType))
                {
                    _registered = true;
                    _container.AddService(PageType, this);
                }
            }
            else if(_container != null)
            {
                _registered = false;
                _container.RemoveService(PageType);
                _container = null;
            }
        }
    }
}
