using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using Ankh.Scc;
using System.Threading;
using System.ComponentModel;

namespace Ankh.UI.Services
{
	public class SvnLogService : ISvnLogService
	{
        readonly IAnkhServiceProvider _context;
		//ISynchronizeInvoke syncContext;

		Uri remoteTarget;
		ICollection<string> localTargets;
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
		public SvnLogService(IAnkhServiceProvider context)//ISynchronizeInvoke syncContext)
		{
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
			//this.syncContext = syncContext;
			syncContext = SynchronizationContext.Current;
			sopCallback = new SendOrPostCallback(SopCallback);
			logItemReceiver += new EventHandler<SvnLogEventArgs>(OnReceiveItem);
		}

		public Uri RemoteTarget
		{
			get { return remoteTarget; }
			set { remoteTarget = value; }
		}

		public ICollection<string> LocalTargets
		{
			get { return localTargets; }
			set { localTargets = value; }
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
            using (SvnClient client = _context.GetService<ISvnClientPool>().GetClient())
            {
                if (LocalTargets != null)
                    client.Log(LocalTargets, args, logItemReceiver);
                else if (RemoteTarget != null)
                    client.Log(RemoteTarget, args, logItemReceiver);
            }
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
