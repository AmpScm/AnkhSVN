using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ErrorReport.GUI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public class KeyBindingAttribute : Attribute
    {
        public KeyBindingAttribute( Keys keys )
        {
            this.keys = keys;
        }


        public Keys Keys
        {
            get { return keys; }
        }

        private Keys keys;

    }
}
