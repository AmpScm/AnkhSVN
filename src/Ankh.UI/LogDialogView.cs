using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Presenters;

namespace Ankh.UI
{
	public partial class LogDialogView : UserControl, ISvnLogView
	{
		public LogDialogView()
		{
			InitializeComponent();
		}

		public void Add(SharpSvn.SvnLogEventArgs newLogEventArgs)
		{
			logRevisionControl1.Add(newLogEventArgs);
		}

		public void UpdateRowCount()
		{
			logRevisionControl1.UpdateRowCount();
		}

		public void Reset()
		{
			logRevisionControl1.logItems.Clear();
			logRevisionControl1.UpdateRowCount();
		}

		public bool IncludeMergedRevisions
		{
			get { return includeMergedButton.Checked; }
		}

		public bool StrictNodeHistory
		{
			get { return stopOnCopyButton.Checked; }
		}

		public int ScrollPosition
		{
			get { return logRevisionControl1.ScrollPosition; }
		}

		public int ItemCount
		{
			get { return logRevisionControl1.logItems.Count; }
		}

		public event EventHandler<EventArgs> IncludeMergedRevisionsChanged;

		public event EventHandler<EventArgs> StrictNodeHistoryChanged;

		public event EventHandler<ScrollEventArgs> ScrollPositionChanged;

		private void logRevisionControl1_ScrollPositionChanged(object sender, ScrollEventArgs e)
		{
			if (ScrollPositionChanged != null)
				ScrollPositionChanged(this, e);
		}

		private void includeMergedButton_CheckedChanged(object sender, EventArgs e)
		{
			if (IncludeMergedRevisionsChanged != null)
				IncludeMergedRevisionsChanged(this, EventArgs.Empty);
		}

		private void stopOnCopyButton_CheckedChanged(object sender, EventArgs e)
		{
			if (StrictNodeHistoryChanged != null)
				StrictNodeHistoryChanged(this, EventArgs.Empty);
		}

	}
}
