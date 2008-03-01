using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;

namespace Ankh.VSPackage
{
    [GuidAttribute( GuidList.guidAnkhSccProviderServiceString )]
    class SccProviderService : IVsSccProvider, IVsSccManager2, IVsSccGlyphs
    {
        public int AnyItemsUnderSourceControl( out int pfResult )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public int SetActive()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public int SetInactive()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public int BrowseForProject( out string pbstrDirectory, out int pfOK )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public int CancelAfterBrowseForProject()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public int GetSccGlyph( int cFiles, string[] rgpszFullPaths, VsStateIcon[] rgsiGlyphs, uint[] rgdwSccStatus )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public int GetSccGlyphFromStatus( uint dwSccStatus, VsStateIcon[] psiGlyph )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public int IsInstalled( out int pbInstalled )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public int RegisterSccProject( IVsSccProject2 pscp2Project, string pszSccProjectName, string pszSccAuxPath, string pszSccLocalPath, string pszProvider )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public int UnregisterSccProject( IVsSccProject2 pscp2Project )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        public int GetCustomGlyphList( uint BaseIndex, out uint pdwImageListHandle )
        {
            throw new Exception( "The method or operation is not implemented." );
        }
    }
}
