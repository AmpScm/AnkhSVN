using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace UseCase
{
    


    public delegate void ItemAddedEventHandler( object sender, 
        string item );
    public delegate void ItemRemovedEventHandler( object sender, 
        object item );
	/// <summary>
	/// Summary description for ItemListUserControl.
	/// </summary>
	public class ItemListUserControl : System.Windows.Forms.UserControl
	{
        public event ItemAddedEventHandler Add;
        public event ItemRemovedEventHandler Delete;
        

		public ItemListUserControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

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

        


        public string Title
        {
            get{ return this.label.Text; }
            set{ this.label.Text = value; }
        }

        public IList Items
        {
            get{ return this.listBox.Items; }
            set
            { 
                this.listBox.Items.Clear();
                foreach (object item in value )
                    this.listBox.Items.Add( item );
            }
        }




		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.deleteButton = new System.Windows.Forms.Button();
            this.label = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.addTextBox = new System.Windows.Forms.TextBox();
            this.listBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // deleteButton
            // 
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteButton.Location = new System.Drawing.Point(376, 32);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 20);
            this.deleteButton.TabIndex = 15;
            this.deleteButton.Text = "Delete";
            this.deleteButton.Click += new System.EventHandler(this.deleteClick);
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(8, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(88, 23);
            this.label.TabIndex = 14;
            this.label.Text = "Title";
            // 
            // addButton
            // 
            this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addButton.Location = new System.Drawing.Point(360, 0);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 20);
            this.addButton.TabIndex = 12;
            this.addButton.Text = "Add";
            this.addButton.Click += new System.EventHandler(this.addClick);
            // 
            // addTextBox
            // 
            this.addTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.addTextBox.Location = new System.Drawing.Point(96, 0);
            this.addTextBox.Name = "addTextBox";
            this.addTextBox.Size = new System.Drawing.Size(248, 20);
            this.addTextBox.TabIndex = 11;
            this.addTextBox.Text = "";
            // 
            // listBox
            // 
            this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox.Location = new System.Drawing.Point(96, 32);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(272, 67);
            this.listBox.TabIndex = 13;
            this.listBox.TabStop = false;
            // 
            // ItemListUserControl
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.deleteButton,
                                                                          this.label,
                                                                          this.addButton,
                                                                          this.addTextBox,
                                                                          this.listBox});
            this.Name = "ItemListUserControl";
            this.Size = new System.Drawing.Size(456, 112);
            this.ResumeLayout(false);

        }
		#endregion

        


        private System.Windows.Forms.TextBox addTextBox;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Label label;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private void addClick(object sender, System.EventArgs e)
        {
            if ( this.addTextBox.Text.Trim() != string.Empty &&
                this.Add != null )
            {
                this.Add( this, this.addTextBox.Text );  
                this.addTextBox.Text = string.Empty;
            }
        }

        private void deleteClick(object sender, System.EventArgs e)
        {
            if ( this.listBox.SelectedItem != null && this.Delete != null )
                this.Delete( this, this.listBox.SelectedItem  );
        }


	}
}
