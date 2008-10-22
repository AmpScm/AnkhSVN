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
    partial class LogChangedPaths : UserControl, ICurrentItemDestination<ISvnLogItem>
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

        LogDataSource _dataSource;
        public LogDataSource LogSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
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

        

        void SelectionChanged(object sender, IList<ISvnLogItem> e)
        {
        }



        void FocusChanged(object sender, ISvnLogItem e)
        {
            changedPaths.Items.Clear();

            if (e != null && e.ChangedPaths != null)
            {
                List<PathListViewItem> paths = new List<PathListViewItem>();

                List<string> origins = new List<string>();
                foreach (SvnOrigin o in LogSource.Targets)
                {
                    string origin = SvnTools.UriPartToPath(o.RepositoryRoot.MakeRelativeUri(o.Uri).ToString()).Replace('\\', '/');
                    if (origin.Length == 0 || origin[0] != '/')
                        origin = "/" + origin;

                    origins.Add(origin.TrimEnd('/'));
                }

                foreach (SvnChangeItem i in e.ChangedPaths)
                {
                    paths.Add(new PathListViewItem(changedPaths, e, i, e.RepositoryRoot, HasFocus(origins, i.Path)));
                }

                changedPaths.Items.AddRange(paths.ToArray());
            }
        }

        static bool HasFocus(IEnumerable<string> originPaths, string itemPath)
        {
            foreach (string origin in originPaths)
            {
                if (!itemPath.StartsWith(origin))
                    continue;

                int n = itemPath.Length - origin.Length;

                if (n == 0)
                    return true;

                if (n > 0)
                {
                    if (itemPath[origin.Length] == '/')
                        return true;
                }
            }

            return false;
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
