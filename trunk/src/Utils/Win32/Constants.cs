// $Id$
using System;

namespace Utils.Win32
{
    /// <summary>
    /// Contains Win32 constants.
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Private ctor to avoid instantiation.
        /// </summary>
        private Constants()
        {
        }

        // SHGetFileInfo constants

        /// <summary>
        /// Retrieve the handle to the icon that represents the file and the index of 
        /// the icon within the system image list. The handle is copied to the hIcon 
        /// member of the structure specified by psfi, and the index is copied 
        /// to the iIcon member. 
        /// </summary>
        public const uint SHGFI_ICON = 0x100;


        /// <summary>
        /// Modify SHGFI_ICON, causing the function to retrieve the file's large icon. 
        /// The SHGFI_ICON flag must also be set. 
        /// </summary>
        public const uint SHGFI_LARGEICON = 0x0; // 'Large icon

        /// <summary>
        /// Modify SHGFI_ICON, causing the function to retrieve the file's small icon. 
        /// The SHGFI_ICON flag must also be set. 
        /// </summary>
        public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

        /// <summary>
        /// Modify SHGFI_ICON, causing the function to retrieve a Shell-sized icon. 
        /// If this flag is not specified the function sizes the icon according 
        /// to the system metric values. The SHGFI_ICON flag must also be set. 
        /// </summary>
        public const uint SHGFI_SHELLICONSIZE = 0x4;

        /// <summary>
        /// Indicates that the function should not attempt to access the file specified by pszPath. 
        /// Rather, it should act as if the file specified by pszPath exists with the file 
        /// attributes passed in dwFileAttributes. This flag cannot be combined with the 
        /// SHGFI_ATTRIBUTES, SHGFI_EXETYPE, or SHGFI_PIDL flags. 
        /// </summary>
        public const uint SHGFI_USEFILEATTRIBUTES = 0x10;

        /// <summary>
        /// Modify SHGFI_ICON, causing the function to retrieve the file's open icon. 
        /// A container object displays an open icon to indicate that the container is open. 
        /// The SHGFI_ICON flag must also be set. 
        /// </summary>
        public const uint SHGFI_OPENICON = 0x2;

        /// <summary>
        /// Retrieve the index of a system image list icon. If successful, 
        /// the index is copied to the iIcon member of psfi. The return value is a 
        /// handle to the system image list. Only those images whose indices are 
        /// successfully copied to iIcon are valid. 
        /// Attempting to access other images in the system image list will result in 
        /// undefined behavior. 
        /// </summary>
        public const uint SHGFI_SYSICONINDEX = 16384;


        // Tree view image list constants
        /// <summary>
        /// Indicates the normal image list, which contains selected, nonselected, 
        /// and overlay images for the items of a tree-view control.
        /// </summary>
        public static readonly IntPtr TVSIL_NORMAL = (IntPtr)0x0;

        /// <summary>
        /// Indicates the state image list. You can use state images to indicate 
        /// application-defined item states. A state image is displayed to the 
        /// left of an item's selected or nonselected image.
        /// </summary>
        public static readonly IntPtr TVSIL_STATE = (IntPtr)0x2;


        public static readonly IntPtr LVSIL_NORMAL = (IntPtr)0x0;
        public static readonly IntPtr LVSIL_SMALL = (IntPtr)0x1;
        public static readonly IntPtr LVSIL_STATE = (IntPtr)0x2;

        // file attribute constants
        /// <summary>
        /// The file is a directory.
        /// </summary>
        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
    }
}
