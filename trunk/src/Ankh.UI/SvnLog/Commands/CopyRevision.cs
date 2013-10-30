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

            foreach (ISvnLogItem li in e.Selection.GetSelection<ISvnLogItem>())
            {
                if (sb.Length > 0)
                    sb.Append(",");

                sb.Append(li.Revision);
            }

            Clipboard.SetText(sb.ToString(), TextDataFormat.UnicodeText);
        }
    }
}
