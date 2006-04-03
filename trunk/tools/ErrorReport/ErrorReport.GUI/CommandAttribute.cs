using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReport.GUI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute( string text )
        {
            this.text = text;
        }

        public string Text
        {
            get { return text; }
        }

        public string Tooltip
        {
            get { return tooltip; }
            set { tooltip = value; }
        }

        private string tooltip = string.Empty;
        private string text;
    }
}
