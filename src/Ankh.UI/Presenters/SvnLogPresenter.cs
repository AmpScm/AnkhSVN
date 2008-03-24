using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using SharpSvn;
using System.Threading;

namespace Ankh.UI.Presenters
{
	public class SvnLogPresenter
	{
		readonly ISvnLogView logView;
		readonly ISvnLogService logService;
		bool operationRunning;

		const int initialBatch = 30;
		const int extraItems = 50;
		const int hysteresis = 20;
		long lastRevision;
		SendOrPostCallback invokeUpdateRowCount;
		
		SynchronizationContext sync;

		public SvnLogPresenter(ISvnLogView logView, ISvnLogService logService)
		{
			if (logView == null)
				throw new ArgumentNullException("logView");
			if (logService == null)
				throw new ArgumentNullException("logService");

			this.logView = logView;
			this.logService = logService;

			this.logView.IncludeMergedRevisionsChanged += logView_IncludeMergedRevisionsChanged;
			this.logView.StrictNodeHistoryChanged += logView_StrictNodeHistoryChanged;
			this.logView.ScrollPositionChanged += logView_ItemsRequestedChanged;

			this.logService.Started += logService_Started;
			this.logService.Completed += logService_Completed;
			this.logService.LogItemReceived += logService_LogItemReceived;

			sync = SynchronizationContext.Current;
			invokeUpdateRowCount = new SendOrPostCallback(InvokeUpdateRowCount);
		}

		public void Start()
		{
			this.logService.RequiredItemCount = initialBatch;
			logService.Start();
		}

		void logService_Started(object sender, EventArgs e)
		{
			operationRunning = true;
			Trace.WriteLine("Logservice_Started");
		}

		void logView_ItemsRequestedChanged(object sender, ScrollEventArgs e)
		{
			if (operationRunning)
				return;

			if (e.ScrollOrientation != ScrollOrientation.VerticalScroll)
				return;
			if (e.NewValue < e.OldValue)
				return;
			
			int itemsToFetch = extraItems - (logView.ItemCount - e.NewValue);
			if (itemsToFetch < hysteresis)
				return;

			this.logService.RequiredItemCount = itemsToFetch;

			this.logService.StartRevision = lastRevision > 1 ? lastRevision - 1 : SvnRevision.None;
			Trace.WriteLine("Starting log");
			this.logService.Start();
		}

		void logView_StrictNodeHistoryChanged(object sender, EventArgs e)
		{
			this.logService.Cancel();
			this.logService.RequiredItemCount = initialBatch;
			this.logService.StartRevision = SvnRevision.None;
			this.logView.Reset();

			this.logService.StrictNodeHistory = this.logView.StrictNodeHistory;
			
			this.logService.Start();
		}

		void logView_IncludeMergedRevisionsChanged(object sender, EventArgs e)
		{
			this.logService.Cancel();
			this.logService.RequiredItemCount = initialBatch;
			this.logService.StartRevision = SvnRevision.None;
			this.logView.Reset();
			
			this.logService.IncludeMergedRevisions = this.logView.IncludeMergedRevisions;

			this.logService.Start();
		}

		void logService_LogItemReceived(object sender, EventArgs e)
		{
			foreach (SvnLogEventArgs arg in this.logService.RetrieveAndFlushLogItems())
			{
				this.logView.Add(arg);
				lastRevision = arg.Revision;
			}
			if (sync != null)
				sync.Post(invokeUpdateRowCount, null);
			else
				InvokeUpdateRowCount(null);
		}

		void logService_Completed(object sender, EventArgs e)
		{
			
			
			operationRunning = false;
		}

		void InvokeUpdateRowCount(object state)
		{
			logView.UpdateRowCount();
		}
	}
}
