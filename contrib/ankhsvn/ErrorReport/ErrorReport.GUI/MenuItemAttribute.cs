using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReport.GUI
{
    [AttributeUsage(AttributeTargets.Class)]
    class MenuItemAttribute : Attribute
    {
        public MenuItemAttribute( string path )
        {
            this.path = path;
        }

        public string Path
        {
            get { return path; }
        }

        private string path;
    }
}
