// $Id$
using System;
using System.IO;
using System.Windows.Forms;
using Ankh.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you create a patch.
    /// </summary>
    [Command(AnkhCommand.CreatePatch)]
    class CreatePatchCommand : LocalDiffCommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
        }
        

        public override void OnExecute(CommandEventArgs e)
        {
            string diff = this.GetDiff(
                e.Context,
                e.Selection,
                null,
                true,
                delegate(SvnItem item)
                {
                    return item.IsModified;
                });

            if (diff == null)
            {
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Patch files(*.patch)|*.patch|Diff files(*.diff)|*.diff|" +
                    "Text files(*.txt)|*.txt|All files(*.*)|*.*";
                dlg.AddExtension = true;

                if (dlg.ShowDialog(e.Context.DialogOwner) == DialogResult.OK)
                {
                    using (StreamWriter w = File.CreateText(dlg.FileName))
                        w.Write(diff);
                }
            }
        }
    }
}
