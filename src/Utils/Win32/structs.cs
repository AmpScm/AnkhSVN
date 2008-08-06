// $Id$
using System;
using System.Runtime.InteropServices;

namespace Utils.Win32
{
    /// <summary>
    /// Represents the SHFILEINFO struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential), CLSCompliant(false)]
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
}
