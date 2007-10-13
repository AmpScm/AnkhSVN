using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Diagnostics;

namespace Ankh.VSPackage
{
    [Guid(GuidList.guidAnkhSccProviderServiceString)]
    public class SccProviderService : IVsSccProvider, IVsSccManager2
    {

        public int AnyItemsUnderSourceControl( out int pfResult )
        {
            Trace.WriteLine( "In AnyItemsUnderSourceControl" );
            pfResult = active ? 1 : 0;
            return VSConstants.S_OK;
        }

        public int SetActive()
        {
            Trace.WriteLine( "In SetActive" );
            this.active = true;
            return VSConstants.S_OK;
        }

        public int SetInactive()
        {
            Trace.WriteLine( "In SetInActive" );
            this.active = false;
            return VSConstants.S_OK;
        }

        private bool active;

        #region IVsSccManager2 Members

        public int BrowseForProject( out string pbstrDirectory, out int pfOK )
        {
            pbstrDirectory = null;
            pfOK = 0;

            return VSConstants.E_NOTIMPL;
        }

        public int CancelAfterBrowseForProject()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int GetSccGlyph( int cFiles, string[] rgpszFullPaths, VsStateIcon[] rgsiGlyphs, uint[] rgdwSccStatus )
        {
            rgsiGlyphs[ 0 ] = VsStateIcon.STATEICON_CHECKEDIN;
            rgdwSccStatus[ 0 ] = 23;
            return VSConstants.S_OK;
        }

        public int GetSccGlyphFromStatus( uint dwSccStatus, VsStateIcon[] psiGlyph )
        {
            psiGlyph[ 0 ] = VsStateIcon.STATEICON_CHECKEDIN;

            return VSConstants.S_OK;
        }

        public int IsInstalled( out int pbInstalled )
        {
            pbInstalled = 1;

            return VSConstants.S_OK;
        }

        public int RegisterSccProject( IVsSccProject2 pscp2Project, string pszSccProjectName, string pszSccAuxPath, string pszSccLocalPath, string pszProvider )
        {
            return VSConstants.S_OK;
        }

        public int UnregisterSccProject( IVsSccProject2 pscp2Project )
        {
            return VSConstants.S_OK;

        }

        #endregion
    }
}
