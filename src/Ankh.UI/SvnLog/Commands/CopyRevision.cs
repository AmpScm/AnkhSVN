using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.Commands;
using Ankh.Scc;

namespace Ankh.UI.SvnLog.Commands
{
    [SvnCommand(AnkhCommand.CopyRevisionNumber, AlwaysAvailable=true)]
    sealed class CopyRevision : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (EnumTools.IsEmpty(e.Selection.GetSelection<ISvnLogItem>()))
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            List<long> revs = new List<long>();

            foreach (ISvnLogItem li in e.Selection.GetSelection<ISvnLogItem>())
            {
                revs.Add(li.Revision);
            }

            revs.Sort();

            foreach(long r in revs)
            {
                if (sb.Length > 0)
                    sb.Append(",");

                sb.Append(r);
            }

            Clipboard.SetText(sb.ToString(), TextDataFormat.Text);
        }
    }
}
