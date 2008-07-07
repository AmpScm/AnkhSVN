#region Copyright And Revision History

/*---------------------------------------------------------------------------

	WaitCursor.cs
	Copyright © 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	11.18.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

#region Using Statements

using System;
using System.Windows.Forms;

#endregion

namespace Ankh.Diff
{
	public sealed class WaitCursor : MarshalByRefObject, IDisposable
	{
		#region Constructors

		public WaitCursor() : this(null)
		{
		}

		public WaitCursor(Control Ctrl)
		{
			m_Control = Ctrl;

			//Try to find the highest-level control (i.e., Form) that we can.
			if (m_Control != null)
			{
				Form frm = m_Control.FindForm();
				if (frm == null)
				{
					frm = Form.ActiveForm;
				}

				if (frm != null)
				{
					m_Control = frm;
				}
			}
			else
			{
				m_Control = Form.ActiveForm;
			}

			if (m_Control == null)
				m_Previous = Cursor.Current;
			else
				m_Previous = m_Control.Cursor;
			Refresh();
		}

		#endregion

		#region Public Methods

		public void Close()
		{
			bool bUseWaitCursor = m_Previous == Cursors.WaitCursor;
			if (m_Control != null)
			{
				m_Control.Cursor = m_Previous;
				m_Control.UseWaitCursor = bUseWaitCursor;
			}
			Cursor.Current = m_Previous;
			Application.UseWaitCursor = bUseWaitCursor;
		}

		public void Refresh()
		{
			if (m_Control != null)
			{
				m_Control.Cursor = Cursors.WaitCursor;
				m_Control.UseWaitCursor = true;
			}

			Cursor.Current = Cursors.WaitCursor;
			Application.UseWaitCursor = true;
		}

		#endregion

		#region IDisposable Implementation

		public void Dispose()
		{
			Close();
		}

		#endregion

		#region Private Data Members

		private Cursor m_Previous;
		private Control m_Control;

		#endregion
	}
}
