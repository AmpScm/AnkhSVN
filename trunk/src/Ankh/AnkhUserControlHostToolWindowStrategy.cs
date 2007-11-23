using System;

using System.Text;
using System.Windows.Forms;
using EnvDTE;

namespace Ankh
{
    class AnkhUserControlHostToolWindowStrategy : IToolWindowStrategy
    {
        public AnkhUserControlHostToolWindowStrategy( IContext context )
        {
            this.context = context;
        }

        public ToolWindowResult CreateToolWindow( Type controlType, string caption, string guid )
        {
            object ctl = null;
            Window window = this.context.DTE.Windows.CreateToolWindow(
                this.context.AddIn, ProgId, caption, guid, ref ctl );

            AnkhUserControlHostLib.IAnkhUserControlHostCtlCtl
                hoster = (AnkhUserControlHostLib.IAnkhUserControlHostCtlCtl)ctl;

            object control = Activator.CreateInstance( controlType );

            hoster.HostUserControl( control );

            return new ToolWindowResult( (Control)control, window );
        }

        private IContext context;

        private const string ProgId = "AnkhUserControlHost.AnkhUserControlHostCtl";
    }
}
