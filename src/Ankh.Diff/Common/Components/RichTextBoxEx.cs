#region Copyright And Revision History

/*---------------------------------------------------------------------------

	RichTextBoxEx.cs
	Copyright © 2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	5.21.2003	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;

namespace Ankh.Diff
{
	public sealed class RichTextBoxEx : RichTextBox
	{
        #region Constructors

		public RichTextBoxEx()
		{
			SetStyle(ControlStyles.StandardClick, true);
			SetStyle(ControlStyles.StandardDoubleClick, true);

			m_dPixelsPerDLU = TextEditor.CalcPixelsPerDLU(this);
		}

		#endregion

        #region Public Properties

		public override string Text
		{
			get
			{
				//Pulling the standard Text property causes the Undo buffer to be cleared.  :-(
				if (IsHandleCreated)
				{
					string strText = Windows.GetText(this);
					return strText;
				}
				else
				{
					return base.Text;
				}
			}
			set
			{
				base.Text = value;
			}
		}

		[Browsable(false)]
		public override int TextLength
		{
			get
			{
				//Pulling the standard TextLength property causes the Undo buffer to be cleared,
				//and it always returns Text.Length.
				if (IsHandleCreated)
				{
					int iLength = Windows.GetTextLength(this);
					return iLength;
				}
				else
				{
					return base.TextLength;
				}
			}
		}

		[Browsable(true), DefaultValue(0), Category("Behavior"),
		Description("The number of spaces a TAB character should be as wide as.  This only works for monospaced fonts (e.g. Courier New).  Set to 0 to use the edit control's default tab stops.")]
		public int TabSpaces
		{
			get
			{
				return m_iTabSpaces;
			}
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("TabSpaces", value, "TabSpaces must be non-negative.");

				if (value != m_iTabSpaces)
				{
					m_iTabSpaces = value;
					TextEditor.SetTabWidth(this, m_iTabSpaces, m_dPixelsPerDLU);
				}
			}
		}

		[Browsable(false), Description("Whether text can currently be cut.")]
		public bool CanCut
		{
			get
			{
				return SelectionLength > 0 && !ReadOnly;
			}
		}

		[Browsable(false), Description("Whether text can currently be copied.")]
		public bool CanCopy
		{
			get
			{
				return SelectionLength > 0;
			}
		}

		[Browsable(false), Description("Whether text can currently be pasted.")]
		public bool CanPasteText
		{
			get
			{
				return TextEditor.CanPasteText(this);
			}
		}

		[Browsable(false), Description("The 0-based line and column for the current caret position."),
		ReadOnly(true)]
		public Point CaretPoint
		{
			get
			{
				return TextEditor.GetCaretPoint(this);
			}
			set
			{
				TextEditor.SetCaretPoint(this, value);
			}
		}

		[Browsable(false), Description("Gets the 0-based index of the uppermost visible line.")]
		public int FirstVisibleLine
		{
			get
			{
				return TextEditor.GetFirstVisibleLine(this);
			}
		}

		[Browsable(false), Description("Gets the number of lines.  Always >= 1.")]
		public int LineCount
		{
			get
			{
				return TextEditor.GetLineCount(this);
			}
		}

		#endregion

        #region Public Methods

		public string GetCurrentLineText(bool bSelectLine)
		{
			int iLineIndex = LineIndex(-1);
			int iLineLength = LineLength(iLineIndex);
			
			if (iLineIndex >= 0 && iLineLength > 0)
			{
				int iSelStart = SelectionStart;
				int iSelLength = SelectionLength;

				SelectionStart = iLineIndex;
				SelectionLength = iLineLength;

				string strText = SelectedText;

				//Restore the previous selection if they don't want
				//the whole line selected.
				if (!bSelectLine)
				{
					SelectionStart = iSelStart;
					SelectionLength = iLineLength;
				}

				return strText;
			}
			else
			{
				return "";
			}
		}

		[Description("Gets the index of the first character of the specified line.")]
		public int LineIndex(int iLineNo)
		{
			return TextEditor.LineIndex(this, iLineNo);
		}

		[Description("Gets the length, in characters, of the specified line.")]
		public int LineLength(int iLineNo)
		{
			return TextEditor.LineLength(this, iLineNo);
		}

		[Description("Gets the index of the line that contains the specified character index.")]
		public int LineFromChar(int iCharIndex)
		{
			return TextEditor.LineFromChar(this, iCharIndex);
		}

		[Description("Scrolls the text the specified number of characters horizontally and/or lines vertically.")]
		public bool Scroll(int iHorzChars, int iVertLines)
		{
			return TextEditor.Scroll(this, iHorzChars, iVertLines);
		}

		[Description("Moves the caret to the 0-based line number.")]
		public bool GotoLine(int iLineNo, bool bSelectLine)
		{
			return TextEditor.GotoLine(this, iLineNo, bSelectLine);
		}

		#endregion

        #region Protected Methods

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			m_dPixelsPerDLU = TextEditor.HandleFontChanged(this, m_iTabSpaces);
		}

		#endregion

		#region Private Data Members

		private int m_iTabSpaces = 0;
		private double m_dPixelsPerDLU = 2;

		#endregion
	}
}
