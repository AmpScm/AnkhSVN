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
    }
}
