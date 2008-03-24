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
    public partial class PropertyEditorDialog : System.Windows.Forms.Form
    {
        public PropertyEditorDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            
            CreateMyToolTip();

            SetNewEditor(new PlainPropertyEditor());
			
            this.propItems = new ArrayList();

            this.nameCombo.Items.Add(new ExecutablePropertyEditor());
            this.nameCombo.Items.Add(new MimeTypePropertyEditor());
            this.nameCombo.Items.Add(new IgnorePropertyEditor());
            this.nameCombo.Items.Add(new KeywordsPropertyEditor());
            this.nameCombo.Items.Add(new EolStylePropertyEditor());
            this.nameCombo.Items.Add(new ExternalsPropertyEditor()); 
           
            //Set default value in list to first item in nameCombo 
            this.nameCombo.SelectedIndex = 0;        
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
            ToolTip ToolTip = new ToolTip(this.components);

            // Set up the delays in milliseconds for the ToolTip.
            ToolTip.AutoPopDelay = 5000;
            ToolTip.InitialDelay = 1000;
            ToolTip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            ToolTip.ShowAlways = true;
         
            // Set up the ToolTip text for the Button and Checkbox.
            ToolTip.SetToolTip( this.nameCombo, "Select or compose your own property name");
            ToolTip.SetToolTip( this.newButton, "Clear name and value fields");
            ToolTip.SetToolTip( this.saveButton, "Save property name and value");
            ToolTip.SetToolTip( this.deleteButton, "Delete selected property");
            ToolTip.SetToolTip( this.propListView, "List of defined properties");
        }

  
    


        private ArrayList propItems;

        
    
        private IPropertyEditor currentEditor;        
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



