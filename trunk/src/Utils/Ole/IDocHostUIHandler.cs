// $Id$
// P/Invoke declarations for OLE taken from 
// http://www.faisoncomputing.com/samples/programming_samples.htm
// with permission from the author
using System;
using System.Windows.Forms;
using mshtml;
using System.Runtime.InteropServices;

namespace Utils.Ole
{
  public enum DOCHOSTUITYPE
  {
    DOCHOSTUITYPE_BROWSE = 0,
    DOCHOSTUITYPE_AUTHOR = 1
  }

  public enum DOCHOSTUIDBLCLK
  {
    DOCHOSTUIDBLCLK_DEFAULT = 0,
    DOCHOSTUIDBLCLK_SHOWPROPERTIES = 1,
    DOCHOSTUIDBLCLK_SHOWCODE = 2
  }

  public enum DOCHOSTUIFLAG
  {
    DOCHOSTUIFLAG_DIALOG = 0x00000001,
    DOCHOSTUIFLAG_DISABLE_HELP_MENU = 0x00000002,
    DOCHOSTUIFLAG_NO3DBORDER = 0x00000004,
    DOCHOSTUIFLAG_SCROLL_NO = 0x00000008,
    DOCHOSTUIFLAG_DISABLE_SCRIPT_INACTIVE = 0x00000010,
    DOCHOSTUIFLAG_OPENNEWWIN = 0x00000020,
    DOCHOSTUIFLAG_DISABLE_OFFSCREEN = 0x00000040,
    DOCHOSTUIFLAG_FLAT_SCROLLBAR = 0x00000080,
    DOCHOSTUIFLAG_DIV_BLOCKDEFAULT = 0x00000100,
    DOCHOSTUIFLAG_ACTIVATE_CLIENTHIT_ONLY = 0x00000200,
    DOCHOSTUIFLAG_OVERRIDEBEHAVIORFACTORY = 0x00000400,
    DOCHOSTUIFLAG_CODEPAGELINKEDFONTS = 0x00000800,
    DOCHOSTUIFLAG_URL_ENCODING_DISABLE_UTF8 = 0x00001000,
    DOCHOSTUIFLAG_URL_ENCODING_ENABLE_UTF8 = 0x00002000,
    DOCHOSTUIFLAG_ENABLE_FORMS_AUTOCOMPLETE = 0x00004000,
    DOCHOSTUIFLAG_ENABLE_INPLACE_NAVIGATION = 0x00010000,
    DOCHOSTUIFLAG_IME_ENABLE_RECONVERSION = 0x00020000,
    DOCHOSTUIFLAG_THEME = 0x00040000,
    DOCHOSTUIFLAG_NOTHEME = 0x00080000,
    DOCHOSTUIFLAG_NOPICS = 0x00100000,
    DOCHOSTUIFLAG_NO3DOUTERBORDER = 0x00200000,
    DOCHOSTUIFLAG_DELEGATESIDOFDISPATCH = 0x00400000
  }

  [ StructLayout( LayoutKind.Sequential )]
  public struct DOCHOSTUIINFO
  {
    public uint cbSize;
    public uint dwFlags;
    public uint dwDoubleClick;
    [MarshalAs(UnmanagedType.BStr)] public string pchHostCss;
    [MarshalAs(UnmanagedType.BStr)] public string pchHostNS;
  }

  [StructLayout( LayoutKind.Sequential )]
  public struct tagMSG
  {
    public IntPtr hwnd;
    public uint message;
    public uint wParam;
    public int lParam;
    public uint time;
    public tagPOINT pt;
  }

  [ComImport(),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
  GuidAttribute("bd3f23c0-d43e-11cf-893b-00aa00bdce1a")]
  public interface IDocHostUIHandler
  {
    [PreserveSig]
    uint ShowContextMenu(uint dwID, ref tagPOINT ppt,
                        [MarshalAs(UnmanagedType.IUnknown)]  object pcmdtReserved,
                        [MarshalAs(UnmanagedType.IDispatch)] object pdispReserved);

    void GetHostInfo(ref DOCHOSTUIINFO pInfo);
    void ShowUI(uint dwID, ref object pActiveObject, ref object pCommandTarget, ref object pFrame, ref object pDoc);
    void HideUI();
    void UpdateUI();
    void EnableModeless(int fEnable);
    void OnDocWindowActivate(int fActivate);
    void OnFrameWindowActivate(int fActivate);
    void ResizeBorder(ref tagRECT prcBorder, int pUIWindow, int fRameWindow);

    [PreserveSig]
    uint TranslateAccelerator(ref tagMSG lpMsg, ref Guid pguidCmdGroup, uint nCmdID);

    void GetOptionKeyPath([MarshalAs(UnmanagedType.BStr)] ref string pchKey, uint dw);
    object GetDropTarget(ref object pDropTarget);

    [PreserveSig]
    void GetExternal([MarshalAs(UnmanagedType.IDispatch)] out object ppDispatch);

    [PreserveSig]
    uint TranslateUrl(uint dwTranslate, 
                      [MarshalAs(UnmanagedType.BStr)] string pchURLIn,
                      [MarshalAs(UnmanagedType.BStr)] ref string ppchURLOut);

    IDataObject FilterDataObject(IDataObject pDO);
  }

  [ComImport(),
  GuidAttribute("3050f6d0-98b5-11cf-bb82-00aa00bdce0b")]
  public interface IDocHostUIHandler2: IDocHostUIHandler
  {
    void GetOverrideKeyPath([MarshalAs(UnmanagedType.BStr)] ref string pchKey, uint dw);
  }
}
