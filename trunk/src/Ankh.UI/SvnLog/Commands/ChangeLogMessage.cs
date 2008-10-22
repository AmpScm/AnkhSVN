using System;
using System.Collections.Generic;
using System.Windows.Forms;

using SharpSvn;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.Scc;
using Ankh.VS;
using Ankh.Scc.UI;


namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogChangeLogMessage, HideWhenDisabled = true)]
    class ChangeLogMessage : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            int count = 0;
			foreach (ISvnLogItem i in e.Selection.GetSelection<ISvnLogItem>())
            {
                count++;

                if (count > 1)
                    break;
            }
            if (count != 1)
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            ILogControl logWindow = (ILogControl)e.Selection.ActiveDialogOrFrameControl;
            IAnkhSolutionSettings slnSettings = e.GetService<IAnkhSolutionSettings>();
			List<ISvnLogItem> logItems = new List<ISvnLogItem>(e.Selection.GetSelection<ISvnLogItem>());
            if (logItems.Count != 1)
                return;

            using (EditLogMessageDialog dialog = new EditLogMessageDialog())
            {
                dialog.Context = e.Context;
                dialog.LogMessage = logItems[0].LogMessage;

                if (dialog.ShowDialog(e.Context) == DialogResult.OK)
                {
					if (dialog.LogMessage == logItems[0].LogMessage)
						return; // No changes

                    using (SvnClient client = e.GetService<ISvnClientPool>().GetClient())
                    {
                        client.SetRevisionProperty(new SvnUriTarget(logItems[0].RepositoryRoot, logItems[0].Revision), SvnPropertyNames.SvnLog, dialog.LogMessage);
                    }
                    
                    logWindow.Restart();
                    // TODO: Somehow repair scroll position/number of items loaded
                }
            }
        }   
    }
}
