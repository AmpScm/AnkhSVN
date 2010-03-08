using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Ankh.Commands;

namespace Ankh.Scc.SccUI.Commands
{
    [Command(AnkhCommand.MakeNonSccFileWriteable, AlwaysAvailable=true)]
    class MakeNonSccFileWritableCommand : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public void OnExecute(CommandEventArgs e)
        {
            SvnItem item = e.Argument as SvnItem;
            if (item == null)
                return;

            using(EditReadOnlyFileDialog dialog = new EditReadOnlyFileDialog(item))
            {
                switch(dialog.ShowDialog(e.Context))
                {
                    case DialogResult.Yes:
                        // make writable and allow
                        FileAttributes attr = File.GetAttributes(item.FullPath);
                        File.SetAttributes(item.FullPath, attr & ~FileAttributes.ReadOnly);
                        e.Result = true;
                        break;
                    case DialogResult.No:
                        // Don't make writable but allow
                        e.Result = true;
                        break;
                    default:
                        // Don't make writeable and don't allow
                        e.Result = false;
                        break;
                }
            }
        }

        #region ICommandHandler Members

        #endregion
    }
}
