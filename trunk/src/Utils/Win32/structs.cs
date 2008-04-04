// $Id$
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

    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STARTUP_INFO
    {
        public int cb;
        public string reserved;
        public string desktop;
        public string title;
        public int wX;
        public int wY;
        public int xSize;
        public int ySize;
        public int xCountChars;
        public int yCountChars;
        public int fillAttributes;
        public int flags;
        public short showWindow;
        public short cbReserved2;
        public byte lpReserved2;
        public IntPtr stdInput;
        public IntPtr stdOutput;
        public IntPtr stdError;
    }
}
