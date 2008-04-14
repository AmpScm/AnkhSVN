using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.UI
{
	public interface ISvnLogService
	{
		/// <summary>
		/// The remote target
		/// </summary>
		Uri RemoteTarget { get; set; }
		
		/// <summary>
		/// The local target
		/// </summary>
		ICollection<string> LocalTargets { get; set; }

		/// <summary>
		/// Aka stop-on-copy
		/// </summary>
		bool StrictNodeHistory { get; set; }

		/// <summary>
		/// Gets or sets a value indicating wether merged revisions should be retrieved
		/// </summary>
		bool IncludeMergedRevisions { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IEnumerable<SvnLogEventArgs> RetrieveAndFlushLogItems();

		/// <summary>
		/// Fires when one or more log items were received
		/// </summary>
		event EventHandler<EventArgs> LogItemReceived;

		event EventHandler<EventArgs> Started;

		/// <summary>
		/// Fires when the log service stops fetching new logs
		/// </summary>
		event EventHandler<EventArgs> Completed;

		/// <summary>
		/// The number of log items that should eventually be retrieved
		/// </summary>
		int RequiredItemCount { get; set; }

		/// <summary>
		/// Starts fetching the <see cref="RequiredItemCount"/> of log items
		/// </summary>
		void Start();

		void Cancel();

		SvnRevision StartRevision { get; set; }
		SvnRevision EndRevision { get; set; }

	}
}
