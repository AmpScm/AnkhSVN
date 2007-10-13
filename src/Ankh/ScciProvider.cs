using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ankh
{
    [GuidAttribute("EA4CA998-F8E5-40dd-9A2F-20571D30AC96")]
    [ComVisible(true)]
    public class ScciProvider : IVsSccProvider, Microsoft.VisualStudio.OLE.Interop.IServiceProvider
    {
        #region IVsSccProvider Members

        public int AnyItemsUnderSourceControl( out int pfResult )
        {
            pfResult = 1;
            return VSConstants.S_OK;
        }

        public int SetActive()
        {
            Trace.WriteLine( "In SetActive" );

            return VSConstants.S_OK;
        }

        public int SetInactive()
        {
            Trace.WriteLine( "In SetInactive" );
            return VSConstants.S_OK;
        }

        #endregion

        #region IServiceProvider Members

        public int QueryService( ref Guid guidService, ref Guid riid, out IntPtr ppvObject )
        {
            ppvObject = Marshal.GetIUnknownForObject( this );

            return VSConstants.S_OK;
        }

        #endregion
    }
}
