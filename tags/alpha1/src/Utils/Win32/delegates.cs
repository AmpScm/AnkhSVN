// $Id: Win32.cs 422 2003-04-20 20:16:46Z Arild $
using System;

namespace Utils.Win32
{
    /// <summary>
    /// A delegate to be used for SetWindowsHookEx
    /// </summary>
	public delegate int HOOKPROC( int code, IntPtr wParam, 
        CWPSTRUCT message );

    // Delegate type used in BROWSEINFO.lpfn field.
    public delegate int BFFCALLBACK ( IntPtr hwnd, uint uMsg, IntPtr lParam, IntPtr lpData );
}
