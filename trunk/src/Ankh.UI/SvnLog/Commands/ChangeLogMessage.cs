using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using SharpSvn;
using SharpSvn.Implementation;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using Ankh.VS;
using System.ComponentModel;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogChangeLogMessage, HideWhenDisabled = true)]
    class ChangeLogMessage : ICommandHandler, IComponent
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
            if (count != 1)
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            IUIService uiService = e.GetService<IUIService>();
            IAnkhSolutionSettings slnSettings = e.GetService<IAnkhSolutionSettings>();
            List<LogItem> logItems = new List<LogItem>(e.Selection.GetSelection<LogItem>());
            if (logItems.Count != 1)
                return;

            using (EditLogMessageDialog dialog = new EditLogMessageDialog())
            {
                dialog.Context = e.Context;
                dialog.LogMessage = logItems[0].RawData.LogMessage;

                if (uiService.ShowDialog(dialog) == DialogResult.OK)
                {
                    using (SvnClient client = e.GetService<ISvnClientPool>().GetClient())
                    {
                        client.SetRevisionProperty(new SvnUriTarget(slnSettings.ProjectRootUri, logItems[0].RawData.Revision), SvnPropertyNames.SvnLog, dialog.LogMessage);
                    }
                }
            }
        }

        public event EventHandler Disposed;

        ISite _site;
        public ISite Site
        {
            get
            {
                return _site;
            }
            set
            {
                _site = value;
            }
        }


        public void Dispose()
        {
        }
    }
}
