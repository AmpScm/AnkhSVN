using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI.VSSelectionControls;
using System.Windows.Forms;
using System.ComponentModel;
using SharpSvn;
using System.Runtime.InteropServices;
using System.Globalization;
using Ankh.VS;

namespace Ankh.UI.SvnLog
{
    public class LogRevisionView : ListViewWithSelection<LogItem>
    {
        public LogRevisionView()
        {
        }
        public LogRevisionView(IContainer container)
        {
            container.Add(this);
            VirtualItemsSelectionRangeChanged += new ListViewVirtualItemsSelectionRangeChangedEventHandler(LogRevisionView_VirtualItemsSelectionRangeChanged);
        }

        void LogRevisionView_VirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
        {
            GC.KeepAlive(e);
        }

    }

    public class LogItem : ListViewItem
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnLogEventArgs _args;
        public LogItem(IAnkhServiceProvider context, SvnLogEventArgs e)
        {
            _args = e;
            _context = context;
            RefreshText();
        }

        internal SvnLogEventArgs RawData
        {
            get
            {
                return _args;
            }
        }
        IAnkhVSColor VSColor
        {
            get { return _context.GetService<IAnkhVSColor>(); }
        }

        void RefreshText()
        {
            Text = _args.Revision.ToString(CultureInfo.CurrentCulture);
            SubItems.Add(new ListViewSubItem(this, _args.Author));
            SubItems.Add(new ListViewSubItem(this, _args.Time.ToString(CultureInfo.CurrentCulture)));
            SubItems.Add(new ListViewSubItem(this, _args.LogMessage));
        }
    }
}
