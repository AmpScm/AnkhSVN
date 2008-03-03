using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SharpSvn;

namespace Ankh.VSPackage
{
    [GuidAttribute( GuidList.guidAnkhSccProviderServiceString )]
    class SccProviderService : IVsSccProvider, IVsSccManager2, IVsSccGlyphs
    {
        public SccProviderService( IContext context )
        {
            this.context = context;
        }

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
            for ( int i = 0; i < cFiles; i++ )
            {
                SvnItem item = this.context.StatusCache[ rgpszFullPaths[ i ] ];
                NodeStatus nodeStatus = GenerateStatus( item );
                rgsiGlyphs[ i ] = (VsStateIcon)
                    StatusImages.GetStatusImageForNodeStatus( nodeStatus );
                rgdwSccStatus[ i ] = 21;
            }

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

        public int GetCustomGlyphList( uint BaseIndex, out uint pdwImageListHandle )
        {
            pdwImageListHandle = (uint)StatusImages.StatusImageList.Handle.ToInt32();

            this.baseIndex = BaseIndex;

            return VSConstants.S_OK;
        }

        protected static NodeStatus GenerateStatus( SvnItem item )
        {
            AnkhStatus status = item.Status;
            NodeStatusKind kind;
            if ( status.LocalContentStatus != SvnStatus.Normal )
            {
                kind = (NodeStatusKind)status.LocalContentStatus;
            }
            else if ( status.LocalPropertyStatus != SvnStatus.Normal &&
                status.LocalPropertyStatus != SvnStatus.None )
            {
                kind = (NodeStatusKind)status.LocalPropertyStatus;
            }
            else
            {
                kind = NodeStatusKind.Normal;
            }

            return new NodeStatus( kind, item.IsReadOnly, item.IsLocked );
        }


        private uint baseIndex;
        private bool active;
        private IContext context;

    }
}
