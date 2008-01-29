using System;

using System.Text;
using EnvDTE;
using System.Windows.Forms;

namespace Ankh
{
    /// <summary>
    /// A strategy for creating tool windows.
    /// </summary>
    public interface IToolWindowStrategy
    {
        ToolWindowResult CreateToolWindow( Type controlType, string caption, string guid );
    }

    public class ToolWindowResult
    {
        public ToolWindowResult( Control control, Window window )
        {
            this.control = control;
            this.window = window;
        }

        public Control Control
        {
            get { return control; }
        }

        public Window Window
        {
            get { return window; }
        }

        private Window window;
        private Control control;
        
	
    }
}
