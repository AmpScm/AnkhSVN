using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.ContextServices;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using Ankh.UI;

namespace Ankh.VS.Dialogs
{
    class AnkhDialogOwner : AnkhService, IWin32Window, IAnkhDialogOwner
    {
        IVsUIShell _shell;

        public AnkhDialogOwner(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IVsUIShell Shell
        {
            get { return _shell ?? (_shell = GetService <IVsUIShell>(typeof(SVsUIShell))); }
        }

        #region IAnkhDialogOwner Members

        public IWin32Window DialogOwner
        {
            get 
            {
                if (Shell != null)
                    return this;
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

        #region IWin32Window Members

        public IntPtr Handle
        {
            get 
            {
                if (Shell == null)
                    return IntPtr.Zero;

                IntPtr handle;

                Marshal.ThrowExceptionForHR(Shell.GetDialogOwnerHwnd(out handle));

                return handle;
            }
        }        

        #endregion

        #region IAnkhDialogOwner Members

        public AnkhMessageBox MessageBox
        {
            get { return new AnkhMessageBox(this); }
        }

        #endregion
    }
}
