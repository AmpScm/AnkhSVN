// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NSvn;
using NSvn.Core;
using System.Text.RegularExpressions;

namespace Ankh.UI
{
	/// <summary>
	/// Gives a tree view of the repository based on revision.
	/// </summary>
	public class RepositoryExplorerControl : System.Windows.Forms.UserControl
	{		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public RepositoryExplorerControl()
		{
			//This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			//Set revision choices in combobox
			this.revisionComboBox.Items.AddRange( new object[]{
																	  RevisionChoice.Head,
																	  RevisionChoice.Prev,
																	  RevisionChoice.Base,
																	  RevisionChoice.Committed,
																	  RevisionChoice.Date});
			//Head is set to default revision
			this.revisionComboBox.SelectedIndex = 0;
			
            this.components = new System.ComponentModel.Container();
			RepositoryExplorerToolTip();
		}

        /// <summary>
        /// Adds a new menu item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="position">The The position to put the new item.</param>
        public void AddMenuItem( MenuItem item, int position )
        {
            this.treeView.ContextMenu.MenuItems.Add( position, item );
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

		private enum RevisionChoice
		{
			Date,
			Head,
			Base,
			Committed,
			Prev
		}
	
		//Enables the DateTimePicker if date is selected
		private void EnableAndDisableDateTimePicker()
		{
			if (this.revisionComboBox.SelectedItem != null)
			{
				if ((RevisionChoice) this.revisionComboBox.SelectedItem == 
					RevisionChoice.Date)
				{
					this.dateTimePicker.Enabled = true;
				}
				else
				{
					this.dateTimePicker.Enabled = false;
				}	
			}
		}

		//Checks whether the selected revision is a number
		private bool ValidateRevisionNumber(string text)
		{
			return validRevisionNumber.IsMatch( text );
		}

		private void RepositoryExplorerToolTip()
		{
			// Create the ToolTip and associate with the Form container.
			ToolTip ToolTip = new ToolTip(this.components);

			// Set up the delays in milliseconds for the ToolTip.
			ToolTip.AutoPopDelay = 5000;
			ToolTip.InitialDelay = 1000;
			ToolTip.ReshowDelay = 500;
			// Force the ToolTip text to be displayed whether or not the form is active.
			ToolTip.ShowAlways = true;
         
			// Set up the ToolTip text for the Button and Checkbox.
			ToolTip.SetToolTip( this.revisionComboBox, 
				"Select or print a revision number that will start the log" ); 
			ToolTip.SetToolTip( this.urlTextBox, 
				"Write the url to your repository" );
			ToolTip.SetToolTip( this.dateTimePicker, 
				"Select a date from the calendar" );
			ToolTip.SetToolTip( this.treeView, 
				"Select a date from the calendar" );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.urlLabel = new System.Windows.Forms.Label();
			this.urlTextBox = new System.Windows.Forms.TextBox();
			this.revisionLabel = new System.Windows.Forms.Label();
			this.revisionComboBox = new System.Windows.Forms.ComboBox();
			this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.goButton = new System.Windows.Forms.Button();
			this.treeView = new Ankh.UI.RepositoryTreeView();
			this.SuspendLayout();
			// 
			// urlLabel
			// 
			this.urlLabel.Location = new System.Drawing.Point(1, 8);
			this.urlLabel.Name = "urlLabel";
			this.urlLabel.Size = new System.Drawing.Size(24, 23);
			this.urlLabel.TabIndex = 0;
			this.urlLabel.Text = "Url:";
			// 
			// urlTextBox
			// 
			this.urlTextBox.Location = new System.Drawing.Point(28, 5);
			this.urlTextBox.Name = "urlTextBox";
			this.urlTextBox.Size = new System.Drawing.Size(220, 20);
			this.urlTextBox.TabIndex = 1;
			this.urlTextBox.Text = "http://arild.no-ip.com:8088/svn/test";
			// 
			// revisionLabel
			// 
			this.revisionLabel.Location = new System.Drawing.Point(1, 29);
			this.revisionLabel.Name = "revisionLabel";
			this.revisionLabel.TabIndex = 2;
			this.revisionLabel.Text = "Select Revision:";
			// 
			// revisionComboBox
			// 
			this.revisionComboBox.Location = new System.Drawing.Point(2, 45);
			this.revisionComboBox.Name = "revisionComboBox";
			this.revisionComboBox.Size = new System.Drawing.Size(121, 21);
			this.revisionComboBox.TabIndex = 3;
			this.revisionComboBox.SelectedIndexChanged += new System.EventHandler(this.revisionComboBox_SelectedIndexChanged);
			// 
			// dateTimePicker
			// 
			this.dateTimePicker.Enabled = false;
			this.dateTimePicker.Location = new System.Drawing.Point(136, 45);
			this.dateTimePicker.Name = "dateTimePicker";
			this.dateTimePicker.TabIndex = 4;
			// 
			// goButton
			// 
			this.goButton.Location = new System.Drawing.Point(256, 4);
			this.goButton.Name = "goButton";
			this.goButton.TabIndex = 5;
			this.goButton.Text = "Go";
			this.goButton.Click += new System.EventHandler(this.goButton_Click);
			// 
			// treeView
			// 
			this.treeView.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.treeView.Enabled = false;
			this.treeView.ImageIndex = -1;
			this.treeView.Location = new System.Drawing.Point(0, 72);
			this.treeView.Name = "treeView";
			this.treeView.RepositoryRoot = null;
			this.treeView.SelectedImageIndex = -1;
			this.treeView.Size = new System.Drawing.Size(336, 240);
			this.treeView.TabIndex = 6;
			// 
			// RepositoryExplorerControl
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.treeView,
																		  this.goButton,
																		  this.dateTimePicker,
																		  this.revisionComboBox,
																		  this.revisionLabel,
																		  this.urlTextBox,
																		  this.urlLabel});
			this.Name = "RepositoryExplorerControl";
			this.Size = new System.Drawing.Size(344, 320);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label urlLabel;
		private System.Windows.Forms.TextBox urlTextBox;
		private System.Windows.Forms.Label revisionLabel;
		private System.Windows.Forms.ComboBox revisionComboBox;
		private System.Windows.Forms.DateTimePicker dateTimePicker;
		private Ankh.UI.RepositoryTreeView treeView;
        private System.Windows.Forms.Button goButton;
		private static readonly Regex validRevisionNumber = 
			new Regex(@"\d+", RegexOptions.Compiled);
		
