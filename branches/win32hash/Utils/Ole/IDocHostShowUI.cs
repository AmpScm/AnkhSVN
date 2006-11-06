// $Id$
// P/Invoke declarations for OLE taken from 
// http://www.faisoncomputing.com/samples/programming_samples.htm
// with permission from the author
using System;
using mshtml;
using System.Runtime.InteropServices;

namespace Utils.Ole
{
  [ComImport,
  Guid("C4D244B0-D43E-11CF-893B-00AA00BDCE1A"),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown) ]
  public interface IDocHostShowUI
  {
   [PreserveSig]
    uint ShowMessage(IntPtr hwnd, 
                     [MarshalAs(UnmanagedType.BStr)] string lpstrText, 
                     [MarshalAs(UnmanagedType.BStr)] string lpstrCaption, 
                     uint dwType, 
                     [MarshalAs(UnmanagedType.BStr)] string lpstrHelpFile, 
                     uint dwHelpContext,
                     out int lpResult);
                                  
    [PreserveSig]
    uint ShowHelp(IntPtr hwnd, [MarshalAs(UnmanagedType.BStr)] string pszHelpFile, 
                  uint uCommand, uint dwData, 
                  tagPOINT ptMouse, 
                  [MarshalAs(UnmanagedType.IDispatch)] object pDispatchObjectHit);                       
  }
}
