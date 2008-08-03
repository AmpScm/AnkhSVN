using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using System.Windows.Forms;

namespace Ankh.UI.VSSelectionControls.Commands
{
    abstract class ListViewCommandBase : ICommandHandler
    {
        public virtual void OnUpdate(CommandUpdateEventArgs e)
        {
            SmartListView list = GetListView(e);

            if (list == null)
            {
                e.Enabled = false;
                return;
            }

            OnUpdate(list, e);
        }

        public virtual void OnExecute(CommandEventArgs e)
        {
            SmartListView list = GetListView(e);

            if (list == null)
                return;

            OnExecute(list, e);
        }

        private static SmartListView GetListView(BaseCommandEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            Control c = e.Selection.ActiveDialogOrFrameControl;
            SmartListView list = null;
            ContainerControl cc;
            while (null != (cc = c as ContainerControl))
            {
                c = cc.ActiveControl;

                list = c as SmartListView;
                if (list != null)
                    break;
            }

            if (list != null)
                list = c as SmartListView;
            return list;
        }

        protected abstract void OnUpdate(SmartListView list, CommandUpdateEventArgs e);
        protected abstract void OnExecute(SmartListView list, CommandEventArgs e);
    }
}
