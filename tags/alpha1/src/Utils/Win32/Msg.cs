// $Id$
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

        /// <summary>
        /// Retrieves the handle to the normal 
        /// or state image list associated with a tree-view control. 
        /// </summary>
        public const uint TVM_GETIMAGELIST = 4360;


        /// <summary>
        /// Retrieves some or all of a tree-view item's attributes. 
        /// </summary>
        public const uint TVM_GETITEM = 4364;

        /// <summary>
        /// The TVM_SETITEM message sets some or all of a tree-view item's attributes.
        /// </summary>
        public const uint TVM_SETITEM = 4365;

        /// <summary>
        /// Retrieves the tree-view item that bears the specified relationship to 
        /// a specified item. 
        /// </summary>
        public const uint TVM_GETNEXTITEM = 4362;
	}
}
