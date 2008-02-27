using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpSvn;

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
            dataGridView1.DataSource = e == null ? null : e.ChangedPaths;
        }
    }
}
