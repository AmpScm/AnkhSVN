using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using SharpSvn;
using SharpSvn.Implementation;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogChangeLogMessage)]
    class ChangeLogMessage : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            int count = 0;
            foreach (LogItem i in e.Selection.GetSelection<LogItem>())
            {
                count++;

                if (count > 1)
                    break;
            }
            if(count != 1)
                e.Enabled = e.Visible = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            string newValue = null;
            using (SvnClient client = e.GetService<ISvnClientPool>().GetClient())
            {
                SvnSetRevisionPropertyArgs args = new SvnSetRevisionPropertyArgs();
                client.SetRevisionProperty(null, SvnPropertyNames.SvnLog, args, newValue);
            }
        }
    }
}
