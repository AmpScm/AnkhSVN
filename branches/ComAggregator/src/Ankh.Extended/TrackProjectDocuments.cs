using System.Collections.Generic;
using System.Text;

using EnvDTE;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

//using Microsoft.VisualStudio.Shell.Interop;
//using Microsoft.VisualStudio;
//using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;


namespace Ankh.Extended
{
    class TrackProjectDocuments : IVsTrackProjectDocumentsEvents2
    {
        public TrackProjectDocuments( IContext context )
        {
            this.context = context;
            AdviceEvents();
        }

        private void AdviceEvents()
        {
            IServiceProvider sp = (IServiceProvider)this.context.DTE;
            IntPtr svcPtr;
            Guid serviceGuid = typeof(SVsTrackProjectDocuments).GUID;
            Guid interfaceGuid = typeof( IVsTrackProjectDocuments2 ).GUID;

            //Guid iUnkown = NativeMethods.
            sp.QueryService( ref serviceGuid, ref interfaceGuid, out svcPtr );

            IVsTrackProjectDocuments2 track = (IVsTrackProjectDocuments2) Marshal.GetObjectForIUnknown( svcPtr );

            this.cookie = 42;
            if ( track.AdviseTrackProjectDocumentsEvents( this, out this.cookie ) != VSConstants.S_OK || this.cookie == 0 )
            {
                throw new Exception( "Couldn't register project documents events interface" );
            };

            
        }

        #region IVsTrackProjectDocumentsEvents2 Members

