// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
    /// <summary>
    /// Summary description for ConflictDialog.
    /// </summary>
    public class ConflictDialog : System.Windows.Forms.Form
    {
        public event EventHandler EditClicked;

        public enum Choice
        {
            Mine,
            OldRev,
            NewRev,
            ConflictMarkers
        }

        
        public ConflictDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.mineFileRadioButton.Checked = true;
            this.mineFileRadioButton.Tag = Choice.Mine;
            this.oldRevRadioButton.Tag = Choice.OldRev;
            this.newRevRadioButton.Tag = Choice.NewRev;
            this.fileRadioButton.Tag = Choice.ConflictMarkers;
            CreateMyToolTip();
        }

        public Choice Selection
        {
            get { return this.selectedChoice; }
        }

        public string Filename
        {
            get{ return this.filename; }
            set
            { 
                this.filename = value;
            }
        }

        public int OldRev
        {
            get { return this.oldRev; }
            set { this.oldRev = value; }
        }

        public int NewRev
        {
            get { return this.newRev; }
            set { this.newRev = value; }
        }
        /// <summary>
        /// Overrides base.OnVisibleChange. Sets button names.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged (EventArgs e) 
        {
            base.OnVisibleChanged ( e );
            this.mineFileRadioButton.Text = string.Format 
                ("{0}.mime", this.filename );
            this.oldRevRadioButton.Text = string.Format 
                ("{0}.r{1}", 
                this.filename, this.OldRev);
            this.newRevRadioButton.Text = string.Format 
                ("{0}.r{1}", 
                this.filename, this.NewRev);
            this.fileRadioButton.Text = string.Format 
                ("{0}", 
                this.filename);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        private void selectedButton(object sender, System.EventArgs e)
        {
            this.selectedChoice = (Choice)((RadioButton) sender).Tag;
        }

        private void editButton_Click(object sender, System.EventArgs e)
        {
            if (this.EditClicked != null)
                this.EditClicked (this, EventArgs.Empty); 
        }

        private void CreateMyToolTip()
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip conflictToolTip = new ToolTip(this.components);

            // Set up the delays in milliseconds for the ToolTip.
            conflictToolTip.AutoPopDelay = 5000;
            conflictToolTip.InitialDelay = 1000;
            conflictToolTip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            conflictToolTip.ShowAlways = true;
         
            // Set up the ToolTip text for the Button and Checkbox.
            conflictToolTip.SetToolTip( this.mineFileRadioButton, "Latest local file" ); 
            conflictToolTip.SetToolTip( this.oldRevRadioButton, "Latest updated revision" );
            conflictToolTip.SetToolTip( this.newRevRadioButton, "Latest version in repository" );
            conflictToolTip.SetToolTip( this.fileRadioButton, "File with conflict markers" );
            conflictToolTip.SetToolTip( this.editButton, "Selected file is edited" );
            conflictToolTip.SetToolTip( this.okButton, "Conflict is resolved and the three files that are not selected will be deleted" ); 
            conflictToolTip.SetToolTip( this.cancelButton, "Latest local file" );  
        }


		#region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mineFileRadioButton = new System.Windows.Forms.RadioButton();
            this.oldRevRadioButton = new System.Windows.Forms.RadioButton();
            this.newRevRadioButton = new System.Windows.Forms.RadioButton();
            this.fileRadioButton = new System.Windows.Forms.RadioButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.conflictLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mineFileRadioButton
            // 
            this.mineFileRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.mineFileRadioButton.Location = new System.Drawing.Point(16, 24);
            this.mineFileRadioButton.Name = "mineFileRadioButton";
            this.mineFileRadioButton.Size = new System.Drawing.Size(348, 24);
            this.mineFileRadioButton.TabIndex = 0;
            this.mineFileRadioButton.TabStop = true;
            this.mineFileRadioButton.Text = "test.mine";
            this.mineFileRadioButton.Click += new System.EventHandler(this.selectedButton);
            // 
            // oldRevRadioButton
            // 
            this.oldRevRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.oldRevRadioButton.Location = new System.Drawing.Point(16, 48);
            this.oldRevRadioButton.Name = "oldRevRadioButton";
            this.oldRevRadioButton.Size = new System.Drawing.Size(348, 24);
            this.oldRevRadioButton.TabIndex = 1;
            this.oldRevRadioButton.TabStop = true;
            this.oldRevRadioButton.Text = "test.r1";
            this.oldRevRadioButton.Click += new System.EventHandler(this.selectedButton);
            // 
            // newRevRadioButton
            // 
            this.newRevRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.newRevRadioButton.Location = new System.Drawing.Point(16, 72);
            this.newRevRadioButton.Name = "newRevRadioButton";
            this.newRevRadioButton.Size = new System.Drawing.Size(348, 24);
            this.newRevRadioButton.TabIndex = 2;
            this.newRevRadioButton.TabStop = true;
            this.newRevRadioButton.Text = "test.r2";
            this.newRevRadioButton.Click += new System.EventHandler(this.selectedButton);
            // 
            // fileRadioButton
            // 
            this.fileRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.fileRadioButton.Location = new System.Drawing.Point(16, 96);
            this.fileRadioButton.Name = "fileRadioButton";
            this.fileRadioButton.Size = new System.Drawing.Size(348, 24);
            this.fileRadioButton.TabIndex = 3;
            this.fileRadioButton.TabStop = true;
            this.fileRadioButton.Text = "test.txt";
            this.fileRadioButton.Click += new System.EventHandler(this.selectedButton);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(292, 129);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            // 
            // editButton
            // 
            this.editButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.editButton.Location = new System.Drawing.Point(132, 129);
            this.editButton.Name = "editButton";
            this.editButton.TabIndex = 4;
            this.editButton.Text = "Edit";
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(212, 129);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 5;
            this.okButton.Text = "Resolve";
            // 
            // conflictLabel
            // 
            this.conflictLabel.Location = new System.Drawing.Point(16, 5);
            this.conflictLabel.Name = "conflictLabel";
            this.conflictLabel.Size = new System.Drawing.Size(352, 16);
            this.conflictLabel.TabIndex = 7;
            this.conflictLabel.Text = "Select file:";
            // 
            // ConflictDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(376, 160);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.conflictLabel,
                                                                          this.okButton,
                                                                          this.editButton,
                                                                          this.cancelButton,
                                                                          this.fileRadioButton,
                                                                          this.newRevRadioButton,
                                                                          this.oldRevRadioButton,
                                                                          this.mineFileRadioButton});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ConflictDialog";
            this.Text = "Conflict";
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.RadioButton mineFileRadioButton;
        private System.Windows.Forms.RadioButton fileRadioButton;
        private Choice selectedChoice;

        private string filename;
        private int oldRev;
        private int newRev;
        private System.Windows.Forms.RadioButton oldRevRadioButton;
        private System.Windows.Forms.RadioButton newRevRadioButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Label conflictLabel;
        private System.ComponentModel.IContainer components;
        

        [STAThread] 
        public static void Main()
        {
            ConflictDialog the = new ConflictDialog();
            the.Filename = "Moo.cs";
            the.OldRev = 42;
            the.NewRev = 100;
            the.EditClicked += new EventHandler( test ); 
            the.ShowDialog();
            MessageBox.Show("You selected: " + the.Selection.ToString());
        }
        public static void test (object sender, EventArgs e)
        {
            ConflictDialog testD = (ConflictDialog) sender;
            MessageBox.Show ("You want to Edit: " +testD.Selection.ToString());

        }

  
        
    }

}

