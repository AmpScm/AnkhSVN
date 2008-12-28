using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;
using Ankh.Scc;
using System.Windows.Forms;
using System.IO;
using SharpSvn;
using Ankh.UI;

namespace Ankh.Commands.RepositoryExplorer
{
    [Command(AnkhCommand.CopyToWorkingCopy, AlwaysAvailable = true)]
    class CopyToWorkingCopy : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            ISvnRepositoryItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnRepositoryItem>());

            if (item != null && item.Origin != null && !item.Origin.IsRepositoryRoot)
                return;

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            ISvnRepositoryItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnRepositoryItem>());

            if (item == null)
                return;

            string copyTo;
            bool copyBelow = false;
            bool suggestExport = false;
            IFileStatusCache cache = e.GetService<IFileStatusCache>();

            if (item.NodeKind == SharpSvn.SvnNodeKind.Directory)
            {
                using (FolderBrowserDialog fd = new FolderBrowserDialog())
                {
                    fd.ShowNewFolderButton = false;

                    if (DialogResult.OK != fd.ShowDialog(e.Context.DialogOwner))
                        return;

                    copyTo = fd.SelectedPath;
                    copyBelow = true;

                    SvnItem dirItem = cache[copyTo];

                    if (dirItem == null || !dirItem.IsVersioned)
                        suggestExport = true;
                }
            }
            else
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.CheckPathExists = true;
                    sfd.OverwritePrompt = true;
                    string name = item.Origin.Target.FileName;
                    string ext = Path.GetExtension(item.Origin.Target.FileName);
                    sfd.Filter = string.Format("{0} files|*.{0}|All files (*.*)|*", ext.TrimStart('.'));
                    sfd.FileName = name;

                    if (DialogResult.OK != sfd.ShowDialog(e.Context.DialogOwner))
                        return;

                    copyTo = Path.GetFullPath(sfd.FileName);

                    SvnItem fileItem = cache[copyTo];

                    if (File.Exists(copyTo))
                    {
                        // We prompted to confirm; remove the file!

                        if (fileItem.IsVersioned)
                            e.GetService<IProgressRunner>().RunModal("Copying",
                                delegate(object sender, ProgressWorkerArgs a)
                                {
                                    a.Client.Delete(copyTo);
                                });
                        else
                            File.Delete(copyTo);
                    }

                    SvnItem dir = fileItem.Parent;

                    if (dir == null || !(dir.IsVersioned && dir.IsVersionable))
                        suggestExport = true;
                }
            }

            if (!suggestExport)
            {
                e.GetService<IProgressRunner>().RunModal("Copying",
                    delegate(object sender, ProgressWorkerArgs a)
                    {
                        SvnCopyArgs ca = new SvnCopyArgs();
                        ca.CreateParents = true;
                        if (copyBelow)
                            ca.AlwaysCopyAsChild = true;

                        a.Client.Copy(item.Origin.Target, copyTo, ca);
                    });
            }
            else
            {
                AnkhMessageBox mb = new AnkhMessageBox(e.Context);

                if (DialogResult.Yes == mb.Show("The specified path is not in a workingcopy; would you like to export the file instead?",
                    "No Working Copy", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    e.GetService<IProgressRunner>().RunModal("Exporting",
                    delegate(object sender, ProgressWorkerArgs a)
                    {
                        SvnExportArgs ea = new SvnExportArgs();
                        ea.Revision = item.Revision;

                        a.Client.Export(item.Origin.Target, copyTo, ea);
                    });
                }
            }
        }
    }
}
