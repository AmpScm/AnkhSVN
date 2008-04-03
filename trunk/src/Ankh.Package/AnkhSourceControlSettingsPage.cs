using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Ankh.UI;

namespace Ankh.VSPackage
{
    class AnkhSourceControlSettingsPage : DialogPage
    {
        AnkhSettingsControl _control;
        protected override System.Windows.Forms.IWin32Window Window
        {
            get
            {
                return _control ?? (_control = CreateControl());
            }
        }

        private AnkhSettingsControl CreateControl()
        {
            AnkhSettingsControl control = new AnkhSettingsControl();
            IAnkhServiceProvider sp = (IAnkhServiceProvider)GetService(typeof(IAnkhServiceProvider));
            
            if (sp != null)
                control.Context = sp;

            return control;
        }
    }
}
