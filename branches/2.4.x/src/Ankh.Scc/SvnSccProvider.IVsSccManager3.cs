using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Shell.Interop;
using System;
namespace Ankh.Scc
{
    // From Microsoft.VisualStudio.Shell.Interop.11.0
    [CLSCompliant(false)]
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("224209ED-E56C-4C8D-A7FF-31CF4686798D")]
    public interface ICOMVsSccManager3 : IVsSccManager2
    {
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new int RegisterSccProject([In, MarshalAs(UnmanagedType.Interface)] IVsSccProject2 pscp2Project, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string pszSccProjectName, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string pszSccAuxPath, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string pszSccLocalPath, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPWStr)] string pszProvider);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new int UnregisterSccProject([In, MarshalAs(UnmanagedType.Interface)] IVsSccProject2 pscp2Project);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new int GetSccGlyph([In] int cFiles, [In, ComAliasName("OLE.LPCOLESTR"), MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0)] string[] rgpszFullPaths, [Out, ComAliasName("VsShell.VsStateIcon"), MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] VsStateIcon[] rgsiGlyphs, [Out, ComAliasName("OLE.DWORD"), MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] uint[] rgdwSccStatus);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new int GetSccGlyphFromStatus([In, ComAliasName("OLE.DWORD")] uint dwSccStatus, [Out, ComAliasName("VsShell.VsStateIcon"), MarshalAs(UnmanagedType.LPArray)] VsStateIcon[] psiGlyph);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new int IsInstalled([ComAliasName("OLE.BOOL")] out int pbInstalled);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new int BrowseForProject([MarshalAs(UnmanagedType.BStr)] out string pbstrDirectory, [ComAliasName("OLE.BOOL")] out int pfOK);
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new int CancelAfterBrowseForProject();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        bool IsBSLSupported();
    }

    partial class SvnSccProvider : ICOMVsSccManager3
    {
        public bool IsBSLSupported()
        {
            return true;
        }
    }
}