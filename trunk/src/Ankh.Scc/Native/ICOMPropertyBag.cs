using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Ankh.Scc.Native
{
    /// <summary>
    /// IPropertyBag but then with parsable HResult
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComImport]
    [Guid("55272A00-42CB-11CE-8135-00AA004BB851")]
    interface ICOMPropertyBag
    {
        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int Read(string pszPropName, out object pVar, IErrorLog pErrorLog, uint VARTYPE, object pUnkObj);

        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int Write(string pszPropName, ref object pVar);
    }
}
