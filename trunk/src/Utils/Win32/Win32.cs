using System;
using System.Runtime.InteropServices;
using Utils.Win32.Structs;

namespace Utils.Win32
{
	/// <summary>
	/// Contains P/Invoke declarations for Win32 API calls.
	/// </summary>
	public abstract class Win32
	{
        /// <summary>
        /// Private ctor to avoid instantiation.
        /// </summary>
		private Win32()
		{			
		}

        /// <summary>
        /// Retrieves information about an object in the file system, 
        /// such as a file, a folder, a directory, or a drive root.
        /// </summary>
        /// <param name="cbSizeFileInfo">Size, in bytes, of the SHFILEINFO structure pointed to by 
        /// the psfi parameter. </param>
        /// <param name="dwFileAttributes"> Combination of one or more file attribute flags 
        /// (FILE_ATTRIBUTE_ values). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, 
        /// this parameter is ignored.</param>
        /// <param name="psfi"> Address of a SHFILEINFO structure to receive the file information.</param>
        /// <param name="pszPath">Pointer to a null-terminated string of maximum length MAX_PATH 
        /// that contains the path and file name. Both absolute and relative paths are valid. 
        /// If the uFlags parameter includes the SHGFI_PIDL flag, this parameter must 
        /// be the address of an ITEMIDLIST (PIDL) structure that contains the list of 
        /// item identifiers that uniquely identify the file within the Shell's namespace. 
        /// The PIDL must be a fully qualified PIDL. Relative PIDLs are not allowed. 
        /// If the uFlags parameter includes the SHGFI_USEFILEATTRIBUTES flag, 
        /// this parameter does not have to be a valid file name. The function 
        /// proceeds as if the file exists with the specified name and with the file 
        /// attributes passed in the dwFileAttributes parameter. This enables you to obtain 
        /// information about a file type by passing just the extension for pszPath and 
        /// passing FILE_ATTRIBUTE_NORMAL in dwFileAttributes.
        /// This string can use either short (the 8.3 form) or long file names.
        /// </param>
        /// <param name="uFlags"></param>
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, 
            ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
    

        /// <summary>
        /// The SendMessage function sends the specified message to a window or windows. 
        /// It calls the window procedure for the specified window and does not 
        /// return until the window procedure has processed the message. 
        /// To send a message and return immediately, use the SendMessageCallback or 
        /// SendNotifyMessage function. To post a message to a thread's message queue 
        /// and return immediately, use the PostMessage or PostThreadMessage function. 
        /// </summary>
        /// <param name="hWnd"> Handle to the window whose window procedure will receive the message. 
        /// If this parameter is HWND_BROADCAST, the message is sent to all top-level 
        /// windows in the system, including disabled or invisible unowned windows, 
        /// overlapped windows, and pop-up windows; but the message is not sent 
        /// to child windows. </param>
        /// <param name="msg">Specifies the message to be sent.</param>
        /// <param name="wParam">Specifies additional message-specific information.</param>
        /// <param name="lParam">Specifies additional message-specific information.</param>
        /// <returns>The return value specifies the result of the message processing; 
        /// it depends on the message sent.</returns>
        [DllImport("User32.dll")]
        public static extern int SendMessage( IntPtr hWnd, uint msg, IntPtr wParam, 
            IntPtr lParam );

        /// <summary>
        /// The SendMessage function sends the specified message to a window or windows. 
        /// It calls the window procedure for the specified window and does not 
        /// return until the window procedure has processed the message. 
        /// To send a message and return immediately, use the SendMessageCallback or 
        /// SendNotifyMessage function. To post a message to a thread's message queue 
        /// and return immediately, use the PostMessage or PostThreadMessage function. 
        /// </summary>
        /// <param name="hWnd"> Handle to the window whose window procedure will receive the message. 
        /// If this parameter is HWND_BROADCAST, the message is sent to all top-level 
        /// windows in the system, including disabled or invisible unowned windows, 
        /// overlapped windows, and pop-up windows; but the message is not sent 
        /// to child windows. </param>
        /// <param name="msg">Specifies the message to be sent.</param>
        /// <param name="wParam">Specifies additional message-specific information.</param>
        /// <param name="lParam">Specifies additional message-specific information.</param>
        /// <returns>The return value specifies the result of the message processing; 
        /// it depends on the message sent.</returns>
        [DllImport("User32.dll")]
        public static extern int SendMessage( IntPtr hWnd, uint msg, uint wParam, 
            uint lParam );

        /// <summary>
        /// The SendMessage function sends the specified message to a window or windows. 
        /// It calls the window procedure for the specified window and does not 
        /// return until the window procedure has processed the message. 
        /// To send a message and return immediately, use the SendMessageCallback or 
        /// SendNotifyMessage function. To post a message to a thread's message queue 
        /// and return immediately, use the PostMessage or PostThreadMessage function. 
        /// </summary>
        /// <param name="hWnd"> Handle to the window whose window procedure will receive the message. 
        /// If this parameter is HWND_BROADCAST, the message is sent to all top-level 
        /// windows in the system, including disabled or invisible unowned windows, 
        /// overlapped windows, and pop-up windows; but the message is not sent 
        /// to child windows. </param>
        /// <param name="msg">Specifies the message to be sent.</param>
        /// <param name="wParam">Specifies additional message-specific information.</param>
        /// <param name="lParam">Specifies additional message-specific information.</param>
        /// <returns>The return value specifies the result of the message processing; 
        /// it depends on the message sent.</returns>
        [DllImport("User32.dll")]
        public static extern int SendMessage( IntPtr hWnd, uint msg, IntPtr wParam, 
            uint lParam );

        /// <summary>
        /// The SendMessage function sends the specified message to a window or windows. 
        /// It calls the window procedure for the specified window and does not 
        /// return until the window procedure has processed the message. 
        /// To send a message and return immediately, use the SendMessageCallback or 
        /// SendNotifyMessage function. To post a message to a thread's message queue 
        /// and return immediately, use the PostMessage or PostThreadMessage function. 
        /// </summary>
        /// <param name="hWnd"> Handle to the window whose window procedure will receive the message. 
        /// If this parameter is HWND_BROADCAST, the message is sent to all top-level 
        /// windows in the system, including disabled or invisible unowned windows, 
        /// overlapped windows, and pop-up windows; but the message is not sent 
        /// to child windows. </param>
        /// <param name="msg">Specifies the message to be sent.</param>
        /// <param name="wParam">Specifies additional message-specific information.</param>
        /// <param name="lParam">Specifies additional message-specific information.</param>
        /// <returns>The return value specifies the result of the message processing; 
        /// it depends on the message sent.</returns>
        [DllImport("User32.dll")]
        public static extern int SendMessage( IntPtr hWnd, uint msg, uint wParam, 
            IntPtr lParam );
    
    }
}
