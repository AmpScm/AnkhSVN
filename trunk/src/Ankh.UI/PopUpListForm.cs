using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;

namespace Ankh.UI
{
    public partial class PopUpListForm : Form
    {
        public event EventHandler SelectionCommitted;

        public PopUpListForm()
        {
            this.InitializeComponent();

            this.Deactivate += new EventHandler(PopUpListForm_Deactivate);

            this.timer.Tick += new EventHandler(timer_Tick);
            this.listBox.SelectedIndexChanged += new EventHandler(listBox_SelectedIndexChanged);
        }

        void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (this.SelectedValue != null)
            {
                PropertyDescriptor pd = TypeDescriptor.GetProperties(this.listBox.SelectedItem)[this.toolTipMember];
                MethodInfo mi = this.toolTip.GetType().GetMethod("Show", new Type[] { typeof(string), typeof(IWin32Window) });
                if (pd != null && mi != null)
                {
                    string toolTipString = pd.GetValue(this.listBox.SelectedItem).ToString();
                    mi.Invoke(this.toolTip, new object[] { toolTipString, this.listBox });
                    //this.toolTip.Show( toolTipString, this.listBox );
                }
            }
            this.timer.Stop();
        }

        public string ValueMember
        {
            get { return this.listBox.ValueMember; }
            set { this.listBox.ValueMember = value; }
        }

        public string DisplayMember
        {
            get { return this.listBox.DisplayMember; }
            set { this.listBox.DisplayMember = value; }
        }

        public object DataSource
        {
            get { return this.listBox.DataSource; }
            set { this.listBox.DataSource = value; }
        }

        public string ToolTipMember
        {
            get { return this.toolTipMember; }
            set { this.toolTipMember = value; }
        }

        public object SelectedValue
        {
            get { return this.listBox.SelectedValue; }
            set { this.listBox.SelectedValue = value; }
        }



        void PopUpListForm_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Space)
            {
                this.CommitSelection();
            }
        }
        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            this.CommitSelection();
        }

        private void CommitSelection()
        {
            if (this.SelectionCommitted != null)
            {
                this.SelectionCommitted(this, EventArgs.Empty);
            }

            this.Hide();
        }

        private ToolTip toolTip;
        private string toolTipMember;
    }
}
