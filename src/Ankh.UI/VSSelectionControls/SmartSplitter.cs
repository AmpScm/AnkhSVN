using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace Ankh.UI.VSSelectionControls
{
    public class SmartSplitContainer : SplitContainer, ISupportInitialize
    {
        void ISupportInitialize.BeginInit()
        {
            // Ignored for .Net 2.0 compatibility
            if (Environment.Version.Major >= 4)
            {
                GetType().InvokeMember("BeginInit", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance, null, this, null);
            }
        }

        void ISupportInitialize.EndInit()
        {
            // Ignored for .Net 2.0 compatibility
            if (Environment.Version.Major >= 4)
            {
                GetType().InvokeMember("EndInit", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance, null, this, null);
            }
        }

        // Handle Ctrl+(Shift+)Tab and Ctrl+Return as without Ctrl
        protected override bool ProcessDialogKey(Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            if (key == Keys.Tab && (keyData & Keys.Alt) == Keys.None)
            {
                keyData = keyData & ~Keys.Control;
            }
            else if (key == Keys.Return && (keyData & Keys.Modifiers) == Keys.Control)
            {
                keyData = keyData & ~Keys.Control;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
