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

namespace Ankh.UI
{
    public partial class LogChangedPaths : UserControl, ICurrentItemDestination<SvnLogEventArgs>
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


        #region ICurrentItemDestination<SvnLogEventArgs> Members
        ICurrentItemSource<SvnLogEventArgs> itemSource;
        public ICurrentItemSource<SvnLogEventArgs> ItemSource
        {
            get { return itemSource; }
            set
            {
                if (itemSource != null)
                {
                    itemSource.SelectionChanged -= new SelectionChangedEventHandler<SvnLogEventArgs>(SelectionChanged);
                    itemSource.FocusChanged -= new FocusChangedEventHandler<SvnLogEventArgs>(FocusChanged);
                }
                itemSource = value;
                if (itemSource != null)
                {
                    itemSource.SelectionChanged += new SelectionChangedEventHandler<SvnLogEventArgs>(SelectionChanged);
                    itemSource.FocusChanged += new FocusChangedEventHandler<SvnLogEventArgs>(FocusChanged);
                }
            }
        }

        #endregion

        void SelectionChanged(object sender, IList<SvnLogEventArgs> e)
        {
        }

        void FocusChanged(object sender, SvnLogEventArgs e)
        {
            changedPaths.Items.Clear();
            foreach (SvnChangeItem i in e.ChangedPaths)
                changedPaths.Items.Add(new PathListViewItem(i));
        }
    }
}
