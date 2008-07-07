#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Copyright © 2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	$History: TextBoxFinder.cs $
	
	*****************  Version 2  *****************
	User: Bill         Date: 11/06/05   Time: 12:55p
	Updated in $/CSharp/Menees/Classes
	Updated #regions.

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
    public class TextBoxFinder : Finder
    {
        #region Constructors

        public TextBoxFinder(TextBoxBase Edit)
        {
            m_Edit = Edit;
        }

        #endregion

        #region Protected Overrides

        protected override bool OnDialogExecute(IFindDlg Dlg, IWin32Window Owner, FindData Data)
        {
            //Initialize the find text from the selection.
            string strOldFindText = null;
            if (m_Edit.SelectionLength > 0)
            {
                //Only use the selection if it is one line or less.
                string strSelectedText = m_Edit.SelectedText;
                if (strSelectedText.IndexOf('\n') < 0)
                {
                    strOldFindText = Data.Text;
                    Data.Text = m_Edit.SelectedText;
                }
            }

            //Call the base method to display the dialog.
            bool bResult = base.OnDialogExecute(Dlg, Owner, Data);

            //If they cancelled, then we may need to restore the old find text.
            if (!bResult && strOldFindText != null)
            {
                Data.Text = strOldFindText;
            }

            return bResult;
        }

        protected override bool OnFindNext()
        {
            string strFindText, strEditText;
            GetStrings(out strFindText, out strEditText);

            //Search from the starting position to the end.
            int iStartingPosition = m_Edit.SelectionStart + m_Edit.SelectionLength;
            int iFindIndex = strEditText.IndexOf(strFindText, iStartingPosition);

            if (iFindIndex < 0)
            {
                //If not found, then search from the beginning to the starting position.
                iFindIndex = strEditText.IndexOf(strFindText, 0, iStartingPosition);
            }

            return HandleFindIndex(iFindIndex);
        }

        protected override bool OnFindPrevious()
        {
            string strFindText, strEditText;
            GetStrings(out strFindText, out strEditText);

            //Search from the starting position to the beginning.
            int iStartingPosition = m_Edit.SelectionStart;
            int iFindIndex = strEditText.LastIndexOf(strFindText, iStartingPosition);

            if (iFindIndex < 0)
            {
                //If not found, then search from the end to the starting position.
                int iLastIndex = strEditText.Length;
                iFindIndex = strEditText.LastIndexOf(strFindText, iLastIndex, iLastIndex - iStartingPosition);
            }

            return HandleFindIndex(iFindIndex);
        }

        #endregion

        #region Private Methods

        private void GetStrings(out string strFindText, out string strEditText)
        {
            strFindText = Data.Text;
            strEditText = m_Edit.Text;
            if (!Data.MatchCase)
            {
                strFindText = strFindText.ToUpper();
                strEditText = strEditText.ToUpper();
            }
        }

        private bool HandleFindIndex(int iFindIndex)
        {
            if (iFindIndex >= 0)
            {
                m_Edit.SelectionStart = iFindIndex;
                m_Edit.SelectionLength = Data.Text.Length;
                m_Edit.ScrollToCaret();
                return true;
            }
            else
            {
                string strMsg = String.Format("'{0}' was not found.", Data.Text);
                MessageBox.Show(m_Edit, strMsg, "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        #endregion

        #region Private Data Members

        private TextBoxBase m_Edit;

        #endregion
    }
}
