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
        public const uint TVSIL_NORMAL = 0x0;

        /// <summary>
        /// Indicates the state image list. You can use state images to indicate 
        /// application-defined item states. A state image is displayed to the 
        /// left of an item's selected or nonselected image.
        /// </summary>
        public const uint TVSIL_STATE = 0x2;


        // file attribute constants
        /// <summary>
        /// The file is a directory.
        /// </summary>
        public const uint FILE_ATTRIBUTE_DIRECTORY =  0x10;

               

        /// <summary>
        /// The pszText and cchTextMax members are valid.
        /// </summary>
        public const int   TVIF_TEXT = 0x0001;

        /// <summary>
        /// The iImage member is valid.
        /// </summary>
        public const int   TVIF_IMAGE = 0x0002;


        /// <summary>
        /// The lParam member is valid.
        /// </summary>
        public const int   TVIF_PARAM = 0x0004;

        /// <summary>
        /// The state and stateMask members are valid.
        /// </summary>
        public const int   TVIF_STATE = 0x0008;


        /// <summary>
        /// The hItem member is valid.
        /// </summary>
        public const int   TVIF_HANDLE = 0x0010;

        /// <summary>
        /// The iSelectedImage member is valid.
        /// </summary>
        public const int   TVIF_SELECTEDIMAGE = 0x0020;
        /// <summary>
        /// The cChildren member is valid.
        /// </summary>
        public const int   TVIF_CHILDREN = 0x0040;

        /// <summary>
        /// The iIntegral member is valid.
        /// </summary>
        public const int   TVIF_INTEGRAL = 0x0080;

        /// <summary>
        /// The tree-view control will retain the supplied information and 
        /// will not request it again. This flag is valid only when processing 
        /// the TVN_GETDISPINFO notification.
        /// </summary>
        public const int   TVIF_DI_SETITEM = 0x1000;

        /// <summary>
        /// Retrieves the topmost or very first item of the tree-view control. 
        /// </summary>
        public const int   TVGN_ROOT = 0x0000;

        /// <summary>
        /// Retrieves the next sibling item.
        /// </summary>
        public const int   TVGN_NEXT = 0x0001;

        /// <summary>
        /// Retrieves the previous sibling item.
        /// </summary>
        public const int   TVGN_PREVIOUS = 0x0002;

        /// <summary>
        /// Retrieves the parent of the specified item. 
        /// </summary>
        public const int   TVGN_PARENT = 0x0003;

        /// <summary>
        /// Retrieves the first child item of the item 
        /// specified by the hitem parameter. 
        /// </summary>
        public const int   TVGN_CHILD = 0x0004;

        /// <summary>
        /// Retrieves the first item that is visible in the tree-view window. 
        /// </summary>
        public const int   TVGN_FIRSTVISIBLE = 0x0005;

        /// <summary>
        /// Retrieves the next visible item that follows the specified item. The specified 
        /// item must be visible. 
        /// Use the TVM_GETITEMRECT message to determine whether an item is visible.
        /// </summary>
        public const int   TVGN_NEXTVISIBLE = 0x0006;

        /// <summary>
        /// Retrieves the first visible item that precedes the specified item. 
        /// The specified item must be visible. Use the TVM_GETITEMRECT message 
        /// to determine whether an item is visible.
        /// </summary>
        public const int   TVGN_PREVIOUSVISIBLE = 0x0007;

        /// <summary>
        /// Retrieves the item that is the target of a drag-and-drop operation. 
        /// </summary>
        public const int   TVGN_DROPHILITE = 0x0008;

        /// <summary>
        /// Retrieves the currently selected item. 
        /// </summary>
        public const int   TVGN_CARET = 0x0009;

        /// <summary>
        /// Version 4.71. Retrieves the last expanded item in the tree. 
        /// This does not retrieve the last item visible in the tree-view window.
        /// </summary>
        public const int   TVGN_LASTVISIBLE = 0x000A;


        /// <summary>
        /// Mask for the bits used to specify the item's state image index. 
        /// </summary>
        public const int TVIS_STATEIMAGEMASK = 61440;

	}
}
