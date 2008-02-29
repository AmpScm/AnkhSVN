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

		[Obsolete("BH: Does not work for now; should move to package api")]
        public ToolWindowResult CreateToolWindow( Type controlType, string caption, string guid )
        {
#if REQUIRES_WORK
            object ctl = null;
            Window window = this.context.DTE.Windows.CreateToolWindow(
                this.context.AddIn, ProgId, caption, guid, ref ctl );

            AnkhUserControlHostLib.IAnkhUserControlHostCtlCtl
                hoster = (AnkhUserControlHostLib.IAnkhUserControlHostCtlCtl)ctl;

            object control = Activator.CreateInstance( controlType );

            hoster.HostUserControl( control );

            return new ToolWindowResult( (Control)control, window );
#endif
			return null;
        }

        private IContext context;

        private const string ProgId = "AnkhUserControlHost.AnkhUserControlHostCtl";
    }
}
