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
            this.newButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.propertyEditor = new Ankh.UI.PlainPropertyEditor();
            this.valueLabel = new System.Windows.Forms.Label();
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
            this.nameCombo.TextChanged += new System.EventHandler(this.nameCombo_TextChanged);
            // 
            // propListView
            // 
            this.propListView.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.propListView.AutoArrange = false;
            this.propListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                           this.nameColumn,
                                                                                           this.valueColumn});
            this.propListView.Location = new System.Drawing.Point(0, 224);
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
            // newButton
            // 
            this.newButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.newButton.Location = new System.Drawing.Point(224, 192);
            this.newButton.Name = "newButton";
            this.newButton.TabIndex = 3;
            this.newButton.Text = "&New";
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(392, 192);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.TabIndex = 5;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(304, 344);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 7;
            this.okButton.Text = "&Ok";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(392, 344);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "&Cancel";
            // 
            // saveButton
            // 
            this.saveButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(308, 192);
            this.saveButton.Name = "saveButton";
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "&Save";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // propertyEditor
            // 
            this.propertyEditor.Location = new System.Drawing.Point(68, 48);
            this.propertyEditor.Name = "propertyEditor";
            this.propertyEditor.Size = new System.Drawing.Size(240, 128);
            this.propertyEditor.TabIndex = 9;
            this.propertyEditor.Changed += new System.EventHandler(this.propertyEditor_Changed);
            // 
            // valueLabel
            // 
            this.valueLabel.Location = new System.Drawing.Point(24, 56);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(40, 16);
            this.valueLabel.TabIndex = 10;
            this.valueLabel.Text = "Value:";
            // 
            // PropertyEditorDialog
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(474, 375);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.valueLabel,
                                                                          this.propertyEditor,
                                                                          this.saveButton,
                                                                          this.cancelButton,
                                                                          this.okButton,
                                                                          this.deleteButton,
                                                                          this.newButton,
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
               this.AddItem( new string[]{ item.Name, 
                    item.Text.Replace("\t", "    ").Replace( "\r\n", "[NL]") }, item );
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



        private void newButton_Click(object sender, System.EventArgs e)
        {
            this.propertyEditor.Clear();
            nameCombo.Text = "";
            this.propListView.SelectedItems.Clear();
        }
        private void deleteButton_Click(object sender, System.EventArgs e)
        {
            object item = this.propListView.SelectedItems[0].Tag;
            this.propItems.Remove(item);

            this.PopulateListView();

            this.ValidateForm();
        }


        private void saveButton_Click(object sender, System.EventArgs e)
        {
            
            PropertyItem item= this.propertyEditor.PropertyItem;
            item.Name = nameCombo.Text;
            
            if (this.propListView.SelectedItems.Count > 0)
            {
                object selectedItem = this.propListView.SelectedItems[0].Tag;
                int index = this.propItems.IndexOf(selectedItem);
                this.propItems[index] = item;
            }
            else
            {
                this.propItems.Add(item);
            }

            this.PopulateListView();
            this.propertyEditor.Clear();
            nameCombo.Text = "";

        }

        private void propListView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.propListView.SelectedItems.Count > 0)
            {
                TextPropertyItem item = (TextPropertyItem)this.propListView.SelectedItems[0].Tag;
                this.nameCombo.Text = item.Name;
 
                this.propertyEditor.PropertyItem = item;
            }
            this.ValidateForm();
        }   

        private void propertyEditor_Changed(object sender, System.EventArgs e)
        {
            this.ValidateForm();
        }

        private void nameCombo_TextChanged(object sender, System.EventArgs e)
        {
            this.ValidateForm();
        }

        private void ValidateForm ()
        {
            this.deleteButton.Enabled = this.propListView.SelectedItems.Count > 0;
            this.saveButton.Enabled = this.nameCombo.Text.Trim() != "" && 
              this.propertyEditor.Valid;

        }  


        private ArrayList propItems;

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.ComboBox nameCombo;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader valueColumn;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.ListView propListView;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label valueLabel;
        private Ankh.UI.PlainPropertyEditor propertyEditor;
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


