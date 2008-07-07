#region Copyright And Revision History

/*---------------------------------------------------------------------------

	ListViewEx.cs
	Copyright © 2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	4.23.2003	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Permissions;
using System.ComponentModel;

namespace Ankh.Diff
{
	public enum ColumnHeaderAutoSize { UseData = -1, UseHeader = -2 };

	[ToolboxBitmap(typeof(ListView))]
	public sealed class ListViewEx : ListView
	{
		#region Public Enums and Constants

		public const int AUTOSIZE_USEDATA = (int)ColumnHeaderAutoSize.UseData;
		public const int AUTOSIZE_USEHEADER = (int)ColumnHeaderAutoSize.UseHeader;

		#endregion

		#region Public Properties

		[Browsable(false)]
		public bool InDoubleClick
		{
			get
			{
				return m_bInDoubleClick;
			}
		}

		[DefaultValue(true)]
		public bool AllowItemCheck
		{
			get
			{
				return m_bAllowItemCheck;
			}
			set
			{
				m_bAllowItemCheck = value;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// This returns the column width even inside of a BeginUpdate/EndUpdate pair.
		/// </summary>
		public int GetActualColumnWidth(ColumnHeader Header)
		{
			return Windows.SendMessage(this, Windows.LVM_GETCOLUMNWIDTH, Header.Index, 0);
		}

		public void AutoSizeColumn(ColumnHeader Column)
		{
			AutoSizeColumn(Column, true);
		}

		public void AutoSizeColumn(ColumnHeader Column, bool bAllowShrinking)
		{
			BeginUpdate();
			try
			{
				int iOriginalWidth = Column.Width;

				Column.Width = AUTOSIZE_USEHEADER;
				int iHeaderWidth = GetActualColumnWidth(Column);

				Column.Width = AUTOSIZE_USEDATA;
				int iContentWidth = GetActualColumnWidth(Column);

				int iNewWidth = Math.Max(iHeaderWidth, iContentWidth);

				if (!bAllowShrinking && iNewWidth < iOriginalWidth)
				{
					//Set the column width back to what it was.
					Column.Width = iOriginalWidth;
				}
				else if (iNewWidth != Column.Width)
				{
					Column.Width = iNewWidth;
				}
			}
			finally
			{
				EndUpdate();
			}
		}

		public void AutoSizeColumns()
		{
			BeginUpdate();
			try
			{
				foreach(ColumnHeader Column in Columns)
				{
					AutoSizeColumn(Column);
				}
			}
			finally
			{
				EndUpdate();
			}
		}

		#endregion

		#region Protected Methods

		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
		protected override void WndProc(ref Message M)
		{
			//We need to change three things about the default ListView's double-click behavior:
			//1.  We don't want double-clicks to cause items to be checked/unchecked.
			//2.  We want the event to fire even if they click in the whitespace (i.e. not on an item).
			//3.  We don't want the event to fire again after a double-click launches a modal, that
			//    gets closed, and then someone tries to check or uncheck the item.  Somehow in
			//    .NET 1.0 that reflects a WM_LBUTTONUP message and fires the OnDoubleClick override!
			if (M.Msg == Windows.WM_LBUTTONDBLCLK)
			{
				m_bDoubleClickEventFired = false;
				m_bInDoubleClick = true;
				try
				{
					base.WndProc(ref M);
				}
				finally
				{
					m_bInDoubleClick = false;
				}

				if (!m_bDoubleClickEventFired)
				{
					base.OnDoubleClick(EventArgs.Empty);
				}

				m_bDoubleClickEventFired = false;
			}
			else
			{
				base.WndProc(ref M);
			}
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			if (m_bInDoubleClick)
			{
				m_bDoubleClickEventFired = true;
				base.OnDoubleClick(e);
			}
		}

		protected override void OnItemCheck(ItemCheckEventArgs e)
		{
			//Don't let double click change the check state.
			//Also, don't let mouse multi-selection change the
			//check states.
			if (!m_bAllowItemCheck || m_bInDoubleClick)
			{
				e.NewValue = e.CurrentValue;
				return;
			}
			else if (m_bMouseDown && SelectedIndices.Count > 1)
			{
				//Only allow a mouse click to change multiple checks
				//if they clicked on one item.  If they changed items,
				//this works around a ListView bug.
				ListViewItem MouseDownItem = GetItemAt(m_ptMouseDown.X, m_ptMouseDown.Y);
				Point ptCurrent = PointToClient(Control.MousePosition);
				ListViewItem CurrentMouseItem = GetItemAt(ptCurrent.X, ptCurrent.Y);

				//The list view also has a bug where it will do item checks
				//during multi-selection with Ctrl+Click.
				if (MouseDownItem != CurrentMouseItem || !IsPointInCheck(ptCurrent, e.Index))
				{
					e.NewValue = e.CurrentValue;
					return;
				}
			}

			base.OnItemCheck(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			Capture = true;
			m_bMouseDown = true;
			m_ptMouseDown = new Point(e.X, e.Y);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			Capture = false;
			m_bMouseDown = false;
		}

		#endregion

		#region Private Methods

		private bool IsPointInCheck(Point pt, int iIndex)
		{
			if (Columns.Count > 0)
			{
				ListViewItem Item = Items[iIndex];
				int iScrollPos = Windows.GetScrollPos(this, true);
				int iPX = pt.X + iScrollPos;

				int iImageWidth = StateImageList != null ? StateImageList.ImageSize.Width : 16;

				int iCheckStart = iImageWidth * Item.IndentCount;
				int iCheckStop = iCheckStart + iImageWidth;

				return iPX >= iCheckStart && iPX <= iCheckStop;
			}

			return false;
		}

		#endregion

		#region Private Data Members

		private bool m_bInDoubleClick;
		private bool m_bDoubleClickEventFired;
		private bool m_bMouseDown;
		private bool m_bAllowItemCheck = true;
		private Point m_ptMouseDown;

		#endregion
	}
}
