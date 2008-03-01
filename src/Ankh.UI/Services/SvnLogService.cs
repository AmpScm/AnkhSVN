using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using Utils.Services;
using Ankh.UI.Helpers;
using System.Threading;
using System.ComponentModel;

namespace Ankh.UI.Services
{
	public class SvnLogService : ISvnLogService
	{
		[ThreadStatic]
		static SvnClient client;
		//ISynchronizeInvoke syncContext;

		Uri remoteTarget;
		string localTarget;
		Queue<SvnLogEventArgs> logItemQueue = new Queue<SvnLogEventArgs>();
		SvnRevision start = SvnRevision.None;
		SvnRevision end = SvnRevision.None;
		bool includeMergedRevisions;
		bool strictNodeHistory;
		int requiredItemCount;
		bool cancel;

		delegate void VoidHandler();
		delegate void LogItemFetcher(SvnLogArgs args);
		EventHandler<SvnLogEventArgs> logItemReceiver;

		SynchronizationContext syncContext;
		SendOrPostCallback sopCallback;
		public SvnLogService()//ISynchronizeInvoke syncContext)
		{
			//this.syncContext = syncContext;
			syncContext = SynchronizationContext.Current;
			sopCallback = new SendOrPostCallback(SopCallback);
			logItemReceiver += new EventHandler<SvnLogEventArgs>(OnReceiveItem);
		}

		SvnClient Client
		{
			get 
			{
				if(client == null)
					client = SvnClientFactory.NewClient();
				return client;
			}
		}

		public Uri RemoteTarget
		{
			get { return remoteTarget; }
			set { remoteTarget = value; }
		}

		public string LocalTarget
		{
			get { return localTarget; }
			set { localTarget = value; }
		}

		public bool StrictNodeHistory
		{
			get { return strictNodeHistory; }
			set { strictNodeHistory = value; }
		}

		public bool IncludeMergedRevisions
		{
			get { return includeMergedRevisions; }
			set { includeMergedRevisions = value; }
		}

		public IEnumerable<SharpSvn.SvnLogEventArgs> RetrieveAndFlushLogItems()
		{
			while (logItemQueue.Count > 0)
				yield return logItemQueue.Dequeue();
		}

		public event EventHandler<EventArgs> LogItemReceived;

		public event EventHandler<EventArgs> Started;

		public event EventHandler<EventArgs> Completed;

		
		public int RequiredItemCount
		{
			get { return requiredItemCount; }
			set { requiredItemCount = value; }
		}

		
		public void Start()
		{
			cancel = false;
			LogItemFetcher fetcher = new LogItemFetcher(DoFetch);
			SvnLogArgs args = new SvnLogArgs();
			args.Start = StartRevision;
			args.End = EndRevision;
			args.Limit = RequiredItemCount;
			args.StrictNodeHistory = StrictNodeHistory;
			args.RetrieveMergedRevisions = IncludeMergedRevisions;

			fetcher.BeginInvoke(args, new AsyncCallback(FetchCompleted), null);

			if (Started != null)
				Started(this, EventArgs.Empty);
		}

		void DoFetch(SvnLogArgs args)
		{
			if (LocalTarget != null)
				Client.Log(LocalTarget, args, logItemReceiver);
			if (RemoteTarget != null)
				Client.Log(RemoteTarget, args, logItemReceiver);
		}

		void FetchCompleted(IAsyncResult rslt)
		{
			if (Completed != null)
				Completed(this, EventArgs.Empty);
		}

		void OnReceiveItem(object sender, SvnLogEventArgs e)
		{
			if (cancel)
			{
				e.Cancel = true;
				cancel = false;
			}

			lock (logItemQueue)
			{
				e.Detach();

				logItemQueue.Enqueue(e);
			}

			if(syncContext != null)
				syncContext.Send(sopCallback, null);
			else
				SopCallback(null);
		}

		void SopCallback(object state)
		{
			if (LogItemReceived != null)
				LogItemReceived(this, EventArgs.Empty);
		}

		public void Cancel()
		{
			cancel = true;
			Reset();
		}

		public SvnRevision StartRevision
		{
			get { return start; }
			set { start = value; }
		}

		public SvnRevision EndRevision
		{
			get { return end; }
			set { end = value; }
		}

		void Reset()
		{
			logItemQueue.Clear();
		}
	}
}
