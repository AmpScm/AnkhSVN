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
        private Ankh.UI.RepositoryExplorerControl repositoryExplorerControl1;
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

        public void AddMenuItem( MenuItem item, int position )
        {
            this.repositoryExplorerControl1.AddMenuItem( item, position );
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
            this.repositoryExplorerControl1 = new Ankh.UI.RepositoryExplorerControl();
            this.SuspendLayout();
            // 
            // repositoryExplorerControl1
            // 
            this.repositoryExplorerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.repositoryExplorerControl1.Name = "repositoryExplorerControl1";
            this.repositoryExplorerControl1.Size = new System.Drawing.Size(384, 307);
            this.repositoryExplorerControl1.TabIndex = 0;
            // 
            // TestRepositoryExplorer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(384, 307);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.repositoryExplorerControl1});
            this.Name = "TestRepositoryExplorer";
            this.Text = "TestRepositoryExplorer";
            this.ResumeLayout(false);

        }
		#endregion

        private void goButton_Click(object sender, System.EventArgs e)
        {
//            this.repositoryTreeView1.RepositoryRoot = 
//                new RepositoryDirectory( this.urlTextBox.Text );
        }

        public static void Main()
        {
            Application.Run( new TestRepositoryExplorer() );
//            Ankh.Commands.RegisterCommands( null );
        }
	}
}
