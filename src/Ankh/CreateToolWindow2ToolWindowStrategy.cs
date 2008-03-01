using System;
using System.Text;
using EnvDTE;
using System.Reflection;
using System.Windows.Forms;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Ankh
{
    class CreateToolWindow2ToolWindowStrategy : IToolWindowStrategy
    {
        public CreateToolWindow2ToolWindowStrategy( IContext context )
        {
            this.context = context;
        }


        public ToolWindowResult CreateToolWindow( Type controlType, string caption, string guid )
        {
            Windows2 windows2 = this.context.DTE.Windows as Windows2;

            string assembly = controlType.Assembly.GetModules(false)[0].FullyQualifiedName;
            string typeName = controlType.FullName;

            object control = null;
            Window window = windows2.CreateToolWindow2( null, assembly, typeName,
                caption, guid, ref control );

            if ( window == null )
            {
                throw new InvalidOperationException( "Unable to create toolwindow: " + typeName );
            }

            return new ToolWindowResult( (Control)control, window );
        }

        private IContext context;
    }
}
