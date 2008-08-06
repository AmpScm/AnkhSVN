using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.Scc.UI;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogShowLogMessage)]
    class ShowLogMessage: ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            ILogControl lc = e.Selection.ActiveFrameControl as ILogControl;

            if (lc == null)
            {
                e.Enabled = false;
                return;
            }
            
            e.Checked = lc.ShowLogMessage;
        }

        public void OnExecute(CommandEventArgs e)
        {
            ILogControl lc = e.Selection.ActiveFrameControl as ILogControl;

            if (lc == null)
                return;

            lc.ShowLogMessage = !lc.ShowLogMessage;
        }
    }
}
