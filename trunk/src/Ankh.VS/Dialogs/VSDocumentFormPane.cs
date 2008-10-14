using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Ankh.UI;

namespace Ankh.VS.Dialogs
{
    sealed class VSDocumentFormPane : WindowPane
    {
        readonly VSDocumentForm _form;
        public VSDocumentFormPane(IAnkhServiceProvider context, VSDocumentForm form)
            : base(context)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            _form = form;
        }

        bool _created;
        public override IWin32Window Window
        {
            get
            {
                if (!_created)
                {
                    _created = true;
                    if (!_form.IsHandleCreated)
                    {
                        _form.Visible = true; // If .Visible = false no window is created!
                        _form.CreateControl();
                        _form.Visible = false; // And hide the window now or we hijack the focus. See issue #507
                    }
                }
                return _form;
            }
        }

        internal void Show()
        {
            base.Initialize();
            //throw new NotImplementedException();
        }
    }
}
