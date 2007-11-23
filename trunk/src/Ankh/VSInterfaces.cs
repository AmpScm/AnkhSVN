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
using System.Collections;
using System.Reflection;

//using Microsoft.VisualStudio.Shell.Interop;
//using Microsoft.VisualStudio;
//using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;


namespace Ankh
{
    public class VSConstants
    {
        public const int S_OK = 0;
        public const int E_FAIL = -2147467259;
    }

    enum VSITEMID
    {
        VSITEMID_NIL = -1,
        VSITEMID_ROOT = -2,
        VSITEMID_SELECTION = -3
    }

    [ComImport, TypeLibType( (short)0x1040 ), Guid( "238B5173-2429-11D7-8BF6-00B0D03DAA06" )]
    public interface VCProjectItem
    {
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x2a3 )]
        bool MatchName( [In, MarshalAs( UnmanagedType.BStr )] string NameToMatch, [In] bool FullOnly );
        [DispId( 0x2a4 )]
        object project { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x2a4 )] get; }
        [DispId( 0x2a5 )]
        object Parent { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x2a5 )] get; }
        [DispId( 0x2a6 )]
        string ItemName { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x2a6 )] get; }
        [DispId( 0x2a7 )]
        string Kind { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x2a7 )] get; }
        [DispId( 680 )]
        object VCProjectEngine { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 680 )] get; }
    }

    [TypeLibType( (short)0x10 )]
    public enum enumFileFormat
    {
        eANSI,
        eUTF8,
        eUnicode
    }

    [ComImport, TypeLibType( (short)0x1040 ), Guid( "238B5187-2429-11D7-8BF6-00B0D03DAA06" )]
    public interface VCToolFile
    {
        [DispId( 0x898 )]
        string Name { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x898 )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x898 )] set; }
        [DispId( 0x899 )]
        string Path { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x899 )] get; }
        [DispId( 0x89a )]
        object CustomBuildRules { [return: MarshalAs( UnmanagedType.Interface )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x89a )] get; }
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x89d )]
        void Save( [In, MarshalAs( UnmanagedType.BStr )] string Path );
        [return: MarshalAs( UnmanagedType.IDispatch )]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x89e )]
        object AddCustomBuildRule( [In, MarshalAs( UnmanagedType.BStr )] string Name, [In, MarshalAs( UnmanagedType.BStr )] string CommandLine, [In, MarshalAs( UnmanagedType.BStr )] string Outputs, [In, MarshalAs( UnmanagedType.BStr )] string FileExtensions );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x89f )]
        void RemoveCustomBuildRule( [In, MarshalAs( UnmanagedType.IDispatch )] object CustomBuildRule );
    }
 

    [ComImport, TypeLibType( (short)0x1040 ), Guid( "238B5174-2429-11D7-8BF6-00B0D03DAA06" )]
    public interface VCProject //: VCProjectItem
    {
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x2a3 )]
        bool MatchName( [In, MarshalAs( UnmanagedType.BStr )] string NameToMatch, [In] bool FullOnly );
        [DispId( 0x2a4 )]
        object project { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x2a4 )] get; }
        [DispId( 0x2a5 )]
        object Parent { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x2a5 )] get; }
        [DispId( 0x2a6 )]
        string ItemName { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x2a6 )] get; }
        [DispId( 0x2a7 )]
        string Kind { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x2a7 )] get; }
        [DispId( 680 )]
        object VCProjectEngine { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 680 )] get; }
        [DispId( 0x33d )]
        string Name { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x33d )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x33d )] set; }
        [DispId( 0x322 )]
        string ProjectDirectory { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x322 )] get; }
        [DispId( 0x330 )]
        string ProjectFile { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x330 )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x330 )] set; }
        [DispId( 0x336 )]
        object Platforms { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x336 ), TypeLibFunc( (short)0x400 )] get; }
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x32b )]
        void AddPlatform( [In, MarshalAs( UnmanagedType.BStr )] string PlatformName );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x32c )]
        void RemovePlatform( [In, MarshalAs( UnmanagedType.IDispatch )] object Platform );
        [DispId( 0x328 )]
        object Configurations { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x328 ), TypeLibFunc( (short)0x400 )] get; }
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x325 )]
        void AddConfiguration( [MarshalAs( UnmanagedType.BStr )] string ConfigurationName );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x337 )]
        void RemoveConfiguration( [MarshalAs( UnmanagedType.IDispatch )] object Config );
        [DispId( 0x327 )]
        object Files { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x327 ), TypeLibFunc( (short)0x400 )] get; }
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x324 )]
        bool CanAddFile( [In, MarshalAs( UnmanagedType.BStr )] string File );
        [return: MarshalAs( UnmanagedType.IDispatch )]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x323 )]
        object AddFile( [MarshalAs( UnmanagedType.BStr )] string bstrPath );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x326 )]
        void RemoveFile( [In, MarshalAs( UnmanagedType.IDispatch )] object File );
        [DispId( 0x332 )]
        object Filters { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x332 ), TypeLibFunc( (short)0x400 )] get; }
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x32f )]
        bool CanAddFilter( [In, MarshalAs( UnmanagedType.BStr )] string Filter );
        [return: MarshalAs( UnmanagedType.IDispatch )]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x32e )]
        object AddFilter( [MarshalAs( UnmanagedType.BStr )] string bstrFilterName );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x32d )]
        void RemoveFilter( [In, MarshalAs( UnmanagedType.IDispatch )] object Filter );
        [DispId( 0x339 )]
        object Items { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x339 ), TypeLibFunc( (short)0x400 )] get; }
        [return: MarshalAs( UnmanagedType.IDispatch )]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x33a )]
        object AddWebReference( [In, MarshalAs( UnmanagedType.BStr )] string URL, [In, MarshalAs( UnmanagedType.BStr )] string Name );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x329 )]
        void Save();
        [DispId( 0x333 )]
        bool IsDirty { [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)0x40 ), DispId( 0x333 )] get; }
        [DispId( 0x338 )]
        enumFileFormat FileFormat { [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)0x40 ), DispId( 0x338 )] get; [param: In] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)0x40 ), DispId( 0x338 )] set; }
        [DispId( 0x33b )]
        string FileEncoding { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x33b ), TypeLibFunc( (short)0x40 )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x33b ), TypeLibFunc( (short)0x40 )] set; }
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 820 ), TypeLibFunc( (short)0x40 )]
        void SaveProjectOptions( [In, MarshalAs( UnmanagedType.IUnknown )] object StreamUnk );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x335 ), TypeLibFunc( (short)0x40 )]
        void LoadProjectOptions( [In, MarshalAs( UnmanagedType.IUnknown )] object StreamUnk );
        [DispId( 830 )]
        string SccProjectName { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 830 ), TypeLibFunc( (short)0x40 )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)0x40 ), DispId( 830 )] set; }
        [DispId( 0x33f )]
        string SccAuxPath { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)0x40 ), DispId( 0x33f )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)0x40 ), DispId( 0x33f )] set; }
        [DispId( 0x340 )]
        string SccLocalPath { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)0x40 ), DispId( 0x340 )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)0x40 ), DispId( 0x340 )] set; }
        [DispId( 0x341 )]
        string SccProvider { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)0x40 ), DispId( 0x341 )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x341 ), TypeLibFunc( (short)0x40 )] set; }
        [DispId( 0x342 )]
        string keyword { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x342 )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x342 )] set; }
        [DispId( 0x347 )]
        string OwnerKey { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x347 ), TypeLibFunc( (short)0x40 )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x347 ), TypeLibFunc( (short)0x40 )] set; }
        [DispId( 840 )]
        string ProjectGUID { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)0x40 ), DispId( 840 )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)0x40 ), DispId( 840 )] set; }
        [return: MarshalAs( UnmanagedType.IDispatch )]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x349 )]
        object AddAssemblyReference( [In, MarshalAs( UnmanagedType.BStr )] string Path );
        [return: MarshalAs( UnmanagedType.IDispatch )]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x34a )]
        object AddActiveXReference( [In, MarshalAs( UnmanagedType.BStr )] string typeLibGuid, [In] int majorVersion, [In] int minorVersion, [In] int localeID, [In, MarshalAs( UnmanagedType.BStr )] string wrapper );
        [return: MarshalAs( UnmanagedType.IDispatch )]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x34b )]
        object AddProjectReference( [In, MarshalAs( UnmanagedType.IDispatch )] object proj );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x34c )]
        bool CanAddAssemblyReference( [In, MarshalAs( UnmanagedType.BStr )] string bstrRef );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x34d )]
        bool CanAddActiveXReference( [In, MarshalAs( UnmanagedType.BStr )] string typeLibGuid, [In] int majorVersion, [In] int minorVersion, [In] int localeID, [In, MarshalAs( UnmanagedType.BStr )] string wrapper );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x34e )]
        bool CanAddProjectReference( [In, MarshalAs( UnmanagedType.IDispatch )] object proj );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x34f )]
        void RemoveReference( [In, MarshalAs( UnmanagedType.IDispatch )] object pDispRef );
        [DispId( 0x343 )]
        object VCReferences { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x343 ), TypeLibFunc( (short)0x400 )] get; }
        [DispId( 0x350 )]
        object References { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)0x400 ), DispId( 0x350 )] get; }
        [DispId( 0x351 )]
        object ReferencesConsumableByDesigners { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)0x400 ), DispId( 0x351 )] get; }
        [DispId( 0x344 )]
        string RootNamespace { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x344 )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x344 )] set; }
        [DispId( 0x345 )]
        object Object { [return: MarshalAs( UnmanagedType.IDispatch )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x345 )] get; }
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x356 )]
        void Version( out int major, out int minor );
        [DispId( 0x358 )]
        bool ShowAllFiles { [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x358 )] get; [param: In] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x358 )] set; }
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x359 )]
        void AddToolFile( [In, MarshalAs( UnmanagedType.Interface )] VCToolFile ToolFile );
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x35a )]
        void RemoveToolFile( [In, MarshalAs( UnmanagedType.Interface )] VCToolFile ToolFile );
        [DispId( 0x35b )]
        object ToolFiles { [return: MarshalAs( UnmanagedType.Interface )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x35b )] get; }
        [DispId( 0x35d )]
        string ManagedDBConnection { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x35d )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x35d )] set; }
        [DispId( 0x35e )]
        string ManagedDBProvider { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x35e )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x35e )] set; }
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x360 )]
        void MakeManagedDBConnection( [In, Optional] bool forceNew /* = false */);
        [DispId( 0x36c )]
        string AssemblyReferenceSearchPaths { [return: MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x36c )] get; [param: In, MarshalAs( UnmanagedType.BStr )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x36c )] set; }
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x36d )]
        void LoadUserFile();
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x36e )]
        void SaveUserFile();
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x36f )]
        void IncludeHeaderFile( [In, MarshalAs( UnmanagedType.BStr )] string HeaderFile, [In, MarshalAs( UnmanagedType.BStr )] string FileName );
    }

    [ComImport, TypeLibType( (short)0x1040 ), Guid( "31EFB5B1-C655-4ADA-BB52-3ED87FB2A4AE" ), DefaultMember( "Item" )]
    public interface Windows2 : Windows
    {
        [return: MarshalAs( UnmanagedType.Interface )]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0 )]
        Window Item( [In, MarshalAs( UnmanagedType.Struct )] object index );
        [DispId( 0xc9 )]
        int Count { [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0xc9 )] get; }
        [return: MarshalAs( UnmanagedType.CustomMarshaler, MarshalType = "", MarshalCookie = "" )]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), TypeLibFunc( (short)1 ), DispId( -4 )]
        IEnumerator GetEnumerator();
        [return: MarshalAs( UnmanagedType.Interface )]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 300 )]
        Window CreateToolWindow( [In, MarshalAs( UnmanagedType.Interface )] AddIn AddInInst,
            [In, MarshalAs( UnmanagedType.BStr )] string ProgID,
            [In, MarshalAs( UnmanagedType.BStr )] string Caption, [In, MarshalAs( UnmanagedType.BStr )] string GuidPosition, [In, Out, MarshalAs( UnmanagedType.IDispatch )] ref object DocObj );
        [DispId( 0x12d )]
        DTE DTE { [return: MarshalAs( UnmanagedType.Interface )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x12d )] get; }
        [DispId( 0x12e )]
        DTE Parent { [return: MarshalAs( UnmanagedType.Interface )] [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x12e )] get; }
        [return: MarshalAs( UnmanagedType.Interface )]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x12f )]
        Window CreateLinkedWindowFrame( [In, MarshalAs( UnmanagedType.Interface )] Window Window1, [In, MarshalAs( UnmanagedType.Interface )] Window Window2, [In] vsLinkedWindowType Link );
        [return: MarshalAs( UnmanagedType.Interface )]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 400 )]
        Window CreateToolWindow2( [MarshalAs( UnmanagedType.Interface )] AddIn Addin, [MarshalAs( UnmanagedType.BStr )] string Assembly, [MarshalAs( UnmanagedType.BStr )] string Class, [MarshalAs( UnmanagedType.BStr )] string Caption, [MarshalAs( UnmanagedType.BStr )] string GuidPosition, [In, Out, MarshalAs( UnmanagedType.IDispatch )] ref object ControlObject );
    }
 

}