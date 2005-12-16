using System.Text;

using EnvDTE;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using NSvn.Core;
using System.Windows.Forms;

//using Microsoft.VisualStudio.Shell.Interop;
//using Microsoft.VisualStudio;
//using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;


namespace Ankh.EventSinks
{
    [InterfaceTypeAttribute( 1 )]
    [GuidAttribute( "6D5140C1-7436-11CE-8034-00AA006009FA" )]
    public interface IServiceProvider
    {
        int QueryService( ref Guid guidService, ref Guid riid, out IntPtr ppvObject );
    }

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
        public const int E_FAIL = -2147467259;
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