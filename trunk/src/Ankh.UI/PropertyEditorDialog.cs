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

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();

            SetNewEditor(new PlainPropertyEditor());
			
            this.propItems = new ArrayList();

            this.nameCombo.Items.Add(new ExecutablePropertyEditor());
            this.nameCombo.Items.Add(new MimeTypePropertyEditor());
            this.nameCombo.Items.Add(new IgnorePropertyEditor());
            this.nameCombo.Items.Add(new KeywordsPropertyEditor());
            this.nameCombo.Items.Add(new EolStylePropertyEditor());
            this.nameCombo.Items.Add(new ExternalsPropertyEditor()); 
            //this.nameCombo.SelectedIndex = this.nameCombo.Items.IndexOf();
         }

        /// <summary>
        /// Sets and gets property items.
        /// </summary>
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
            this.editorPanel = new System.Windows.Forms.Panel();
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
            this.nameCombo.SelectedValueChanged += new System.EventHandler(this.nameCombo_SelectedValueChanged);
            // 
            // propListView
            // 
            this.propListView.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.propListView.AutoArrange = false;
            this.propListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                           this.nameColumn,
                                                                                           this.valueColumn});
            this.propListView.GridLines = true;
            this.propListView.Location = new System.Drawing.Point(0, 296);
            this.propListView.Name = "propListView";
            this.propListView.Scrollable = false;
            this.propListView.Size = new System.Drawing.Size(544, 112);
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
            this.valueColumn.Width = 544;
            // 
            // newButton
            // 
            this.newButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.newButton.Location = new System.Drawing.Point(8, 264);
            this.newButton.Name = "newButton";
            this.newButton.TabIndex = 3;
            this.newButton.Text = "Reset";
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(456, 264);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.TabIndex = 5;
            this.deleteButton.Text = "Delete";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(368, 416);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 7;
            this.okButton.Text = "Save";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(456, 416);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            // 
            // saveButton
            // 
            this.saveButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(372, 264);
            this.saveButton.Name = "saveButton";
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Add";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // editorPanel
            // 
            this.editorPanel.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.editorPanel.Location = new System.Drawing.Point(64, 56);
            this.editorPanel.Name = "editorPanel";
            this.editorPanel.Size = new System.Drawing.Size(456, 192);
            this.editorPanel.TabIndex = 2;
            this.editorPanel.TabStop = true;
            // 
            // PropertyEditorDialog
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(538, 447);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.editorPanel,
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

        /// <summary>
        /// List the property items defined.
        /// </summary>
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
                                              AddItem( new string[]{ item.Name, 
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

        /// <summary>
        /// Clear/default the values in the selected editor if new-button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newButton_Click(object sender, System.EventArgs e)
        {
            this.currentEditor.Reset();
            nameCombo.Text = "";
            this.propListView.SelectedItems.Clear();
        }

        /// <summary>
        /// Delete the selected item if delete-button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, System.EventArgs e)
        {
            object item = this.propListView.SelectedItems[0].Tag;
            this.propItems.Remove(item);
            this.PopulateListView();

            this.ValidateForm();
        }

        /// <summary>
        /// Save the valid item if save-button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, System.EventArgs e)
        {
            
            PropertyItem item= this.currentEditor.PropertyItem;
            item.Name = nameCombo.Text;
            
            // anything selected in the property list?
            if (this.propListView.SelectedItems.Count > 0)
            {
                // yup - find the property item associated with the selection and replace it
                object selectedItem = this.propListView.SelectedItems[0].Tag;
                int index = this.propItems.IndexOf(selectedItem);
                this.propItems[index] = item;
            }
            else
            {
                this.propItems.Add(item);
            }

            this.PopulateListView();
            this.currentEditor.Reset();
            nameCombo.Text = "";
        }

        /// <summary>
        /// Checks whether a predefined property is selected.
        /// If selected sets the editor to the selected item.
        /// Validate the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propListView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.propListView.SelectedItems.Count > 0)
            {
                TextPropertyItem item = (TextPropertyItem)this.propListView.SelectedItems[0].Tag;
                this.nameCombo.Text = item.Name;

                //Check whether a predefined property is selected.
                //If selected sets the editor to the selected item.
                //HACK: find better way
                foreach( object o in this.nameCombo.Items )
                { 
                    if ( o.ToString() == item.Name )
                        this.SetNewEditor( (IPropertyEditor)o );
                }
 
                this.currentEditor.PropertyItem = item;
            }

            this.ValidateForm();
        }   
        
        /// <summary>
        /// Validate the form if the type of editor is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void currentEditor_Changed(object sender, System.EventArgs e)
        {
            this.ValidateForm();
        }

        /// <summary>
        /// Sets plain editor if the text in the combo has beed modified.
        /// Validate the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nameCombo_TextChanged(object sender, System.EventArgs e)
        { 
            if ( this.nameCombo.SelectedItem != null)
            {
                SetNewEditor(new PlainPropertyEditor());   
            }
            this.ValidateForm();
        }

        /// <summary>
        /// Sets a new property editor.
        /// </summary>
        /// <param name="editor"></param>
        private void SetNewEditor(IPropertyEditor editor)
        { 
            if (this.currentEditor != null)
            {
                //Unsubscribe the current editor from the Changed event.
                this.currentEditor.Changed -= new EventHandler( 
                    this.currentEditor_Changed);
            }
            //Clear the editor panel and add the new editor.
            this.editorPanel.Controls.Clear();
            this.editorPanel.Controls.Add((Control)editor);
           
            //Sets the current editor to match the selected item.
            this.currentEditor = editor;
            this.currentEditor.Changed += new EventHandler( 
                this.currentEditor_Changed);

            this.ValidateForm();
        }

        /// <summary>
        /// Validate the form.
        /// </summary>
        private void ValidateForm ()
        {
            this.deleteButton.Enabled = this.propListView.SelectedItems.Count > 0;
            this.saveButton.Enabled = this.nameCombo.Text.Trim() != "" && 
                this.currentEditor.Valid;
        }  

        /// <summary>
        /// Sets new editor if selected combo value has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nameCombo_SelectedValueChanged(object sender, System.EventArgs e)
        {
            IPropertyEditor selectedItem = (IPropertyEditor)this.nameCombo.SelectedItem; 

            // is the selection a special svn: keyword?
            if ( selectedItem != null)
            {
                SetNewEditor(selectedItem);

                //clear any existing selection in the list view
                this.propListView.SelectedItems.Clear();

                // is there already set a property of this type?
                //HACK: find better way
                foreach( ListViewItem item in this.propListView.Items )
                {
                    if ( item.Text == selectedItem.ToString() )
                    {                        
                        currentEditor.PropertyItem = (PropertyItem)item.Tag;
                        item.Selected = true;
                    }
                }
            }   
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
            conflictToolTip.SetToolTip( this.nameCombo, "Select or compose your own property name");
            conflictToolTip.SetToolTip( this.newButton, "Clear name and value fields");
            conflictToolTip.SetToolTip( this.saveButton, "Save property name and value");
            conflictToolTip.SetToolTip( this.deleteButton, "Delete selected property");
           conflictToolTip.SetToolTip( this.propListView, "List of defined properties");
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
      

        private IPropertyEditor currentEditor;
        private System.Windows.Forms.Panel editorPanel;

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



