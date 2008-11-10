// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using SharpSvn;
using System.Text;

namespace Ankh.UI.PropertyEditors
{
    /// <summary>
    /// Property editor for keywords.
    /// </summary>
    partial class KeywordsPropertyEditor : PropertyEditControl, IPropertyEditor
    {
        public event EventHandler Changed;

        public KeywordsPropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new System.ComponentModel.Container();
        }

        /// <summary>
        /// Resets the checkboxes.
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++ )
                checkedListBox1.SetItemChecked(i, false);
            this.dirty = false;
        }

        /// <summary>
        /// Indicates whether the selection is valid.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),Browsable(false)]
        public bool Valid
        {
            get
            {
                if (!this.dirty)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        /// <summary>
        /// Sets and gets the property item.
        /// </summary>
        public PropertyItem PropertyItem
        {
            get
            {
                string selectedText = "";

                if (!this.Valid)
                {
                    throw new InvalidOperationException(
                        "Can not get a property item when Valid is false");
                }

                StringBuilder sb = new StringBuilder();

                for(int i = 0; i <checkedListBox1.Items.Count; i++)
                {
                    if(checkedListBox1.GetItemChecked(i))
                    {
                        if(sb.Length > 0)
                            sb.Append(" ");
                        sb.Append(checkedListBox1.Items[i].ToString());
                    }
                }

                selectedText = sb.ToString();

                return new TextPropertyItem(selectedText);
            }

            set
            {
                TextPropertyItem item = (TextPropertyItem)value;

                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    checkedListBox1.SetItemChecked(i, false);

                string text = item.Text;
                if (text != null)
                {
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (char.IsWhiteSpace(text, i) && text[i] != ' ')
                            text = text.Replace(text[i], ' '); // Replace all possible whitespace to spaces
                    }                    

                    foreach(string s in text.Split(' '))
                    {
                        if(!string.IsNullOrEmpty(s))
                        {
                            int n = checkedListBox1.Items.IndexOf(s);

                            if(n >= 0)
                                checkedListBox1.SetItemChecked(n, true);
                        }
                    }
                }
                this.dirty = false;
            }
        }

        /// <summary>
        /// The type of property for this editor.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SvnPropertyNames.SvnKeywords;
        }

        /// <summary>
        /// File property
        /// </summary>
        public SvnNodeKind GetAllowedNodeKind()
        {
            return SvnNodeKind.File;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// Dispatches the Changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.dirty = true;
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Flag for enabling/disabling save button
        /// </summary>
        private bool dirty;       
    }
}

