using System;

namespace Utils.Win32
{
	/// <summary>
	/// Contains Win32 messages.
	/// </summary>
	public class Msg
	{
        /// <summary>
        /// Private ctor to avoid instantiation.
        /// </summary>
		private Msg()
		{			
		}

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
	}
}
