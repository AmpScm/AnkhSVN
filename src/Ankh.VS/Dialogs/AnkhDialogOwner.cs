using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.ContextServices;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using Ankh.UI;
using System.Windows.Forms.Design;

namespace Ankh.VS.Dialogs
{
    class AnkhDialogOwner : AnkhService, IAnkhDialogOwner
    {
        IVsUIShell _shell;
        IUIService _uiService;

        public AnkhDialogOwner(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IVsUIShell Shell
        {
            get { return _shell ?? (_shell = GetService <IVsUIShell>(typeof(SVsUIShell))); }
        }

        IUIService UIService
        {
            get { return _uiService ?? (_uiService = GetService<IUIService>()); }
        }

        #region IAnkhDialogOwner Members

        public IWin32Window DialogOwner
        {
            get 
            {
                if (UIService != null)
                    return UIService.GetDialogOwnerWindow();
                else
                    return null;
            }
        }

        #region IAnkhDialogOwner Members

        public IDisposable InstallFormRouting(Ankh.UI.VSContainerForm container, EventArgs eventArgs)
        {
            return new VSCommandRouting(Context, container);            
        }

        public void OnContainerCreated(Ankh.UI.VSContainerForm form)
        {
            VSCommandRouting routing = VSCommandRouting.FromForm(form);

            if (routing != null)
                routing.OnHandleCreated();
        }
        #endregion

        #endregion

        #region IAnkhDialogOwner Members

        public AnkhMessageBox MessageBox
        {
            get { return new AnkhMessageBox(this); }
        }

        #endregion
    }
}
