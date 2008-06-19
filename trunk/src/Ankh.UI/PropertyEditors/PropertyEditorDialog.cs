// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Ankh.UI
{
    /// <summary>
    /// Dialog for editing svn properties. 
    /// </summary>
    public partial class PropertyEditorDialog : System.Windows.Forms.Form
    {
        public PropertyEditorDialog(string svnItemPath)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            
			this.propItems = new List<PropertyItem>();

            this.svnItemLabel.Text = svnItemPath == null ? "" : svnItemPath;
        }

        /// <summary>
        /// Sets and gets property items.
        /// </summary>
        public PropertyItem[] PropertyItems
        {
            get
            {
                return (PropertyItem[])
                    this.propItems.ToArray();
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
        #region class ListVisitor
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
				// HACK: find out if all this .Replace is really what we want/need
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
        /// Brings up the Property Dialog in edit mode.
        /// </summary>
        private void editButton_Click(object sender, System.EventArgs e)
        {
            PropertyItem item = (PropertyItem)this.propListView.SelectedItems[0].Tag;
            int index = this.propItems.IndexOf(item);
            PropertyDialog pDialog = new PropertyDialog(item);
            if (pDialog.ShowDialog(this) == DialogResult.OK)
            {
                PropertyItem editedItem = pDialog.GetPropertyItem();
                if (editedItem != null)
                {
                    int otherIndex = -1; ;
                    if (!item.Name.Equals(editedItem.Name)
                        && ((otherIndex = TryFindItem(editedItem.Name)) > -1) 
                        && otherIndex != index)
                    {
                        // there is already a property with the same name
                        // TODO
                        // Delete selected item AND replace the existing item ???
                    }
                    else {
                        this.propItems[index] = editedItem;
                        this.PopulateListView();
                        this.UpdateButtons();
                    }
                }
            }
        }

        /// <summary>
        /// Delete the selected item if delete-button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, System.EventArgs e)
        {
            PropertyItem item = (PropertyItem)this.propListView.SelectedItems[0].Tag;
            this.propItems.Remove(item);
            this.PopulateListView();
            this.UpdateButtons();
        }

        /// <summary>
        /// Bring up Property Dialog in Add mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, System.EventArgs e)
        {
            PropertyDialog propDialog = new PropertyDialog();
            if (propDialog.ShowDialog(this) == DialogResult.OK)
            {
                PropertyItem item = propDialog.GetPropertyItem();
                if (item != null)
                {
                    int index = this.TryFindItem(item.Name);
                    if (index > -1)
                    {
                        // There is already a property with the same name.
                        // TODO ask user
                        this.propItems[index] = item;
                    }
                    else
                    {
                        this.propItems.Add(item);
                    }
                    this.PopulateListView();
                    this.UpdateButtons();
                }
            }
        }

        private int TryFindItem(string key)
        {
            int i = 0;
            foreach (PropertyItem item in this.propItems)
            {
                if (key.Equals(item.Name))
                {
                    return i;
                }
                i++;
            }
            return -1;
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
            this.UpdateButtons();
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
        }

        private void UpdateButtons()
        {
            PropertyItem selection = null;
            if (this.propListView.SelectedItems.Count > 0)
            {
                selection = (PropertyItem)this.propListView.SelectedItems[0].Tag;
            }
            this.deleteButton.Enabled = selection != null;
            this.editButton.Enabled = selection != null;
        }

        private List<PropertyItem> propItems;
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
        public BinaryPropertyItem(ICollection<byte> data)
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
        public ICollection<byte> Data
        {
            get { return this.data;}
        }
 
        private ICollection<byte> data;    
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



