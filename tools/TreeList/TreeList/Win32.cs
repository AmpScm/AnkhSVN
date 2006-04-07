using System;
using System.Text;
using System.Runtime.InteropServices;

namespace TreeList.Win32
{
    
    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto, Pack=1 )]
    internal struct LVITEM
    {
        public int mask;
        public int iItem;
        public int iSubItem;
        public uint state;
        public uint stateMask;
        public IntPtr pszText;
        public int cchTextMax;
        public int iImage;
        public IntPtr lParam;
        public int iIndent;
        public int iGroupId;
        public int cColumns;
        public int puColumns;
    }

    internal class ListViewConstants
    {
        public const int LVIF_INDENT = 0x10;
        public const int LVM_SETITEMA = 4102;
        public const int LVM_GETITEMA = 0x1005;
        public const int LVM_SUBITEMHITTEST = 4153;
        public const int LVHT_ONITEMICON = 0x2;
        public const int LVHT_ONITEMSTATEICON = 0x8;
    }

    internal class Functions
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage( IntPtr hWnd, uint message, int wParam, out LVITEM lvItem);

        [DllImport( "user32.dll" )]
        public static extern int SendMessage( IntPtr hWnd, uint message, int wParam, out LVHITTESTINFO lvItem );
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct LVHITTESTINFO
    {
        public POINT pt;
        public uint flags;
        public int iItem;
        public int iSubItem;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct POINT
    {
        public int x;
        public int y;
    }
 

}
