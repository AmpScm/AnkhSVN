#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Copyright © 2005 Bill Menees.  All rights reserved.
	Bill@Menees.com

	$History: ItemClickEventArgs.cs $
	
	*****************  Version 1  *****************
	User: Bill         Date: 11/06/05   Time: 12:54p
	Created in $/CSharp/Menees/Classes
	Pulled out of the RecentItemList.cs file.

-----------------------------------------------------------------------------*/

#endregion

#region Using Directives

using System;
using System.ComponentModel;

#endregion

namespace Ankh.Diff
{
    [Description("A delegate type for hooking up RecentItemList.ItemClick notifications.")]
    public delegate void ItemClickEventHandler(object sender, ItemClickEventArgs e);

    [Description("The arguments for the RecentItemList.ItemClick event.")]
    public sealed class ItemClickEventArgs : EventArgs
    {
        #region Constructors

        internal ItemClickEventArgs(string strItemName, string[] arStrings)
        {
            m_strItemName = strItemName;
            m_arStrings = arStrings;
        }

        #endregion

        #region Public Properties

        public string ItemName
        {
            get { return m_strItemName; }
        }

        public string[] Strings
        {
            get { return m_arStrings; }
        }

        #endregion

        #region Private Data Members

        private string m_strItemName;
        private string[] m_arStrings;

        #endregion
    }
}
