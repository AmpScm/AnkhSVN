// $Id$
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Security.Permissions;
using System;
using Utils.Win32;
using System.Runtime.InteropServices;
using System.Text;

// shamelessly stolen from Q306285 @ http://support.microsoft.com/default.aspx?scid=kb;en-us;306285


namespace Utils
{
    /// <summary>
    /// Component wrapping access to the Browse For Folder common dialog box.
    /// Call the ShowDialog() method to bring the dialog box up.
    /// </summary>
    public sealed class FolderBrowser : Component
    {
        /// <summary>
        /// Helper function that returns the IMalloc interface used by the shell.
        /// </summary>
        private static Win32.IMalloc GetSHMalloc ( )
        {
            IMalloc malloc;
            Win32.Win32.SHGetMalloc ( out malloc );
            return malloc;
        }

        /// <summary>
        /// Shows the folder browser dialog box.
        /// </summary>
        public DialogResult ShowDialog ( ) 
        {
            return ShowDialog ( null );
        }

        /// <summary>
        /// Shows the folder browser dialog box with the specified owner window.
        /// </summary>
        public DialogResult ShowDialog ( IWin32Window owner ) 
        {
            IntPtr pidlRoot = IntPtr.Zero;

            // Get/find an owner HWND for this dialog.
            IntPtr hWndOwner;

            if ( owner != null ) 
            {
                hWndOwner = owner.Handle;
            }
            else 
            {
                hWndOwner = Win32.Win32.GetActiveWindow ( );
            }

            // Get the IDL for the specific startLocation.
            Win32.Win32.SHGetSpecialFolderLocation ( hWndOwner, (int) startLocation, out pidlRoot );

            if (pidlRoot == IntPtr.Zero) 
            {
                return DialogResult.Cancel;
            }

            int mergedOptions = (int)publicOptions | (int)privateOptions;

            if ( ( mergedOptions & (int)BffStyles.NewDialogStyle ) != 0 ) 
            {
                if ( System.Threading.ApartmentState.MTA == Application.OleRequired ( ) )
                    mergedOptions = mergedOptions & (~ (int)BffStyles.NewDialogStyle);
            }

            IntPtr pidlRet = IntPtr.Zero;

            try 
            {
                // Construct a BROWSEINFO.
                BROWSEINFO bi = new BROWSEINFO ( );
                IntPtr buffer = Marshal.AllocHGlobal ( MAX_PATH);

                bi.pidlRoot = pidlRoot;
                bi.hwndOwner = hWndOwner;
                bi.pszDisplayName = buffer;
                bi.lpszTitle = descriptionText;
                bi.ulFlags = mergedOptions;
                // The rest of the fields are initialized to zero by the constructor.
                // bi.lpfn = null;  bi.lParam = IntPtr.Zero;    bi.iImage = 0;

                // Show the dialog.
                pidlRet = Win32.Win32.SHBrowseForFolder ( ref bi );

                // Free the buffer you've allocated on the global heap.
                Marshal.FreeHGlobal ( buffer );

                if ( pidlRet == IntPtr.Zero ) 
                {
                    // User clicked Cancel.
                    return DialogResult.Cancel;
                }

                // Then retrieve the path from the IDList.
                StringBuilder sb = new StringBuilder ( MAX_PATH );
                if ( 0 == Win32.Win32.SHGetPathFromIDList ( pidlRet, sb ) )
                {
                    return DialogResult.Cancel;
                }

                // Convert to a string.
                directoryPath = sb.ToString ( );
            }
            finally 
            {
                Win32.IMalloc malloc = GetSHMalloc ( );
                malloc.Free ( pidlRoot );

                if ( pidlRet != IntPtr.Zero ) 
                {
                    malloc.Free ( pidlRet );
                }
            }

            return DialogResult.OK;
        }

        /// <summary>
        /// Helper function used to set and reset bits in the publicOptions bitfield.
        /// </summary>
        private void SetOptionField ( int mask, bool turnOn )
        {
            if (turnOn)
                publicOptions |= mask;
            else
                publicOptions &= ~mask;
        }

        /// <summary>
        /// Only return file system directories. If the user selects folders
        /// that are not part of the file system, the OK button is unavailable.
        /// </summary>
        [Category ( "Navigation" )]
        [Description ( "Only return file system directories. If the user selects folders " +
             "that are not part of the file system, the OK button is unavailable." )]
        [DefaultValue ( true )]
        public bool OnlyFilesystem
        {
            get 
            {
                return (publicOptions & (int) BffStyles.RestrictToFilesystem) != 0;
            }
            set 
            {
                SetOptionField ( (int) BffStyles.RestrictToFilesystem, value );
            }
        }

        /// <summary>
        /// Location of the root folder from which to start browsing. Only the specified
        /// folder and any folders beneath it in the namespace hierarchy  appear
        /// in the dialog box.
        /// </summary>
        [Category ( "Navigation" )]
        [Description ( "Location of the root folder from which to start browsing. Only the specified " +
             "folder and any folders beneath it in the namespace hierarchy appear " +
             "in the dialog box." )]
        [DefaultValue ( typeof(FolderID), "0")]
        public FolderID StartLocation 
        {
            get 
            {
                return startLocation;
            }
            set 
            {
                new UIPermission ( UIPermissionWindow.AllWindows ).Demand ( );
                startLocation = value;
            }
        }

        /// <summary>
        /// Full path to the folder selected by the user.
        /// </summary>
        [Category("Navigation")]
        [Description("Full path to the folder slected by the user.")]
        public string DirectoryPath 
        {
            get 
            {
                return directoryPath;
            }
        } 



        private static readonly int MAX_PATH = 260;
    
        // Root node of the tree view.
        private FolderID startLocation = FolderID.Desktop;

        // Browse info options.
        private int publicOptions = (int) BffStyles.RestrictToFilesystem |
            (int) BffStyles.RestrictToDomain;
        private int privateOptions = (int) BffStyles.NewDialogStyle;

        // Description text to show.
        private string descriptionText = "Please select a folder below:";

        // Folder chosen by the user.
        private string directoryPath = String.Empty;

        /// <summary>
        /// Enum of CSIDLs identifying standard shell folders.
        /// </summary>
        public enum FolderID 
        {
            Desktop                   = 0x0000,
            Printers                  = 0x0004,
            MyDocuments               = 0x0005,
            Favorites                 = 0x0006,
            Recent                    = 0x0008,
            SendTo                    = 0x0009,
            StartMenu                 = 0x000b,
            MyComputer                = 0x0011,
            NetworkNeighborhood       = 0x0012,
            Templates                 = 0x0015,
            MyPictures                = 0x0027,
            NetAndDialUpConnections   = 0x0031,
        }
    }

}
