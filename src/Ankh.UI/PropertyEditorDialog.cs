// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
	/// <summary>
	/// Dialog for editing svn properties. 
	/// </summary>
	public class PropertyEditorDialog : System.Windows.Forms.Form
	{
        

		public PropertyEditorDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            this.propItems = new ArrayList();
		}

        public PropertyItem[] PropertyItems
        {
            get
            {
                return (PropertyItem[])
                    this.propItems.ToArray(typeof(PropertyItem));
            }
            set
            {
                this.propItems.Clear();
                this.propItems.AddRange(value);
                this.PopulateListView();
            }
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
            this.nameLabel = new System.Windows.Forms.Label();
            this.nameCombo = new System.Windows.Forms.ComboBox();
            this.propListView = new System.Windows.Forms.ListView();
            this.nameColumn = new System.Windows.Forms.ColumnHeader();
            this.valueColumn = new System.Windows.Forms.ColumnHeader();
            this.addButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.modifyButton = new System.Windows.Forms.Button();
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.Location = new System.Drawing.Point(24, 24);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(40, 16);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Name:";
            // 
            // nameCombo
            // 
            this.nameCombo.Location = new System.Drawing.Point(69, 21);
            this.nameCombo.Name = "nameCombo";
            this.nameCombo.Size = new System.Drawing.Size(121, 21);
            this.nameCombo.TabIndex = 1;
            this.nameCombo.TextChanged += new System.EventHandler(this.valueTextBox_TextChanged);
            // 
            // propListView
            // 
            this.propListView.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.propListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                           this.nameColumn,
                                                                                           this.valueColumn});
            this.propListView.Location = new System.Drawing.Point(0, 144);
            this.propListView.Name = "propListView";
            this.propListView.Size = new System.Drawing.Size(480, 97);
            this.propListView.TabIndex = 6;
            this.propListView.View = System.Windows.Forms.View.Details;
            this.propListView.SelectedIndexChanged += new System.EventHandler(this.propListView_SelectedIndexChanged);
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "Name";
            this.nameColumn.Width = 168;
            // 
            // valueColumn
            // 
            this.valueColumn.Text = "Value";
            this.valueColumn.Width = 308;
            // 
            // addButton
            // 
            this.addButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.addButton.Enabled = false;
            this.addButton.Location = new System.Drawing.Point(224, 112);
            this.addButton.Name = "addButton";
            this.addButton.TabIndex = 3;
            this.addButton.Text = "&Add";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(392, 112);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.TabIndex = 5;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(304, 264);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 7;
            this.okButton.Text = "&Ok";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(392, 264);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "&Cancel";
            // 
            // modifyButton
            // 
            this.modifyButton.Location = new System.Drawing.Point(308, 112);
            this.modifyButton.Name = "modifyButton";
            this.modifyButton.TabIndex = 4;
            this.modifyButton.Text = "&Modify";
            // 
            // valueTextBox
            // 
            this.valueTextBox.Location = new System.Drawing.Point(72, 56);
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.TabIndex = 2;
            this.valueTextBox.Text = "";
            this.valueTextBox.TextChanged += new System.EventHandler(this.valueTextBox_TextChanged);
            // 
            // PropertyEditorDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(474, 295);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.valueTextBox,
                                                                          this.modifyButton,
                                                                          this.cancelButton,
                                                                          this.okButton,
                                                                          this.deleteButton,
                                                                          this.addButton,
                                                                          this.propListView,
                                                                          this.nameCombo,
                                                                          this.nameLabel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PropertyEditorDialog";
            this.Text = "Edit Properties";
            this.ResumeLayout(false);

        }
		#endregion

        public static void Main()
        {
            PropertyEditorDialog pop = new PropertyEditorDialog();
            pop.ShowDialog();
        }

        private void PopulateListView()
        {
            this.propListView.Items.Clear();
            ListVisitor visitor= new ListVisitor(this.propListView);
            foreach (PropertyItem item in this.propItems)
                item.AcceptVisitor(visitor);
        }
        #region class ListVitor
        /// <summary>
        /// Visitor that populates the list view.
        /// </summary>
        private class ListVisitor : IPropertyItemVisitor
        {
            public ListVisitor(ListView list)
            {
                this.list = list;
            }

            public void VisitTextPropertyItem(TextPropertyItem item)
            {
               this.AddItem( new string[]{ item.Name, item.Text }, item );
            }

            public void VisitBinaryPropertyItem(BinaryPropertyItem item)
            {
                this.AddItem( new string[]{ item.Name, "<binary value>" }, item );
                
            }

            private void AddItem(string[] items, PropertyItem item)
            {
                ListViewItem listItem = new ListViewItem( items );
                listItem.Tag = item;
                this.list.Items.Add(listItem);
            }
                                                                          
            private ListView list;
        }
        #endregion



        private void addButton_Click(object sender, System.EventArgs e)
        {
            PropertyItem item= new TextPropertyItem(valueTextBox.Text);
            item.Name = nameCombo.Text;
            this.propItems.Add(item);
            this.PopulateListView();

            valueTextBox.Text ="";
            nameCombo.Text = "";
        }
        private void deleteButton_Click(object sender, System.EventArgs e)
        {
            object item = this.propListView.SelectedItems[0].Tag;
            this.propItems.Remove(item);

            this.PopulateListView();

            this.ValidateForm();
        }


        private void propListView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.ValidateForm();
        }
        
        private void valueTextBox_TextChanged(object sender, System.EventArgs e)
        {
            this.ValidateForm();
        }

        private void ValidateForm ()
        {
            this.deleteButton.Enabled = this.propListView.SelectedItems.Count > 0;
            this.addButton.Enabled = this.nameCombo.Text.Trim() != "" && 
                this.valueTextBox.Text.Trim() != "";
        }

        


        private ArrayList propItems;

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.ComboBox nameCombo;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader valueColumn;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button modifyButton;
        private System.Windows.Forms.ListView propListView;
        private System.Windows.Forms.TextBox valueTextBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        

        

        

        

	}

    /// <summary>
    /// Represents a property item.
    /// </summary>
    public abstract class PropertyItem
    {
        protected PropertyItem( )
        {
            this.name="";
        }
        

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public abstract void AcceptVisitor(IPropertyItemVisitor visitor);
        
        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name
        { 
            get{ return this.name;}
            set { this.name = value;}
        }
        
        
        private string name;
       

    }
    /// <summary>
    /// Represents a text property.
    /// </summary>
    public class TextPropertyItem : PropertyItem
    {
        public TextPropertyItem(string text)
        {
            this.text = text;
        }
        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <param name="visitor">A visitor.</param>
        public override void AcceptVisitor(IPropertyItemVisitor visitor)
        {
            visitor.VisitTextPropertyItem(this);
        }
        /// <summary>
        /// The text value of this property.
        /// </summary>
        public string Text
        {
            get { return this.text;}
        }
 
        private string text;    
    }
      
    /// <summary>
    /// Represents a binary property.
    /// </summary>
    public class BinaryPropertyItem : PropertyItem
    {
        public BinaryPropertyItem(byte[] data)
        {
            this.data = data;
        }
    
        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <param name="visitor">A visitor.</param>
        public override void AcceptVisitor(IPropertyItemVisitor visitor)
        {
            visitor.VisitBinaryPropertyItem(this);
        }

        /// <summary>
        /// Binary data belonging to this property.
        /// </summary>
        public byte[] Data
        {
            get { return this.data;}
        }
 
        private byte[] data;    
    }

    /// <summary>
    /// Visitor for visiting property items.
    /// </summary>
    public interface IPropertyItemVisitor
    {
        /// <summary>
        /// Visit a text property.
        /// </summary>
        /// <param name="item"></param>
        void VisitTextPropertyItem(TextPropertyItem item);
        
        /// <summary>
        /// Visit a binary property.
        /// </summary>
        /// <param name="item"></param>
        void VisitBinaryPropertyItem(BinaryPropertyItem item);
    }


}


