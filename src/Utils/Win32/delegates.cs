using System;

namespace Utils.Win32
{
    /// <summary>
    /// A delegate to be used for SetWindowsHookEx
    /// </summary>
	public delegate int HOOKPROC( int code, IntPtr wParam, 
        CWPSTRUCT message );
}