        public int OnAfterAddDirectoriesEx( int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterAddDirectoriesEx() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnAfterAddFilesEx( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterAddFilesEx() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveDirectories( int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterRemoveDirectories() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveFiles( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterRemoveFiles() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnAfterRenameDirectories( int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterRenameDirectories() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnAfterRenameFiles( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterRenameFiles() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnAfterSccStatusChanged( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterSccStatusChanged() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnQueryAddDirectories( IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryAddDirectories() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnQueryAddFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryAddFiles() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveDirectories( IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryRemoveDirectories() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryRemoveFiles() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnQueryRenameDirectories( IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryRenameDirectories() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnQueryRenameFiles( IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryRenameFiles() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }
        #endregion


        private IContext context;
        private uint cookie;


    }

    [InterfaceTypeAttribute( 1 )]
    [GuidAttribute( "6D5140C1-7436-11CE-8034-00AA006009FA" )]
    public interface IServiceProvider
    {
        int QueryService( ref Guid guidService, ref Guid riid, out IntPtr ppvObject );
    }

    //[InterfaceTypeAttribute( 1 )]
    //[GuidAttribute( "53544C4D-663D-11D3-A60D-005004775AB1" )]
    //public interface IVsTrackProjectDocumentsEvents2
    //{
    //    int OnAfterAddDirectoriesEx( int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags );
    //    int OnAfterAddFilesEx( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags );
    //    int OnAfterRemoveDirectories( int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags );
    //    int OnAfterRemoveFiles( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags );
    //    int OnAfterRenameDirectories( int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags );
    //    int OnAfterRenameFiles( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags );
    //    int OnAfterSccStatusChanged( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus );
    //    int OnQueryAddDirectories( IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults );
    //    int OnQueryAddFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults );
    //    int OnQueryRemoveDirectories( IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults );
    //    int OnQueryRemoveFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults );
    //    int OnQueryRenameDirectories( IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults );
    //    int OnQueryRenameFiles( IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults );
    //}

    [ComImport, InterfaceType( (short)1 ), Guid( "53544C4D-663D-11D3-A60D-005004775AB1" )]
    public interface IVsTrackProjectDocumentsEvents2
    {
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryAddFiles( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cFiles, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYADDFILEFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYADDFILEFLAGS[] rgFlags, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYADDFILERESULTS" ), MarshalAs( UnmanagedType.LPArray )] VSQUERYADDFILERESULTS[] pSummaryResult, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYADDFILERESULTS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYADDFILERESULTS[] rgResults );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterAddFilesEx( [In] int cProjects, [In] int cFiles, [In, MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0 )] IVsProject[] rgpProjects, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] int[] rgFirstIndices, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSADDFILEFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSADDFILEFLAGS[] rgFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterAddDirectoriesEx( [In] int cProjects, [In] int cDirectories, [In, MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0 )] IVsProject[] rgpProjects, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] int[] rgFirstIndices, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSADDDIRECTORYFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSADDDIRECTORYFLAGS[] rgFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterRemoveFiles( [In] int cProjects, [In] int cFiles, [In, MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0 )] IVsProject[] rgpProjects, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] int[] rgFirstIndices, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSREMOVEFILEFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSREMOVEFILEFLAGS[] rgFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterRemoveDirectories( [In] int cProjects, [In] int cDirectories, [In, MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0 )] IVsProject[] rgpProjects, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] int[] rgFirstIndices, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSREMOVEDIRECTORYFLAGS[] rgFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryRenameFiles( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cFiles, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkOldNames, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkNewNames, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYRENAMEFILEFLAGS[] rgFlags, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYRENAMEFILERESULTS" ), MarshalAs( UnmanagedType.LPArray )] VSQUERYRENAMEFILERESULTS[] pSummaryResult, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYRENAMEFILERESULTS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYRENAMEFILERESULTS[] rgResults );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterRenameFiles( [In] int cProjects, [In] int cFiles, [In, MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0 )] IVsProject[] rgpProjects, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] int[] rgFirstIndices, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkOldNames, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkNewNames, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSRENAMEFILEFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSRENAMEFILEFLAGS[] rgFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryRenameDirectories( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cDirs, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkOldNames, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkNewNames, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYRENAMEDIRECTORYFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYRENAMEDIRECTORYRESULTS" ), MarshalAs( UnmanagedType.LPArray )] VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYRENAMEDIRECTORYRESULTS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYRENAMEDIRECTORYRESULTS[] rgResults );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterRenameDirectories( [In] int cProjects, [In] int cDirs, [In, MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0 )] IVsProject[] rgpProjects, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] int[] rgFirstIndices, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkOldNames, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkNewNames, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSRENAMEDIRECTORYFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSRENAMEDIRECTORYFLAGS[] rgFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryAddDirectories( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cDirectories, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYADDDIRECTORYFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYADDDIRECTORYFLAGS[] rgFlags, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYADDDIRECTORYRESULTS" ), MarshalAs( UnmanagedType.LPArray )] VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYADDDIRECTORYRESULTS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYADDDIRECTORYRESULTS[] rgResults );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryRemoveFiles( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cFiles, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYREMOVEFILEFLAGS[] rgFlags, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYREMOVEFILERESULTS" ), MarshalAs( UnmanagedType.LPArray )] VSQUERYREMOVEFILERESULTS[] pSummaryResult, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYREMOVEFILERESULTS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYREMOVEFILERESULTS[] rgResults );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryRemoveDirectories( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cDirectories, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYREMOVEDIRECTORYFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYREMOVEDIRECTORYRESULTS" ), MarshalAs( UnmanagedType.LPArray )] VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYREMOVEDIRECTORYRESULTS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYREMOVEDIRECTORYRESULTS[] rgResults );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterSccStatusChanged( [In] int cProjects, [In] int cFiles, [In, MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0 )] IVsProject[] rgpProjects, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] int[] rgFirstIndices, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] uint[] rgdwSccStatus );
    }
 


    public enum VSQUERYRENAMEDIRECTORYRESULTS
    {
        VSQUERYRENAMEDIRECTORYRESULTS_RenameOK = 0,
        VSQUERYRENAMEDIRECTORYRESULTS_RenameNotOK = 1,
    }

    public enum VSQUERYRENAMEFILERESULTS
    {
        VSQUERYRENAMEFILERESULTS_RenameOK = 0,
        VSQUERYRENAMEFILERESULTS_RenameNotOK = 1,
    }

    [FlagsAttribute]
    public enum VSQUERYRENAMEFILEFLAGS
    {
        VSQUERYRENAMEFILEFLAGS_NoFlags = 0,
        VSQUERYRENAMEFILEFLAGS_IsSpecialFile = 1,
        VSQUERYRENAMEFILEFLAGS_IsNestedProjectFile = 2,
        VSQUERYRENAMEFILEFLAGS_Directory = 32,
    }

    [InterfaceTypeAttribute( 1 )]
    [GuidAttribute( "53544C4D-1639-11D3-A60D-005004775AB1" )]
    public interface SVsTrackProjectDocuments
    {
    }

