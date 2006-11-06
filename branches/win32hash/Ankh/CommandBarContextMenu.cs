using System;
using System.Text;
using System.Reflection;

namespace Ankh
{
    public class CommandBarContextMenu : Ankh.UI.IContextMenu
    {
        public CommandBarContextMenu( object  commandBar )
        {
            this.commandBar = commandBar;
        }
        #region IContextMenu Members

        public void Show( int screenX, int screenY )
        {
            this.commandBar.GetType().InvokeMember(
            "ShowPopup", BindingFlags.InvokeMethod, null,
            this.commandBar, new object[] { screenX, screenY } );
        }

        #endregion

        public object CommandBar
        {
            get { return this.commandBar; }
            set { this.commandBar = value; }
        }


        private object commandBar;
    }
}
