using System;
using System.Runtime.InteropServices;

namespace Utils.Win32
{
	/// <summary>
	/// Represents the SHFILEINFO struct
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct SHFILEINFO
	{
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
	}

    /// <summary>
    /// Represents a Win32 RECT
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    /// <summary>
    /// Represents a Win32 TVITEMEX struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
    public class TVITEMEX
    {
        public uint mask;
        public IntPtr hItem;
        public uint state;
        public uint stateMask;
        public string pszText;
        public int cchTextMax;
        public int iImage;
        public int iSelectedImage;
        public int cChildren;
        public int lParam;
        public int iIntegral;
    }

    /// <summary>
    /// Represents a Win32 IMAGEINFO struct.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGEINFO
    {
        public IntPtr hbmImage;
        public IntPtr hbmMask;
        public int unused1;
        public int unused2;
        public RECT rect;
    }
}