		//Gives a tree view of repository if valid revision is selected
		private void goButton_Click(object sender, System.EventArgs e)
		{
            this.Cursor = Cursors.WaitCursor;
            this.goButton.Enabled = false;
			if ( this.revisionComboBox.SelectedItem != null )
			{
				this.treeView.Enabled = true;
				Revision revision = Revision.Head;	 

				//if revision is a number
				if ( ValidateRevisionNumber( this.revisionComboBox.Text ))
				{
					this.treeView.RepositoryRoot = 
						new RepositoryDirectory( this.urlTextBox.Text
						,NSvn.Core.Revision.FromNumber( int.Parse( this.revisionComboBox.ToString() )));
				}
					//if revision is a date
				else if ((RevisionChoice) this.revisionComboBox.SelectedItem == 
					RevisionChoice.Date)
				{
					this.treeView.RepositoryRoot = 
						new RepositoryDirectory( this.urlTextBox.Text
					//TODO - this part don't work yet. Don't know what to do cause I dont get any error message
						,NSvn.Core.Revision.FromDate( this.dateTimePicker.Value ));
				}
				else //if revision is a text
				{
					if (this.revisionComboBox.SelectedItem.ToString().Equals("Head"))
					{
						revision = Revision.Head;
					}
					//Throws an exception from svn 2.4.2003
					else if (this.revisionComboBox.SelectedItem.ToString().Equals("Base"))
					{
						revision = Revision.Base;
					}
					//Throws an exception from svn 2.4.2003
					else if (this.revisionComboBox.SelectedItem.ToString().Equals("Committed"))
					{
						revision = Revision.Committed;
					}
					//Throws an exception from svn 2.4.2003
					else if (this.revisionComboBox.SelectedItem.ToString().Equals("Prev"))
					{
						revision = Revision.Previous;
					}
					this.treeView.RepositoryRoot = 
						new RepositoryDirectory( this.urlTextBox.Text, revision );
				}

                this.Cursor = Cursors.Default;
                this.goButton.Enabled = true;

			}
		}

		private void revisionComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			EnableAndDisableDateTimePicker();
		}		
	}
}



