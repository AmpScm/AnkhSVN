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
            this.propList = new System.Windows.Forms.ListView();
            this.nameColumn = new System.Windows.Forms.ColumnHeader();
            this.valueColumn = new System.Windows.Forms.ColumnHeader();
            this.addButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.modifyButton = new System.Windows.Forms.Button();
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
            // 
            // propList
            // 
            this.propList.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.propList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                       this.nameColumn,
                                                                                       this.valueColumn});
            this.propList.Location = new System.Drawing.Point(0, 144);
            this.propList.Name = "propList";
            this.propList.Size = new System.Drawing.Size(480, 97);
            this.propList.TabIndex = 6;
            this.propList.View = System.Windows.Forms.View.Details;
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
            this.addButton.Location = new System.Drawing.Point(224, 112);
            this.addButton.Name = "addButton";
            this.addButton.TabIndex = 3;
            this.addButton.Text = "&Add";
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.deleteButton.Location = new System.Drawing.Point(392, 112);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.TabIndex = 5;
            this.deleteButton.Text = "&Delete";
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
            // PropertyEditorDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(474, 295);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.modifyButton,
                                                                          this.cancelButton,
                                                                          this.okButton,
                                                                          this.deleteButton,
                                                                          this.addButton,
                                                                          this.propList,
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

        private class ListVisitor : IPropertyItemVisitor
        {
        }

        private ArrayList propItems;

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.ComboBox nameCombo;
        private System.Windows.Forms.ListView propList;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader valueColumn;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button modifyButton;
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


