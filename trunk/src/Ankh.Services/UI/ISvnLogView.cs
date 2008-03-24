using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Ankh.UI.Presenters
{
	public interface ISvnLogView
	{
		void Add(SvnLogEventArgs newLogEventArgs);
		void UpdateRowCount();
		void Reset();

		bool IncludeMergedRevisions { get; }
		bool StrictNodeHistory { get; }
		int ItemCount { get; }

		event EventHandler<EventArgs> IncludeMergedRevisionsChanged;
		event EventHandler<EventArgs> StrictNodeHistoryChanged;
		event EventHandler<ScrollEventArgs> ScrollPositionChanged;
	}
}