    //[GuidAttribute( "53544C4D-6639-11D3-A60D-005004775AB1" )]
    //[InterfaceTypeAttribute( 1 )]
    //public interface IVsTrackProjectDocuments2
    //{
    //    int AdviseTrackProjectDocumentsEvents( IVsTrackProjectDocumentsEvents2 pEventSink, out uint pdwCookie );
    //    int BeginBatch();
    //    int EndBatch();
    //    int Flush();
    //    int OnAfterAddDirectories( IVsProject pProject, int cDirectories, string[] rgpszMkDocuments );
    //    int OnAfterAddDirectoriesEx( IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags );
    //    int OnAfterAddFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments );
    //    int OnAfterAddFilesEx( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags );
    //    int OnAfterRemoveDirectories( IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags );
    //    int OnAfterRemoveFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags );
    //    int OnAfterRenameDirectories( IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags );
    //    int OnAfterRenameFile( IVsProject pProject, string pszMkOldName, string pszMkNewName, VSRENAMEFILEFLAGS flags );
    //    int OnAfterRenameFiles( IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags );
    //    int OnAfterSccStatusChanged( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, uint[] rgdwSccStatus );
    //    int OnQueryAddDirectories( IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults );
    //    int OnQueryAddFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults );
    //    int OnQueryRemoveDirectories( IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults );
    //    int OnQueryRemoveFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults );
    //    int OnQueryRenameDirectories( IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults );
    //    int OnQueryRenameFile( IVsProject pProject, string pszMkOldName, string pszMkNewName, VSRENAMEFILEFLAGS flags, out int pfRenameCanContinue );
    //    int OnQueryRenameFiles( IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults );
    //    int UnadviseTrackProjectDocumentsEvents( uint dwCookie );
    //}
    [ComImport, Guid( "53544C4D-6639-11D3-A60D-005004775AB1" ), InterfaceType( (short)1 )]
    public interface IVsTrackProjectDocuments2
    {
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int BeginBatch();
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int EndBatch();
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Flush();
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryAddFiles( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cFiles, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYADDFILEFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYADDFILEFLAGS[] rgFlags, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYADDFILERESULTS" ), MarshalAs( UnmanagedType.LPArray )] VSQUERYADDFILERESULTS[] pSummaryResult, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYADDFILERESULTS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYADDFILERESULTS[] rgResults );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterAddFilesEx( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cFiles, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSADDFILEFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSADDFILEFLAGS[] rgFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterAddFiles( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cFiles, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterAddDirectoriesEx( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cDirectories, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSADDDIRECTORYFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSADDDIRECTORYFLAGS[] rgFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterAddDirectories( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cDirectories, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterRemoveFiles( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cFiles, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSREMOVEFILEFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSREMOVEFILEFLAGS[] rgFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterRemoveDirectories( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cDirectories, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSREMOVEDIRECTORYFLAGS[] rgFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryRenameFiles( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cFiles, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkOldNames, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkNewNames, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYRENAMEFILEFLAGS[] rgFlags, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYRENAMEFILERESULTS" ), MarshalAs( UnmanagedType.LPArray )] VSQUERYRENAMEFILERESULTS[] pSummaryResult, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYRENAMEFILERESULTS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYRENAMEFILERESULTS[] rgResults );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryRenameFile( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszMkOldName, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszMkNewName, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSRENAMEFILEFLAGS" )] VSRENAMEFILEFLAGS flags, [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] out int pfRenameCanContinue );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterRenameFiles( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cFiles, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkOldNames, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkNewNames, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSRENAMEFILEFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSRENAMEFILEFLAGS[] rgFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterRenameFile( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszMkOldName, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszMkNewName, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSRENAMEFILEFLAGS" )] VSRENAMEFILEFLAGS flags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryRenameDirectories( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cDirs, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkOldNames, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkNewNames, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYRENAMEDIRECTORYFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYRENAMEDIRECTORYRESULTS" ), MarshalAs( UnmanagedType.LPArray )] VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYRENAMEDIRECTORYRESULTS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYRENAMEDIRECTORYRESULTS[] rgResults );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterRenameDirectories( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cDirs, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkOldNames, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgszMkNewNames, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSRENAMEDIRECTORYFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSRENAMEDIRECTORYFLAGS[] rgFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int AdviseTrackProjectDocumentsEvents( [In, MarshalAs( UnmanagedType.Interface )] IVsTrackProjectDocumentsEvents2 pEventSink, [ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSCOOKIE" )] out uint pdwCookie );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int UnadviseTrackProjectDocumentsEvents( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSCOOKIE" )] uint dwCookie );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryAddDirectories( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cDirectories, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYADDDIRECTORYFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYADDDIRECTORYFLAGS[] rgFlags, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYADDDIRECTORYRESULTS" ), MarshalAs( UnmanagedType.LPArray )] VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYADDDIRECTORYRESULTS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYADDDIRECTORYRESULTS[] rgResults );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryRemoveFiles( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cFiles, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYREMOVEFILEFLAGS[] rgFlags, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYREMOVEFILERESULTS" ), MarshalAs( UnmanagedType.LPArray )] VSQUERYREMOVEFILERESULTS[] pSummaryResult, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYREMOVEFILERESULTS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYREMOVEFILERESULTS[] rgResults );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryRemoveDirectories( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cDirectories, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYREMOVEDIRECTORYFLAGS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYREMOVEDIRECTORYRESULTS" ), MarshalAs( UnmanagedType.LPArray )] VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSQUERYREMOVEDIRECTORYRESULTS" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] VSQUERYREMOVEDIRECTORYRESULTS[] rgResults );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterSccStatusChanged( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In] int cFiles, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1 )] string[] rgpszMkDocuments, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" ), MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] uint[] rgdwSccStatus );
    }
 


    [FlagsAttribute]
    public enum VSQUERYRENAMEDIRECTORYFLAGS
    {
        VSQUERYRENAMEDIRECTORYFLAGS_padding = 0,
    }

    public enum VSQUERYREMOVEFILERESULTS
    {
        VSQUERYREMOVEFILERESULTS_RemoveOK = 0,
        VSQUERYREMOVEFILERESULTS_RemoveNotOK = 1,
    }

    [FlagsAttribute]
    public enum VSQUERYREMOVEFILEFLAGS
    {
        VSQUERYREMOVEFILEFLAGS_NoFlags = 0,
        VSQUERYREMOVEFILEFLAGS_IsSpecialFile = 1,
        VSQUERYREMOVEFILEFLAGS_IsNestedProjectFile = 2,
    }

    public enum VSQUERYREMOVEDIRECTORYRESULTS
    {
        VSQUERYREMOVEDIRECTORYRESULTS_RemoveOK = 0,
        VSQUERYREMOVEDIRECTORYRESULTS_RemoveNotOK = 1,
    }

    [FlagsAttribute]
    public enum VSQUERYREMOVEDIRECTORYFLAGS
    {
        VSQUERYREMOVEDIRECTORYFLAGS_padding = 0,
    }

    [FlagsAttribute]
    public enum VSRENAMEDIRECTORYFLAGS
    {
        VSRENAMEDIRECTORYFLAGS_NoFlags = 0,
        VSRENAMEDIRECTORYFLAGS_RenameInSourceControlDoneExternally = 1,
    }


    public enum VSQUERYADDDIRECTORYRESULTS
    {
        VSQUERYADDDIRECTORYRESULTS_AddOK = 0,
        VSQUERYADDDIRECTORYRESULTS_AddNotOK = 1,
    }

    [FlagsAttribute]
    public enum VSQUERYADDDIRECTORYFLAGS
    {
        VSQUERYADDDIRECTORYFLAGS_padding = 0,
    }

    [FlagsAttribute]
    public enum VSADDDIRECTORYFLAGS
    {
        VSADDDIRECTORYFLAGS_NoFlags = 0,
        VSADDDIRECTORYFLAGS_AddToSourceControlDoneExternally = 1,
    }

    [FlagsAttribute]
    public enum VSREMOVEDIRECTORYFLAGS
    {
        VSREMOVEDIRECTORYFLAGS_NoFlags = 0,
        VSREMOVEDIRECTORYFLAGS_IsDirectoryBased = 1,
        VSREMOVEDIRECTORYFLAGS_RemoveFromSourceControlDoneExternally = 2,
    }

    public enum VSQUERYADDFILERESULTS
    {
        VSQUERYADDFILERESULTS_AddOK = 0,
        VSQUERYADDFILERESULTS_AddNotOK = 1,
    }

    [FlagsAttribute]
    public enum VSQUERYADDFILEFLAGS
    {
        VSQUERYADDFILEFLAGS_NoFlags = 0,
        VSQUERYADDFILEFLAGS_IsSpecialFile = 1,
        VSQUERYADDFILEFLAGS_IsNestedProjectFile = 2,
    }

    [FlagsAttribute]
    public enum VSRENAMEFILEFLAGS
    {
        VSRENAMEFILEFLAGS_INVALID = -512,
        VSRENAMEFILEFLAGS_NoFlags = 0,
        VSRENAMEFILEFLAGS_FromShellCommand = 1,
        VSRENAMEFILEFLAGS_FromScc = 2,
        VSRENAMEFILEFLAGS_FromFileChange = 4,
        VSRENAMEFILEFLAGS_WasQueried = 8,
        VSRENAMEFILEFLAGS_AlreadyOnDisk = 16,
        VSRENAMEFILEFLAGS_Directory = 32,
        VSRENAMEFILEFLAGS_RenameInSourceControlDoneExternally = 64,
        VSRENAMEFILEFLAGS_IsSpecialFile = 128,
        VSRENAMEFILEFLAGS_IsNestedProjectFile = 256,
        VSRENAMEFILEFLAGS_ALL = 511,
    }

    [FlagsAttribute]
    public enum VSREMOVEFILEFLAGS
    {
        VSREMOVEFILEFLAGS_NoFlags = 0,
        VSREMOVEFILEFLAGS_IsDirectoryBased = 1,
        VSREMOVEFILEFLAGS_RemoveFromSourceControlDoneExternally = 2,
        VSREMOVEFILEFLAGS_IsSpecialFile = 4,
        VSREMOVEFILEFLAGS_IsNestedProjectFile = 8,
    }

    [FlagsAttribute]
    public enum VSADDFILEFLAGS
    {
        VSADDFILEFLAGS_NoFlags = 0,
        VSADDFILEFLAGS_AddToSourceControlDoneExternally = 1,
        VSADDFILEFLAGS_IsSpecialFile = 2,
        VSADDFILEFLAGS_IsNestedProjectFile = 4,
    }

    public class VSConstants
    {
        public const int S_OK = 0;
        public const int S_FALSE = 0;
    }


    [GuidAttribute( "CD4028ED-C4D8-44BA-890F-E7FB02A380C6" )]
    [InterfaceTypeAttribute( 1 )]
    [ComConversionLossAttribute]
    public interface IVsProject
    {
        int AddItem( uint itemidLoc, VSADDITEMOPERATION dwAddItemOperation, string pszItemName, uint cFilesToOpen, string[] rgpszFilesToOpen, IntPtr hwndDlgOwner, VSADDRESULT[] pResult );
        int GenerateUniqueItemName( uint itemidLoc, string pszExt, string pszSuggestedRoot, out string pbstrItemName );
        int GetItemContext( uint itemid, out IServiceProvider ppSP );
        int GetMkDocument( uint itemid, out string pbstrMkDocument );
        int IsDocumentInProject( string pszMkDocument, out int pfFound, VSDOCUMENTPRIORITY[] pdwPriority, out uint pitemid );
        int OpenItem( uint itemid, ref Guid rguidLogicalView, IntPtr punkDocDataExisting, out /*IVsWindowFrame*/ IntPtr ppWindowFrame );
    }

    public enum VSDOCUMENTPRIORITY
    {
        DP_Unsupported = 0,
        DP_CanAddAsExternal = 10,
        DP_External = 20,
        DP_CanAddAsNonMember = 30,
        DP_NonMember = 40,
        DP_Standard = 50,
        DP_Intrinsic = 60,
    }

    public enum VSADDRESULT
    {
        ADDRESULT_Success = -1,
        ADDRESULT_Failure = 0,
        ADDRESULT_Cancel = 1,
    }

    public enum VSADDITEMOPERATION
    {
        VSADDITEMOP_OPENFILE = 1,
        VSADDITEMOP_CLONEFILE = 2,
        VSADDITEMOP_RUNWIZARD = 3,
        VSADDITEMOP_LINKTOFILE = 4,
    }

}
