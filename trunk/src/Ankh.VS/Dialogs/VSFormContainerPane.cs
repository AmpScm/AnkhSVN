using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Ankh.UI;

namespace Ankh.VS.Dialogs
{
    class VSFormContainerPane : WindowPane
    {
        readonly VSCommandRouting _routing;
        readonly VSContainerForm _form;

        public VSFormContainerPane(IAnkhServiceProvider context, VSCommandRouting routing, VSContainerForm form)
            : base(context)
        {
            if (routing == null)
                throw new ArgumentNullException("routing");
            else if (form == null)
                throw new ArgumentNullException("form");
            _routing = routing;
            _form = form;
        }

        public override System.Windows.Forms.IWin32Window Window
        {
            get { return _form; }
        }
    }
}
