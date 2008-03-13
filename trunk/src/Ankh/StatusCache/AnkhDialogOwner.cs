using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.ContextServices;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;

namespace Ankh
{
    class AnkhDialogOwner : IWin32Window, IAnkhDialogOwner
    {
        readonly IAnkhServiceProvider _context;
        IVsUIShell _shell;

        public AnkhDialogOwner(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        IVsUIShell Shell
        {
            get { return _shell ?? (_shell = (IVsUIShell)_context.GetService(typeof(SVsShell))); }
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
    }
}
