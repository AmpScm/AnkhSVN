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

        IAnkhUISite _site;
        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;

                IAnkhUISite site = value as IAnkhUISite;

                if (site != null)
                {
                    _site = site;

                    OnUISiteChanged(EventArgs.Empty);
                }
            }
        }

        protected void OnUISiteChanged(EventArgs e)
        {
            changedPaths.SelectionPublishServiceProvider = _site;
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
            foreach (SvnChangeItem i in e.ChangedPaths)
                changedPaths.Items.Add(new PathListViewItem(i));
        }

        internal void Reset()
        {
            changedPaths.Items.Clear();
        }
    }
}
