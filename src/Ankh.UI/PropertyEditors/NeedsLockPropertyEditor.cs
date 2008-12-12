// $Id: NeedsLockPropertyEditor.cs 5127 2008-09-08 15:06:35Z rhuijben $
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.UI.PropertyEditors
{
    /// <summary>
    /// Property editor for executable properties.
    /// </summary>
    internal partial class NeedsLockPropertyEditor : PropertyEditControl, IPropertyEditor
    {

        public event EventHandler Changed;

        public NeedsLockPropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();
            Reset();
        }

        void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        public void Reset()
        {
            if (this.needsLockTextBox != null && !this.needsLockTextBox.IsDisposed)
            {
                this.needsLockTextBox.Text = FEEDBACK_TEXT;
            }
        }

        public bool Valid
        {

            get { return true; }
        }

        public PropertyItem PropertyItem
        {
            get
            {
                if (!this.Valid)
                {
                    throw new InvalidOperationException(
                        "Can not get a property item when valid is false");
                }

                return new TextPropertyItem(FEEDBACK_TEXT);
            }

            set { }
        }

        /// <summary>
        /// File property
        /// </summary>
        public SvnNodeKind GetAllowedNodeKind()
        {
            return SvnNodeKind.File;
        }

        public override string ToString()
        {
            return SvnPropertyNames.SvnNeedsLock;
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

        private void CreateMyToolTip()
        {
            // Set up the ToolTip text for the Button and Textbox.
            needsLockToolTip.SetToolTip(this.needsLockTextBox, FEEDBACK_TEXT);
        }

        private static readonly string FEEDBACK_TEXT = "File needs lock.";
    }
}

