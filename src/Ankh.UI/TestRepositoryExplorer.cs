using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NSvn;

namespace Ankh.UI
{
	/// <summary>
	/// Summary description for TestRepositoryExplorer.
	/// </summary>
	public class TestRepositoryExplorer : System.Windows.Forms.Form
	{
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.Button goButton;
        private Ankh.UI.RepositoryTreeView repositoryTreeView1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TestRepositoryExplorer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.goButton = new System.Windows.Forms.Button();
            this.repositoryTreeView1 = new Ankh.UI.RepositoryTreeView();
            this.SuspendLayout();
            // 
            // urlTextBox
            // 
            this.urlTextBox.Location = new System.Drawing.Point(8, 3);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(112, 20);
            this.urlTextBox.TabIndex = 1;
            this.urlTextBox.Text = "http://svn.collab.net/repos/svn/trunk";
            // 
            // goButton
            // 
            this.goButton.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.goButton.Location = new System.Drawing.Point(152, 2);
            this.goButton.Name = "goButton";
            this.goButton.TabIndex = 2;
            this.goButton.Text = "Go";
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // repositoryTreeView1
            // 
            this.repositoryTreeView1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.repositoryTreeView1.ImageIndex = -1;
            this.repositoryTreeView1.Location = new System.Drawing.Point(0, 32);
            this.repositoryTreeView1.Name = "repositoryTreeView1";
            this.repositoryTreeView1.RepositoryRoot = null;
            this.repositoryTreeView1.SelectedImageIndex = -1;
            this.repositoryTreeView1.Size = new System.Drawing.Size(296, 240);
            this.repositoryTreeView1.TabIndex = 3;
            // 
            // TestRepositoryExplorer
            // 
            this.AcceptButton = this.goButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 271);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.repositoryTreeView1,
                                                                          this.goButton,
                                                                          this.urlTextBox});
            this.Name = "TestRepositoryExplorer";
            this.Text = "TestRepositoryExplorer";
            this.ResumeLayout(false);

        }
		#endregion

        private void goButton_Click(object sender, System.EventArgs e)
        {
            this.repositoryTreeView1.RepositoryRoot = 
                new RepositoryDirectory( this.urlTextBox.Text );
        }

        public static void Main()
        {
            Application.Run( new TestRepositoryExplorer() );
        }
	}
}
