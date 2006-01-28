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


namespace Ankh
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

    public enum VSENUMPROJFLAGS : uint
    {
        EPF_LOADEDINSOLUTION = 0x1,
        EPF_UNLOADEDINSOLUTION = 0x2,
        EPF_ALLINSOLUTION = EPF_LOADEDINSOLUTION | EPF_UNLOADEDINSOLUTION,
        EPF_MATCHTYPE = 0x4,
        EPF_VIRTUALVISIBLEPROJECT = 0x8,
        EPF_VIRTUALNONVISIBLEPROJECT = 0x10,
        EPF_ALLVIRTUAL = EPF_VIRTUALVISIBLEPROJECT | EPF_VIRTUALNONVISIBLEPROJECT,
        EPF_ALLPROJECTS = EPF_ALLINSOLUTION | EPF_ALLVIRTUAL
    }

    enum VSITEMID
    {
        VSITEMID_NIL = -1,
        VSITEMID_ROOT = -2,
        VSITEMID_SELECTION = -3
    }


    enum VSHPROPID 
    {
        VSHPROPID_NIL = -1,
        VSHPROPID_LAST = -1000,
        VSHPROPID_Parent = -1000,
        VSHPROPID_FirstChild = -1001,
        VSHPROPID_NextSibling = -1002,
        VSHPROPID_Root = -1003,
        VSHPROPID_TypeGuid = -1004,
        VSHPROPID_SaveName = -2002,
        VSHPROPID_Caption = -2003,
        VSHPROPID_IconImgList = -2004,
        VSHPROPID_IconIndex = -2005,
        VSHPROPID_Expandable = -2006,
        VSHPROPID_ExpandByDefault = -2011,
        VSHPROPID_ProjectName = -2012,
        VSHPROPID_Name = -2012,
        VSHPROPID_IconHandle = -2013,
        VSHPROPID_OpenFolderIconHandle = -2014,
        VSHPROPID_OpenFolderIconIndex = -2015,
        VSHPROPID_CmdUIGuid = -2016,
        VSHPROPID_SelContainer = -2017,
        VSHPROPID_BrowseObject = -2018,
        VSHPROPID_AltHierarchy = -2019,
        VSHPROPID_AltItemid = -2020,
        VSHPROPID_ProjectDir = -2021,
        VSHPROPID_SortPriority = -2022,
        VSHPROPID_UserContext = -2023,
        VSHPROPID_EditLabel = -2026,
        VSHPROPID_ExtObject = -2027,
        VSHPROPID_ExtSelectedItem = -2028,
        VSHPROPID_StateIconIndex = -2029,
        VSHPROPID_ProjectType = -2030,
        VSHPROPID_TypeName = -2030,
        VSHPROPID_ReloadableProjectFile = -2031,
        VSHPROPID_HandlesOwnReload = -2031,
        VSHPROPID_ParentHierarchy = -2032,
        VSHPROPID_ParentHierarchyItemid = -2033,
        VSHPROPID_ItemDocCookie = -2034,
        VSHPROPID_Expanded = -2035,
        VSHPROPID_ConfigurationProvider = -2036,
        VSHPROPID_ImplantHierarchy = -2037,
        VSHPROPID_OwnerKey = -2038,
        VSHPROPID_StartupServices = -2040,
        VSHPROPID_FirstVisibleChild = -2041,
        VSHPROPID_NextVisibleSibling = -2042,
        VSHPROPID_IsHiddenItem = -2043,
        VSHPROPID_IsNonMemberItem = -2044,
        VSHPROPID_IsNonLocalStorage = -2045,
        VSHPROPID_StorageType = -2046,
        VSHPROPID_ItemSubType = -2047,
        VSHPROPID_OverlayIconIndex = -2048,
        VSHPROPID_DefaultNamespace = -2049,
        VSHPROPID_IsNonSearchable = -2051,
        VSHPROPID_IsFindInFilesForegroundOnly = -2052,
        VSHPROPID_CanBuildFromMemory = -2053,
        VSHPROPID_PreferredLanguageSID = -2054,
        VSHPROPID_ShowProjInSolutionPage = -2055,
        VSHPROPID_AllowEditInRunMode = -2056,
        VSHPROPID_IsNewUnsavedItem = -2057,
        VSHPROPID_ShowOnlyItemCaption = -2058,
        VSHPROPID_ProjectIDGuid = -2059,
        VSHPROPID_DesignerVariableNaming = -2060,
        VSHPROPID_DesignerFunctionVisibility = -2061,
        VSHPROPID_HasEnumerationSideEffects = -2062,
        VSHPROPID_DefaultEnableBuildProjectCfg = -2063,
        VSHPROPID_DefaultEnableDeployProjectCfg = -2064,
        VSHPROPID_FIRST = -2064
    }

    [ComImport, InterfaceType( (short)1 ), Guid( "59B2D1D0-5DB0-4F9F-9609-13F0168516D6" ), ComConversionLoss]
    public interface IVsHierarchy
    {
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int SetSite( [In, MarshalAs( UnmanagedType.Interface )] IServiceProvider psp );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetSite( [MarshalAs( UnmanagedType.Interface )] out IServiceProvider ppSP );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int QueryClose( [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] out int pfCanClose );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Close();
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetGuidProperty( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemid, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSHPROPID" )] int propid, out Guid pguid );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int SetGuidProperty( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemid, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSHPROPID" )] int propid, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFGUID" )] ref Guid rguid );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetProperty( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemid, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSHPROPID" )] int propid, [MarshalAs( UnmanagedType.Struct )] out object pvar );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int SetProperty( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemid, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSHPROPID" )] int propid, [In, MarshalAs( UnmanagedType.Struct )] object var );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetNestedHierarchy( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemid, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFIID" )] ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, [ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] out uint pitemidNested );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetCanonicalName( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemid, [MarshalAs( UnmanagedType.BStr )] out string pbstrName );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int ParseCanonicalName( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszName, [ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] out uint pitemid );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Unused0();
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int AdviseHierarchyEvents( [In, MarshalAs( UnmanagedType.Interface )] object pEventSink, [ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSCOOKIE" )] out uint pdwCookie );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int UnadviseHierarchyEvents( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSCOOKIE" )] uint dwCookie );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Unused1();
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Unused2();
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Unused3();
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Unused4();
    }

    [ComImport, Guid( "6DDD8DC3-32B2-4BF1-A1E1-B6DA40526D1E" ), ComConversionLoss, InterfaceType( (short)1 )]
    public interface IVsHierarchyEvents
    {
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnItemAdded( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemidParent, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemidSiblingPrev, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemidAdded );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnItemsAppended( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemidParent );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnItemDeleted( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemid );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnPropertyChanged( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemid, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSHPROPID" )] int propid, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint flags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnInvalidateItems( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemidParent );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnInvalidateIcon( [In] IntPtr hicon );
    }

    [ComImport, ComConversionLoss, Guid( "7F7CD0DB-91EF-49DC-9FA9-02D128515DD4" ), InterfaceType( (short)1 )]
    public interface IVsSolution
    {
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetProjectEnum( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSENUMPROJFLAGS" )] uint grfEnumFlags, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFGUID" )] ref Guid rguidEnumOnlyThisType, [MarshalAs( UnmanagedType.Interface )] out IEnumHierarchies ppenum );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int CreateProject( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFGUID" )] ref Guid rguidProjectType, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string lpszMoniker, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string lpszLocation, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string lpszName, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSCREATEPROJFLAGS" )] uint grfCreateFlags, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFIID" )] ref Guid iidProject, out IntPtr ppProject );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GenerateUniqueProjectName( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string lpszRoot, [MarshalAs( UnmanagedType.BStr )] out string pbstrProjectName );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetProjectOfGuid( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFGUID" )] ref Guid rguidProjectID, [MarshalAs( UnmanagedType.Interface )] out IVsHierarchy ppHierarchy );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetGuidOfProject( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pHierarchy, out Guid pguidProjectID );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetSolutionInfo( [MarshalAs( UnmanagedType.BStr )] out string pbstrSolutionDirectory, [MarshalAs( UnmanagedType.BStr )] out string pbstrSolutionFile, [MarshalAs( UnmanagedType.BStr )] out string pbstrUserOptsFile );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int AdviseSolutionEvents( [In, MarshalAs( UnmanagedType.Interface )] IVsSolutionEvents pSink, [ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSCOOKIE" )] out uint pdwCookie );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int UnadviseSolutionEvents( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSCOOKIE" )] uint dwCookie );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int SaveSolutionElement( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSSLNSAVEOPTIONS" )] uint grfSaveOpts, [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pHier, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSCOOKIE" )] uint docCookie );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int CloseSolutionElement( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSSLNCLOSEOPTIONS" )] uint grfCloseOpts, [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pHier, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSCOOKIE" )] uint docCookie );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetProjectOfProjref( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszProjref, [MarshalAs( UnmanagedType.Interface )] out IVsHierarchy ppHierarchy, [MarshalAs( UnmanagedType.BStr )] out string pbstrUpdatedProjref, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSUPDATEPROJREFREASON" ), MarshalAs( UnmanagedType.LPArray )] /*VSUPDATEPROJREFREASON*/ IntPtr[] puprUpdateReason );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetProjrefOfProject( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pHierarchy, [MarshalAs( UnmanagedType.BStr )] out string pbstrProjref );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetProjectInfoOfProjref( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszProjref, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSHPROPID" )] int propid, [MarshalAs( UnmanagedType.Struct )] out object pvar );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int AddVirtualProject( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pHierarchy, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSADDVPFLAGS" )] uint grfAddVPFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetItemOfProjref( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszProjref, [MarshalAs( UnmanagedType.Interface )] out IVsHierarchy ppHierarchy, [ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] out uint pitemid, [MarshalAs( UnmanagedType.BStr )] out string pbstrUpdatedProjref, [Out, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSUPDATEPROJREFREASON" ), MarshalAs( UnmanagedType.LPArray )] /*VSUPDATEPROJREFREASON*/IntPtr[] puprUpdateReason );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetProjrefOfItem( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pHierarchy, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSITEMID" )] uint itemid, [MarshalAs( UnmanagedType.BStr )] out string pbstrProjref );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetItemInfoOfProjref( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszProjref, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSHPROPID" )] int propid, [MarshalAs( UnmanagedType.Struct )] out object pvar );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetProjectOfUniqueName( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszUniqueName, [MarshalAs( UnmanagedType.Interface )] out IVsHierarchy ppHierarchy );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetUniqueNameOfProject( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pHierarchy, [MarshalAs( UnmanagedType.BStr )] out string pbstrUniqueName );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetProperty( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSPROPID" )] int propid, [MarshalAs( UnmanagedType.Struct )] out object pvar );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int SetProperty( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSPROPID" )] int propid, [In, MarshalAs( UnmanagedType.Struct )] object var );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OpenSolutionFile( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSSLNOPENOPTIONS" )] uint grfOpenOpts, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszFilename );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int QueryEditSolutionFile( [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] out uint pdwEditResult );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int CreateSolution( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string lpszLocation, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string lpszName, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSCREATESOLUTIONFLAGS" )] uint grfCreateFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetProjectFactory( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint dwReserved, [In, Out, MarshalAs( UnmanagedType.LPArray )] Guid[] pguidProjectType, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszMkProject, [MarshalAs( UnmanagedType.Interface )] out IVsProjectFactory ppProjectFactory );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetProjectTypeGuid( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint dwReserved, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszMkProject, out Guid pguidProjectType );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OpenSolutionViaDlg( [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszStartDirectory, [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] int fDefaultToAllProjectsFilter );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int AddVirtualProjectEx( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pHierarchy, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSADDVPFLAGS" )] uint grfAddVPFlags, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFGUID" )] ref Guid rguidProjectID );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int QueryRenameProject( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszMkOldName, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszMkNewName, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint dwReserved, [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] out int pfRenameCanContinue );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterRenameProject( [In, MarshalAs( UnmanagedType.Interface )] IVsProject pProject, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszMkOldName, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszMkNewName, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint dwReserved );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int RemoveVirtualProject( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pHierarchy, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSREMOVEVPFLAGS" )] uint grfRemoveVPFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int CreateNewProjectViaDlg( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszExpand, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszSelect, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.DWORD" )] uint dwReserved );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetVirtualProjectFlags( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pHierarchy, [ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSADDVPFLAGS" )] out uint pgrfAddVPFlags );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GenerateNextDefaultProjectName( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszBaseName, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszLocation, [MarshalAs( UnmanagedType.BStr )] out string pbstrProjectName );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int GetProjectFilesInSolution( [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSGETPROJFILESFLAGS" )] uint grfGetOpts, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint cProjects, [Out, MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.BStr, SizeParamIndex = 1 )] string[] rgbstrProjectNames, [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] out uint pcProjectsFetched );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int CanCreateNewProjectAtLocation( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] int fCreateNewSolution, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszFullProjectFilePath, [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] out int pfCanCreate );
    }

    [ComImport, InterfaceType( (short)1 ), Guid( "BEC77711-2DF9-44D7-B478-A453C2E8A134" )]
    public interface IEnumHierarchies
    {
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Next( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint celt, [Out, MarshalAs( UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0 )] object[] rgelt, [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] out uint pceltFetched );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Skip( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.ULONG" )] uint celt );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Reset();
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Clone( [MarshalAs( UnmanagedType.Interface )] out IEnumHierarchies ppenum );
    }

    [ComImport, InterfaceType( (short)1 ), Guid( "A8516B56-7421-4DBD-AB87-57AF7A2E85DE" )]
    public interface IVsSolutionEvents
    {
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterOpenProject( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pHierarchy, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] int fAdded );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryCloseProject( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pHierarchy, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] int fRemoving, [In, Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] ref int pfCancel );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnBeforeCloseProject( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pHierarchy, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] int fRemoved );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterLoadProject( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pStubHierarchy, [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pRealHierarchy );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryUnloadProject( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pRealHierarchy, [In, Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] ref int pfCancel );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnBeforeUnloadProject( [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pRealHierarchy, [In, MarshalAs( UnmanagedType.Interface )] IVsHierarchy pStubHierarchy );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterOpenSolution( [In, MarshalAs( UnmanagedType.IUnknown )] object pUnkReserved, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] int fNewSolution );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnQueryCloseSolution( [In, MarshalAs( UnmanagedType.IUnknown )] object pUnkReserved, [In, Out, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] ref int pfCancel );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnBeforeCloseSolution( [In, MarshalAs( UnmanagedType.IUnknown )] object pUnkReserved );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int OnAfterCloseSolution( [In, MarshalAs( UnmanagedType.IUnknown )] object pUnkReserved );
    }

    [ComImport, ComConversionLoss, Guid( "33FCD00A-BD45-403C-9C66-07BA9A923501" ), InterfaceType( (short)1 )]
    public interface IVsProjectFactory
    {
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int CanCreateProject( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszFilename, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSCREATEPROJFLAGS" )] uint grfCreateFlags, [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] out int pfCanCreate );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int CreateProject( [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszFilename, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszLocation, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.LPCOLESTR" ), MarshalAs( UnmanagedType.LPWStr )] string pszName, [In, ComAliasName( "Microsoft.VisualStudio.Shell.Interop.VSCREATEPROJFLAGS" )] uint grfCreateFlags, [In, ComAliasName( "Microsoft.VisualStudio.OLE.Interop.REFIID" )] ref Guid iidProject, out IntPtr ppvProject, [ComAliasName( "Microsoft.VisualStudio.OLE.Interop.BOOL" )] out int pfCanceled );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int SetSite( [In, MarshalAs( UnmanagedType.Interface )] IServiceProvider psp );
        [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        int Close();
    }

    [Guid( "7F7CD0DB-91EF-49dc-9FA9-02D128515DD4" )]
    public interface SVsSolution
    {
    }
}