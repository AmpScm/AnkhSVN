#region Copyright And Revision History

/*---------------------------------------------------------------------------

	TextEditor.cs
	Copyright © 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	5.27.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

#region Using Statements

using System;
using System.Windows.Forms;
using System.ComponentModel; //For Browsable
using System.Drawing; //For Point
using System.IO;
using System.Runtime.InteropServices;

#endregion

namespace Ankh.Diff
{
	#region Helper Types

	//We have to derive this class from EventArgs for the event to show up in the Event browser.
	[Description("The event arguments for the CaretMoved event.")]
	public sealed class CaretMovedEventArgs : EventArgs
	{
		public Point CaretPoint
		{
			get { return m_CaretPoint; }
		}

		internal void SetCaretPoint(Point Pt)
		{
			m_CaretPoint = Pt;
		}

		private Point m_CaretPoint = new Point();
	}

	[Description("A delegate type for hooking up CaretMoved notifications.")]
	public delegate void CaretMovedEventHandler(object sender, CaretMovedEventArgs e);

	#endregion

	/// <summary>
	/// Adds common editor features to the TextBox class such as the
	/// ability to open and save files, management of recent files,
	/// and some useful properties and events (e.g. CaretPoint and
	/// CaretMoved).
	/// </summary>
	[ToolboxBitmap(typeof(TextEditor), "Images.TextEditor.bmp")]
	public sealed class TextEditor : TextBox
	{
		#region Constructors

		public TextEditor()
		{
			//Change defaults for inherited properties
			Multiline = true;
			ScrollBars = ScrollBars.Both;
			AcceptsReturn = true;
			AcceptsTab = true;
			WordWrap = false;
			Text = "";
			MaxLength = 0;
			HideSelection = false;

			m_dPixelsPerDLU = CalcPixelsPerDLU(this);
		}

		#endregion

		#region Changed Inherited Properties

		[DefaultValue("")] public override string Text { get { return base.Text; } set { base.Text = value; } }
		[DefaultValue(true)] public override bool Multiline { get { return base.Multiline; } set { base.Multiline = value; } }
		[DefaultValue(0)] public override int MaxLength { get { return base.MaxLength; } set { base.MaxLength = value; } }

		[DefaultValue(ScrollBars.Both)] public new ScrollBars ScrollBars { get { return base.ScrollBars; } set { base.ScrollBars = value; } }
		[DefaultValue(true)] public new bool AcceptsReturn { get { return base.AcceptsReturn; } set { base.AcceptsReturn = value; } }
		[DefaultValue(true)] public new bool AcceptsTab { get { return base.AcceptsTab; } set { base.AcceptsTab = value; } }
		[DefaultValue(false)] public new bool WordWrap { get { return base.WordWrap; } set { base.WordWrap = value; } }
		[DefaultValue(false)] public new bool HideSelection { get { return base.HideSelection; } set { base.HideSelection = value; } }

		#endregion

		#region Design-time properties

		[Browsable(true), DefaultValue(null), Category("Helper Objects"),
		Description("The dialog to use when opening a file.")]
		public OpenFileDialog OpenFileDialog
		{
			get
			{
				return m_OpenDlg;
			}
			set
			{
				m_OpenDlg = value;
			}
		}

		[Browsable(true), DefaultValue(null), Category("Helper Objects"),
		Description("The dialog to use when saving a file.")]
		public SaveFileDialog SaveFileDialog
		{
			get
			{
				return m_SaveDlg;
			}
			set
			{
				m_SaveDlg = value;
			}
		}

		[Browsable(true), DefaultValue(null), Category("Helper Objects"),
		Description("The recent item list manager.")]
		public RecentItemList RecentFiles
		{
			get
			{
				return m_RecentFiles;
			}
			set
			{
				m_RecentFiles = value;
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
					SetTabWidth(this, m_iTabSpaces, m_dPixelsPerDLU);
				}
			}
		}

		#endregion

		#region Run-time only properties

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
		public bool CanPaste
		{
			get
			{
				return CanPasteText(this);
			}
		}

		[Browsable(false), Description("The current file title (e.g. name with no path).")]
		public string Title
		{
			get
			{
				if (m_strFileName.Length == 0)
					return "<Untitled>";
				else
				{
					FileName FN = new FileName(m_strFileName);
					return FN.Name;
				}
			}
		}

		[Browsable(false), Description("The current file name.")]
		public string FileName
		{
			get
			{
				return m_strFileName;
			}
			set
			{
				m_strFileName = value;
			}
		}

		[Browsable(false), Description("The 0-based line and column for the current caret position."),
		ReadOnly(true)]
		public Point CaretPoint
		{
			get
			{
				return GetCaretPoint(this);
			}
			set
			{
				SetCaretPoint(this, value);
			}
		}

		[Browsable(false), Description("Gets the 0-based index of the uppermost visible line.")]
		public int FirstVisibleLine
		{
			get
			{
				return GetFirstVisibleLine(this);
			}
		}

		[Browsable(false), Description("Gets the number of lines.  Always >= 1.")]
		public int LineCount
		{
			get
			{
				return GetLineCount(this);
			}
		}

		#endregion

		#region Custom Events
	
		[Browsable(true), Category("Property Changed"),
		Description("Called during idle times when the caret has moved.")]
		public event CaretMovedEventHandler CaretMoved
		{
			add
			{
				//If this is the first add, we need to hook the Idle event.
				if (m_CaretMovedEvent == null)
				{
					if (m_IdleEvent == null)
					{
						m_IdleEvent = new EventHandler(OnIdle);
						m_CaretMovedEventArgs = new CaretMovedEventArgs();
					}

					Application.Idle += m_IdleEvent;
				}

				m_CaretMovedEvent += value;
			}
			remove
			{
				m_CaretMovedEvent -= value;

				if (m_CaretMovedEvent == null)
				{
					Application.Idle -= m_IdleEvent;
				}
			}
		}

		private void OnCaretMoved(Point CaretPoint)
		{
			if (m_CaretMovedEvent != null)
			{
				m_CaretMovedEventArgs.SetCaretPoint(CaretPoint);
				m_CaretMovedEvent(this, m_CaretMovedEventArgs);
			}
		}

		private void OnIdle(object Sender, EventArgs args)
		{
			try
			{
				Point Pt = CaretPoint;
				if (Pt != m_LastCaretPoint)
				{
					m_LastCaretPoint = Pt;
					OnCaretMoved(Pt);
				}
			}
			catch(Exception e)
			{
				//We must explicitly call this because Application.Idle
				//doesn't run inside the normal ThreadException protection
				//that the Application provides for the main message pump.
				Application.OnThreadException(e);
			}
		}

		#endregion

		#region Internal Methods

		internal static bool CanPasteText(TextBoxBase TB)
		{
			bool bResult = !TB.ReadOnly;

			if (bResult)
			{
				try
				{
					IDataObject iData = Clipboard.GetDataObject();
					if (iData != null)
					{
						bResult = iData.GetDataPresent(DataFormats.Text);
					}
				}
				catch(ExternalException)
				{
					bResult = false;
				}
			}

			return bResult;
		}

		internal static Point GetCaretPoint(TextBoxBase TB)
		{
			//Get offset of current line.
			int iLineIndex = LineIndex(TB, -1);

			//Get Caret offset from first char
			int cpMin = 0, cpMax = 0;
			Windows.SendMessage(TB, Windows.EM_GETSEL, ref cpMin, ref cpMax);

			//Check for multiline selection with the caret at the end.
			int iCaretIndex = 0;
			if (cpMin >= iLineIndex)
				iCaretIndex = cpMin;
			else
				iCaretIndex = cpMax;
			//Note: For a single line selection, there is no
			//way to tell if the caret is at the start or end
			//of the selected text.  I always return the start
			//because several routines use Pt.x to get the
			//selection start relative to the current line.

			Point Result = new Point();

			//Get Caret Column
			Result.X = iCaretIndex - iLineIndex;
			//Get Caret Row
			Result.Y = LineFromChar(TB, iCaretIndex);

			//Validity check
			int iNextLineIndex = LineIndex(TB, Result.Y);
			if (iLineIndex != iCaretIndex && iCaretIndex == iNextLineIndex)
			{
				Result.Y--;
			}

			//Note: This validity check is necessary because of a funny
			//situation where the cursor is on a word wrapped line and
			//you press VK_END.  The cursor moves to the position PAST
			//the wrapping space.  On the first line, we'd have something
			//like: LineIndex = 0, CaretIndex = 66, NextLineIndex = 66.
			//In other words, Windows.EM_LINEINDEX thinks we're on line 1, but
			//Windows.EM_LINEFROMCHAR will say we're on line 2!!!
			//However, if you put the cursor at the beginning of the next
			//line, we'll have something like: LineIndex = 66,
			//CaretIndex = 66, NextLineIndex = 66.  That's why all three
			//indexes have to be in the validity check.

			return Result;
		}

		internal static void SetCaretPoint(TextBoxBase TB, Point pt)
		{
			int iIndex = LineIndex(TB, pt.Y);
			if (iIndex != -1)
			{
				TB.SelectionStart = iIndex + pt.X;
				TB.SelectionLength = 0;
			}
		}

		internal static int GetFirstVisibleLine(TextBoxBase TB)
		{
			return Windows.SendMessage(TB, Windows.EM_GETFIRSTVISIBLELINE, 0);
		}

		internal static int GetLineCount(TextBoxBase TB)
		{
			return Windows.SendMessage(TB, Windows.EM_GETLINECOUNT, 0);
		}

		internal static void SetTabWidth(TextBoxBase TB, int iTabSpaces, double dPixelsPerDLU)
		{
			int iNumTabs = 0;
			int iTabDLUs = 0;

			//Note: This only works well for fixed width fonts.  It works 
			//ok for some non-fixed fonts like "Comic Sans MS", but it 
			//doesn't work for "less monospaced" fonts like "Garamond" or 
			//"Georgia".
			//
			//As far as I can tell, this is because I have to return an
			//integral number of dialog units.  I can calculate the correct
			//number of tab pixels for any font, but when I return the 
			//integral DLUs for the tab size, it doesn't multiply back to 
			//the correct number of pixels (except on monospaced fonts).  
			//If only the stupid edit control would let me pass in pixels
			//instead of bleepin' dialog units!!!

			//If iTabSpaces is 0 we'll just set the tab stops to the 
			//edit control default of 32 DLUs.
			if (iTabSpaces > 0)
			{
				//Get the pixel width that a space should be.
				double dSpacePixels;
				using (Graphics G = Graphics.FromHwnd(TB.Handle))
				{
					TextFormatFlags eFlags = TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding;
					dSpacePixels = TextRenderer.MeasureText(G, " ", TB.Font, new Size(int.MaxValue, int.MaxValue), eFlags).Width;
				}

				//Convert the pixels to "dialog units"
				int iSpaceDLUs = (int)Math.Ceiling(dSpacePixels / dPixelsPerDLU);
				iTabDLUs = (iTabSpaces * iSpaceDLUs);
				iNumTabs = 1;
			}

			//LPARAM has to contain a pointer to the new width,
			//so we have to pass that parameter by reference.
			Windows.SendMessage(TB, Windows.EM_SETTABSTOPS, iNumTabs, ref iTabDLUs);
			TB.Invalidate();
		}

		internal static double CalcPixelsPerDLU(TextBoxBase TB)
		{
			double dAvgCurrentFontWidthPixels;
			//Get the pixel width that a space should be.
			using (Graphics G = Graphics.FromHwnd(TB.Handle))
			{
				//See KBase article Q125681 for what I'm doing here to get the average character width.
				TextFormatFlags eFlags = TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding;
				dAvgCurrentFontWidthPixels = TextRenderer.MeasureText(G, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", TB.Font, new Size(int.MaxValue, int.MaxValue), eFlags).Width / 52.0;
			}

			//Convert the pixels to "dialog units"
			int iSysBaseUnits = Windows.GetDialogBaseUnits();
			int iAvgSystemFontWidthPixels = iSysBaseUnits & 0xFFFF; //Get the low-order word

			double dPixelsPerDLU = (2 * dAvgCurrentFontWidthPixels / iAvgSystemFontWidthPixels);
			return dPixelsPerDLU;
		}

		internal static double HandleFontChanged(TextBoxBase TB, int iTabSpaces)
		{
			//Update the PixelsPerDLU setting
			double dPixelsPerDLU = CalcPixelsPerDLU(TB);

			//Since the tab width is based on the current font,
			//we have to update it when the font changes.
			SetTabWidth(TB, iTabSpaces, dPixelsPerDLU);

			return dPixelsPerDLU;
		}

		internal static bool GotoLine(TextBoxBase TB, int iLineNo, bool bSelectLine)
		{
			bool bResult = false;

			int iIndex = LineIndex(TB, iLineNo);
			if (iIndex != -1)
			{
				TB.SelectionStart = iIndex;
				if (bSelectLine)
					TB.SelectionLength = LineLength(TB, iIndex);
				else
					TB.SelectionLength = 0;

				bResult = true;
			}

			return bResult;
		}

		internal static int LineIndex(TextBoxBase TB, int iLineNo)
		{
			return Windows.SendMessage(TB, Windows.EM_LINEINDEX, iLineNo);
		}

		internal static int LineLength(TextBoxBase TB, int iLineNo)
		{
			return Windows.SendMessage(TB, Windows.EM_LINELENGTH, iLineNo);
		}

		internal static int LineFromChar(TextBoxBase TB, int iCharIndex)
		{
			return Windows.SendMessage(TB, Windows.EM_LINEFROMCHAR, iCharIndex);
		}

		internal static bool Scroll(TextBoxBase TB, int iHorzChars, int iVertLines)
		{
			return Windows.SendMessage(TB, Windows.EM_LINESCROLL, iHorzChars, iVertLines) != 0;
		}

		#endregion

		#region Helper Methods

		//File Methods

		public bool New()
		{
			bool bResult = false;

			if (CanClose(false) == DialogResult.Yes)
			{
				Close();
				bResult = true;
			}

			return bResult;
		}

		public bool Open()
		{
			bool bResult = false;

			if (m_OpenDlg != null && m_OpenDlg.ShowDialog() == DialogResult.OK)
			{
				bResult = OpenFile(m_OpenDlg.FileName);
			}

			return bResult;
		}

		public bool OpenFile(string strFileName)
		{
			bool bResult = false;

			if (New() && File.Exists(strFileName))
			{
				//Do this first so "Title" will return the correct name
				//in the catch block.
				m_strFileName = strFileName;

				try
				{
					//Get the expanded name to store in the recent files list.
					FileName FN = new FileName(strFileName);
					m_strFileName = FN.ExpandedName;

					//Load from file
					Text = FN.LoadFromFile();

					//Add to the recent files list
					if (m_RecentFiles != null)
					{
						m_RecentFiles.Add(m_strFileName);
					}

					Modified = false;
					bResult = true;
				}
				catch(Exception e)
				{
					Utilities.ShowError(String.Format("Unable to open \"{0}\":\r\n\r\n{1}", Title, e.ToString()));
				}
			}

			return bResult;
		}

		public void Close()
		{
			Text = "";
			Modified = false;
			FileName = "";
		}

		public DialogResult CanClose(bool bShowCancel)
		{
			if (Modified)
			{
				MessageBoxButtons Btns = MessageBoxButtons.YesNoCancel;
				MessageBoxDefaultButton DefBtn = MessageBoxDefaultButton.Button1;

				if (!bShowCancel)
				{
					Btns = MessageBoxButtons.YesNo;
					DefBtn = MessageBoxDefaultButton.Button2;
				}

				return MessageBox.Show("Do you want to close \"" + Title + "\" without saving changes?", "Confirmation", Btns, MessageBoxIcon.Question, DefBtn);
			}
			else
			{
				return DialogResult.Yes;
			}
		}

		public DialogResult Save(bool bSaveAs)
		{
			DialogResult Result = DialogResult.No;
			bool bUpdateRecentFiles = false;

			if (m_SaveDlg != null && (bSaveAs || m_strFileName.Length == 0))
			{
				m_SaveDlg.FileName = m_strFileName;
				if (m_SaveDlg.ShowDialog() == DialogResult.OK)
				{
					m_strFileName = m_SaveDlg.FileName;
					FileName FN = new FileName(m_strFileName);

					if (FN.Extension.Length == 0 && m_SaveDlg.DefaultExt.Length > 0)
					{
						m_strFileName = m_strFileName + "." + m_SaveDlg.DefaultExt;
					}

					bUpdateRecentFiles = true;
				}
				else
					Result = DialogResult.Cancel;
			}

			if (m_strFileName.Length != 0 && Result != DialogResult.Cancel)
			{
				try
				{
					FileName FN = new FileName(m_strFileName);
					FN.SaveToFile(Text);

					if (bUpdateRecentFiles && m_RecentFiles != null)
					{
						m_RecentFiles.Add(m_strFileName);
					}

					Modified = false;
					Result = DialogResult.Yes;
				}
				catch(Exception e)
				{
					Utilities.ShowError(String.Format("Unable to save \"{0}\":\r\n\r\n{1}", Title, e.ToString()));
				}
			}

			return Result;
		}

		//Edit Methods

		[Description("Gets the index of the first character of the specified line.")]
		public int LineIndex(int iLineNo)
		{
			return LineIndex(this, iLineNo);
		}

		[Description("Gets the length, in characters, of the specified line.")]
		public int LineLength(int iLineNo)
		{
			return LineLength(this, iLineNo);
		}

		[Description("Gets the index of the line that contains the specified character index.")]
		public int LineFromChar(int iCharIndex)
		{
			return LineFromChar(this, iCharIndex);
		}

		[Description("Scrolls the text the specified number of characters horizontally and/or lines vertically.")]
		public bool Scroll(int iHorzChars, int iVertLines)
		{
			return Scroll(this, iHorzChars, iVertLines);
		}

		[Description("Moves the caret to the 0-based line number.")]
		public bool GotoLine(int iLineNo, bool bSelectLine)
		{
			return GotoLine(this, iLineNo, bSelectLine);
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			m_dPixelsPerDLU = HandleFontChanged(this, m_iTabSpaces);
		}

		#endregion

		#region Private Data Members

		private OpenFileDialog m_OpenDlg = null;
		private SaveFileDialog m_SaveDlg = null;
		private string m_strFileName = "";
		private RecentItemList m_RecentFiles = null;
		private int m_iTabSpaces = 0;
		private double m_dPixelsPerDLU = 2;

		private Point m_LastCaretPoint = new Point(-1, -1);
		private CaretMovedEventHandler m_CaretMovedEvent;
		private EventHandler m_IdleEvent;
		private CaretMovedEventArgs m_CaretMovedEventArgs;

		#endregion
	}
}
