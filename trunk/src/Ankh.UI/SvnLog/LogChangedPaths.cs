using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using Ankh.UI.SvnLog;
using Ankh.UI.Services;
using Ankh.Scc;
using Ankh.Ids;
using Ankh.Commands;
using Ankh.Selection;
using Ankh.Scc.UI;

namespace Ankh.UI
{
    public partial class LogChangedPaths : UserControl, ICurrentItemDestination<ISvnLogItem>
    {
        public LogChangedPaths()
        {
            InitializeComponent();
        }

        public LogChangedPaths(IContainer container)
            : this()
        {
            container.Add(this);

        }

		IAnkhServiceProvider _context;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IAnkhServiceProvider Context
		{
			get { return _context; }
			set
			{
				_context = value;
				changedPaths.SelectionPublishServiceProvider = value;


			}
		}

        #region ICurrentItemDestination<ISvnLogItem> Members
        ICurrentItemSource<ISvnLogItem> itemSource;
        public ICurrentItemSource<ISvnLogItem> ItemSource
        {
            get { return itemSource; }
            set
            {
                if (itemSource != null)
                {
                    itemSource.SelectionChanged -= new SelectionChangedEventHandler<ISvnLogItem>(SelectionChanged);
                    itemSource.FocusChanged -= new FocusChangedEventHandler<ISvnLogItem>(FocusChanged);
                }
                itemSource = value;
                if (itemSource != null)
                {
                    itemSource.SelectionChanged += new SelectionChangedEventHandler<ISvnLogItem>(SelectionChanged);
                    itemSource.FocusChanged += new FocusChangedEventHandler<ISvnLogItem>(FocusChanged);
                }

            }
        }

        #endregion

        IEnumerable<SvnItem> _wcItems;
        IEnumerable<SvnItem> WcItems
        {
            get
            {
                if (_wcItems != null)
                    return _wcItems;

                ISelectionContext selection = Context == null ? null : Context.GetService<ISelectionContext>();

                ILogControl logControl = selection == null ? null : selection.ActiveDialogOrFrameControl as ILogControl;

                if (logControl == null || !logControl.HasWorkingCopyItems)
                    _wcItems = null;
                else
                    _wcItems = logControl.WorkingCopyItems;

                return _wcItems;
            }
        }
        bool IsWorkingCopyItem(SvnChangeItem item)
        {
            if (WcItems == null)
                return true; // Don't gray out

            foreach (SvnItem i in WcItems)
            {
                if (i.Status.Uri.ToString().EndsWith(item.Path))
                    return true;
            }
            return false;
        }

        void SelectionChanged(object sender, IList<ISvnLogItem> e)
        {
        }



        void FocusChanged(object sender, ISvnLogItem e)
        {
            changedPaths.Items.Clear();

            if (e != null && e.ChangedPaths != null)
            {
                List<PathListViewItem> paths = new List<PathListViewItem>();
                foreach (SvnChangeItem i in e.ChangedPaths)
                {
                        
                    paths.Add(new PathListViewItem(changedPaths, e, i, IsWorkingCopyItem(i)));
                }

                changedPaths.Items.AddRange(paths.ToArray());
            }
        }

        internal void Reset()
        {
            changedPaths.Items.Clear();
        }

        

        private void changedPaths_ShowContextMenu(object sender, MouseEventArgs e)
        {
            Point p = MousePosition;

			if(Context != null)
			{
				IAnkhCommandService cs = Context.GetService<IAnkhCommandService>();
				cs.ShowContextMenu(AnkhCommandMenu.LogChangedPathsContextMenu, p.X, p.Y);
			}
        }

        private void changedPaths_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Point mp = changedPaths.PointToClient(MousePosition);
            ListViewHitTestInfo info = changedPaths.HitTest(mp);
            PathListViewItem lvi = info.Item as PathListViewItem;
            if (lvi != null)
            {
                IAnkhCommandService cmdSvc = Context.GetService<IAnkhCommandService>();
                cmdSvc.PostExecCommand(AnkhCommand.LogShowChanges);
            }
        }
    }
}
