// $Id$
using System;

namespace Utils.Win32
{
    /// <summary>
    /// Contains Win32 messages.
    /// </summary>
    public static class Msg
    {
        /// <summary>
        /// Sets the normal or state image list for a tree-view control 
        /// and redraws the control using the new images. 
        /// </summary>
        /// <param name="wParam">Type of image list to set. 
        /// This parameter can be one of the following values: TVSIL_NORMAL or TVSIL_STATE</param>; 
        /// <param name="lParam">Handle to the image list. If himl is NULL, the message 
        /// removes the specified image list from the tree-view control.</param>         /// 
        /// <return>Returns the handle to the previous image list, if any, or NULL otherwise.</return>
        public const uint TVM_SETIMAGELIST = 0x1109;

        /// <summary>
        /// Retrieves the handle to the normal 
        /// or state image list associated with a tree-view control. 
        /// </summary>
        public const uint TVM_GETIMAGELIST = 4360;

        /// <summary>
        /// Sets an imagelist for a listview.
        /// </summary>
        public const uint LVM_SETIMAGELIST = 0x1003;
    }
}
