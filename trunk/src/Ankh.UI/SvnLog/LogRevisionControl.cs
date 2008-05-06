using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using System.Globalization;
using System.Collections.ObjectModel;
using Ankh.UI.SvnLog;


namespace Ankh.UI
{
    public partial class LogRevisionControl : UserControl, ICurrentItemSource<SvnLogEventArgs>
    {
        int scrollPosition = 0;
        const int LogBatchSize = 10;
        const int ExtraBuffer = 5 * LogBatchSize;
        internal List<SvnLogEventArgs> logItems = new List<SvnLogEventArgs>();
        Queue<SvnLogEventArgs> logQueue = new Queue<SvnLogEventArgs>(LogBatchSize);

		internal int ScrollPosition
		{
			get { return scrollPosition; }
		}
        public LogRevisionControl()
        {
            InitializeComponent();
        }

        public LogRevisionControl(IContainer container)
            :this()
        {
            container.Add(this);
        }

		internal void Add(SvnLogEventArgs item)
		{
			logItems.Add(item);
			
		}

		internal void UpdateRowCount()
		{
			dataGridView1.RowCount = logItems.Count;
		}

        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if(e.RowIndex < logItems.Count)
                e.Value = CellValueForIndex(e.ColumnIndex, logItems[e.RowIndex]);
        }

        public Collection<SvnLogEventArgs> SelectedLogEvents
        {
            get
            {
                Collection<SvnLogEventArgs> rslt = new Collection<SvnLogEventArgs>();
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    rslt.Add(logItems[row.Index]);
                return rslt;
            }
        }
        
        private object CellValueForIndex(int columIndex, SvnLogEventArgs value)
        {
            if (value == null)
                return null;
            switch (columIndex)
            {
                case 0:
                    return value.Revision;
                case 1:
                    return value.Author;
                case 2:
                    return value.Time.ToLocalTime().ToString("F", CultureInfo.CurrentCulture);
                case 3:
					if (string.IsNullOrEmpty(value.LogMessage))
						return null;
                    string[] lines = value.LogMessage.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    return lines.Length > 0 ? lines[0] : "";
                default:
                    return null;
            }
        }

		public event EventHandler<ScrollEventArgs> ScrollPositionChanged;
		private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
		{
			scrollPosition = e.NewValue;
			if (ScrollPositionChanged != null)
				ScrollPositionChanged(this, e);
		}


        

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, SelectedItems);

            if (SelectedItems == null || SelectedItems.Count == 0)
                dataGridView1.ContextMenuStrip = null;
            else if (SelectedItems.Count == 1)
            {
                dataGridView1.ContextMenuStrip = singleItemContextMenu;
                long currentRev = SelectedItems[0].Revision;

                revertSingleToolStripMenuItem.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Strings.RevertChangesFromRevisionX,
                    currentRev);

                createBranchTagToolStripMenuItem.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Strings.CreateBranchTagFromRevisionX,
                    currentRev);

                switchToRevisionToolStripMenuItem.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Strings.SwitchToRevisionX,
                    currentRev);
            }
            else
            {
                dataGridView1.ContextMenuStrip = multipleItemContextMenu;
                long firstRev = SelectedItems[SelectedItems.Count - 1].Revision;
                long lastRev = SelectedItems[0].Revision;

                revertChangesToolStripMenuItem.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Strings.RevertChangesFromRevisionXToY,
                    firstRev,
                    lastRev);


                // Diff only works when 2 revisions are selected
                if (SelectedItems.Count == 2)
                {
                    compareToolStripMenuItem.Text = string.Format(
                        CultureInfo.CurrentCulture,
                        Strings.CompareRevisionXToY,
                        firstRev,
                        lastRev);
                    compareToolStripMenuItem.Visible = true;
                }
                else
                {
                    compareToolStripMenuItem.Visible = false;
                }
            }
        }



        #region ICurrentItemSource<SvnLogEventArgs> Members

        public event SelectionChangedEventHandler<SvnLogEventArgs> SelectionChanged;

        public event FocusChangedEventHandler<SvnLogEventArgs> FocusChanged;

        public SvnLogEventArgs FocusedItem
        {
            get 
            {
                if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index >= logItems.Count)
                    return null;
                return logItems[dataGridView1.CurrentRow.Index]; 
            }
        }

        public IList<SvnLogEventArgs> SelectedItems
        {
            get 
            {
                List<SvnLogEventArgs> rslt = new List<SvnLogEventArgs>(dataGridView1.SelectedRows.Count);
                foreach (DataGridViewRow r in dataGridView1.SelectedRows)
                    rslt.Add(logItems[r.Index]);
                return rslt;
            }
        }

        #endregion

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (FocusChanged != null)
                FocusChanged(this, FocusedItem);
        }

        private void singleRefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
//            Reset();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
  //          Reset();
        }
    }
}
