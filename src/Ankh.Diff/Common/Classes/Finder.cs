#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Copyright © 2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	$History: Finder.cs $
	
	*****************  Version 2  *****************
	User: Bill         Date: 11/06/05   Time: 12:51p
	Updated in $/CSharp/Menees/Classes
	FindNext and FindPrevious wouldn't work with a custom IFindDlg.

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	5.31.2003	Created.

-----------------------------------------------------------------------------*/

#endregion

#region Using Directives

using System;
using System.Windows.Forms;

#endregion

namespace Ankh.Diff
{
    //This class isn't sealed, so you can inherit from it
    //and add your own data members.  Then you can use a
    //custom IFindDlg implementation to edit your data.
    public class FindData
    {
        #region Public Properties

        public string Text { get { return m_strText; } set { m_strText = value; } }
        public bool MatchCase { get { return m_bMatchCase; } set { m_bMatchCase = value; } }
        public bool SearchUp { get { return m_bSearchUp; } set { m_bSearchUp = value; } }

        #endregion

        #region Private Data Members

        private string m_strText = "";
        private bool m_bMatchCase;
        private bool m_bSearchUp;

        #endregion
    }
}
