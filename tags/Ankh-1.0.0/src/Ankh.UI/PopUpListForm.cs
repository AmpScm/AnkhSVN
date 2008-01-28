using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;

namespace Ankh.UI
{
    public class PopUpListForm : Form
    {
        private ListBox listBox;

        public event EventHandler SelectionCommitted;

        public PopUpListForm()
        {
            this.InitializeComponent();

            this.Deactivate += new EventHandler( PopUpListForm_Deactivate );

            this.timer.Tick += new EventHandler( timer_Tick );
            this.listBox.SelectedIndexChanged += new EventHandler( listBox_SelectedIndexChanged );
        }

        void listBox_SelectedIndexChanged( object sender, EventArgs e )
        {
            this.timer.Start();
        }

        void timer_Tick( object sender, EventArgs e )
        {
            if ( this.SelectedValue != null )
            {
                PropertyDescriptor pd = TypeDescriptor.GetProperties( this.listBox.SelectedItem )[ this.toolTipMember ];
                MethodInfo mi = this.toolTip.GetType().GetMethod( "Show", new Type[] { typeof( string ), typeof( IWin32Window ) } );
                if ( pd != null && mi != null )
                {
                    string toolTipString = pd.GetValue( this.listBox.SelectedItem ).ToString();
                    mi.Invoke( this.toolTip, new object[] { toolTipString, this.listBox } );
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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listBox = new System.Windows.Forms.ListBox();
            this.toolTip = new System.Windows.Forms.ToolTip( this.components );
            this.timer = new System.Windows.Forms.Timer( this.components );
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.Location = new System.Drawing.Point( 0, 0 );
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size( 140, 104 );
            this.listBox.TabIndex = 0;
            this.listBox.DoubleClick += new System.EventHandler( this.listBox_DoubleClick );
            this.listBox.KeyDown += new System.Windows.Forms.KeyEventHandler( this.listBox_KeyDown );
            // 
            // timer
            // 
            this.timer.Interval = 500;
            // 
            // PopUpListForm
            // 
            this.ClientSize = new System.Drawing.Size( 140, 104 );
            this.ControlBox = false;
            this.Controls.Add( this.listBox );
            this.Name = "PopUpListForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler( this.listBox_KeyDown );
            this.ResumeLayout( false );

        }

        void PopUpListForm_Deactivate( object sender, EventArgs e )
        {
            this.Hide();
        }

        private void listBox_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Space )
            {
                this.CommitSelection();
            }
        }
        private void listBox_DoubleClick( object sender, EventArgs e )
        {
            this.CommitSelection();
        }

        private void CommitSelection()
        {
            if ( this.SelectionCommitted != null )
            {
                this.SelectionCommitted( this, EventArgs.Empty );
            }

            this.Hide();
        }

        private ToolTip toolTip;
        private System.ComponentModel.IContainer components;
        private Timer timer;

        private string toolTipMember;

       

    }
}
