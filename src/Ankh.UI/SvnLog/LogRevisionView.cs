using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using SharpSvn;
using SharpSvn.Implementation;


using Ankh.Scc;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;

namespace Ankh.UI.SvnLog
{
    public class LogRevisionView : ListViewWithSelection<LogListViewItem>
    {
        public LogRevisionView()
        {
        }
        public LogRevisionView(IContainer container)
        {
            container.Add(this);
        }

		protected override void OnRetrieveSelection(ListViewWithSelection<LogListViewItem>.RetrieveSelectionEventArgs e)
		{
			e.SelectionItem = new LogItem((LogListViewItem)e.Item);
			base.OnRetrieveSelection(e);
		}

		protected override void OnResolveItem(ListViewWithSelection<LogListViewItem>.ResolveItemEventArgs e)
		{
			e.Item = ((LogItem)e.SelectionItem).ListViewItem;
			base.OnResolveItem(e);
		}

    }

    public class LogListViewItem : ListViewItem
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnLoggingEventArgs _args;
        public LogListViewItem(IAnkhServiceProvider context, SvnLoggingEventArgs e)
        {
            _args = e;
            _context = context;
            RefreshText();
        }

        void RefreshText()
        {
            Text = _args.Revision.ToString(CultureInfo.CurrentCulture);
            SubItems.Add(new ListViewSubItem(this, _args.Author));
            SubItems.Add(new ListViewSubItem(this, _args.Time.ToString(CultureInfo.CurrentCulture)));
            SubItems.Add(new ListViewSubItem(this, _args.LogMessage));
        }

		internal DateTime Date
		{
			get { return _args.Time.ToLocalTime(); }
		}

		internal string Author
		{
			get { return _args.Author; }
		}

		internal string LogMessage
		{
			get { return _args.LogMessage; }
		}

		internal long Revision
		{
			get { return _args.Revision; }
		}

		internal SvnChangeItemCollection ChangedPaths
		{
			get { return _args.ChangedPaths; }
		}

        internal SvnLoggingEventArgs RawData
		{
			get { return _args; }
		}
    }

	sealed class LogItem : ISvnLogItem
	{
		readonly LogListViewItem _lvi;

		public LogItem(LogListViewItem lvi)
		{
			if (lvi == null)
				throw new ArgumentNullException("lvi");

			_lvi = lvi;
		}

		internal LogListViewItem ListViewItem
		{
			get { return _lvi; }
		}

		[Category("Subversion")]
		[DisplayName("Commit date")]
		public DateTime CommitDate
		{
			get { return _lvi.Date.ToLocalTime(); }
		}

		[Category("Subversion")]
		public string Author
		{
			get { return _lvi.Author; }
		}

		[Category("Subversion")]
		[DisplayName("Log message")]
		public string LogMessage
		{
			get { return _lvi.LogMessage; }
		}

		[Category("Subversion")]
		public long Revision
		{
			get { return _lvi.Revision; }
		}

        [Browsable(false)]
        public SvnChangeItemCollection ChangedPaths
        {
            get { return _lvi.RawData.ChangedPaths; }
        }
	}
}
