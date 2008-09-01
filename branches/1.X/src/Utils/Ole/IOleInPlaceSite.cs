// $Id$
// P/Invoke declarations for OLE taken from 
// http://www.faisoncomputing.com/samples/programming_samples.htm
// with permission from the author
using System;
using System.Runtime.InteropServices;
using mshtml;

namespace Utils.Ole
{
  [ComImport(),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
  GuidAttribute("00000114-0000-0000-C000-000000000046")]
  public interface IOleWindow
  {
    void GetWindow(out IntPtr phwnd);
    void ContextSensitiveHelp(int fEnterMode);
  }

  [ComImport(),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
  GuidAttribute("00000119-0000-0000-C000-000000000046")]
  public interface IOleInPlaceSite: IOleWindow
  {
    void CanInPlaceActivate();
    void OnInPlaceActivate();
    void OnUIActivate();
    void GetWindowContext([Out, MarshalAs(UnmanagedType.IDispatch)] out object ppFrame,
      [Out, MarshalAs(UnmanagedType.IDispatch)] out object ppDoc,
      out tagRECT lprcPosRect,
      out tagRECT  lprcClipRect,
      ref IntPtr lpFrameInfo);
    void Scroll(tagSIZE scrollExtant);
    void OnUIDeactivate(int fUndoable);
    void OnInPlaceDeactivate();
    void DiscardUndoState();
    void DeactivateAndUndo();
    void OnPosRectChange(ref tagRECT lprcPosRect);
  }
  
}
