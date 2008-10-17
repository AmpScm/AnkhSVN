using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using Ankh.Ids;
using Ankh.UI;
using System.IO;
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Ankh.Scc;
using System.Windows.Forms;

namespace Ankh.Commands
{
    [Command(AnkhCommand.UnifiedDiff)]
    [Command(AnkhCommand.CreatePatch)]
    class ItemUnifiedDiffCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public override void OnExecute(CommandEventArgs e)
        {
            PathSelectorResult result = ShowDialog(e);
            if (!result.Succeeded)
                return;

            SvnRevisionRange revRange = new SvnRevisionRange(result.RevisionStart, result.RevisionEnd);

            IAnkhTempFileManager tempfiles = e.GetService<IAnkhTempFileManager>();
            string tempFile = tempfiles.GetTempFile(".patch");

            IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();
            string slndir = ss.ProjectRoot;
            string slndirP = slndir + "\\";

            SvnDiffArgs args = new SvnDiffArgs();
            args.IgnoreAncestry = true;
            args.NoDeleted = false;
            args.Depth = result.Depth;

            using (MemoryStream stream = new MemoryStream())
            {
                e.Context.GetService<IProgressRunner>().Run("Diffing",
                    delegate(object sender, ProgressWorkerArgs ee)
                    {
                        foreach (SvnItem item in result.Selection)
                        {
                            SvnWorkingCopy wc;
                            if (!string.IsNullOrEmpty(slndir) &&
                                item.FullPath.StartsWith(slndirP, StringComparison.OrdinalIgnoreCase))
                                args.RelativeToPath = slndir;
                            else if ((wc = item.WorkingCopy) != null)
                                args.RelativeToPath = wc.FullPath;
                            else
                                args.RelativeToPath = null;

                            ee.Client.Diff(item.FullPath, revRange, args, stream);
                        }

                        stream.Flush();
                        stream.Position = 0;
                    });
                using (StreamReader sr = new StreamReader(stream))
                {
                    switch (e.Command)
                    {
                        case AnkhCommand.UnifiedDiff:
                            File.WriteAllText(tempFile, sr.ReadToEnd(), Encoding.UTF8);
                            VsShellUtilities.OpenDocument(e.Context, tempFile);
                            break;
                        case AnkhCommand.CreatePatch:
                            using (SaveFileDialog dlg = new SaveFileDialog())
                            {
                                dlg.Filter = "Patch files(*.patch)|*.patch|Diff files(*.diff)|*.diff|" +
                                    "Text files(*.txt)|*.txt|All files(*.*)|*.*";
                                dlg.AddExtension = true;

                                if (dlg.ShowDialog(e.Context.DialogOwner) == DialogResult.OK)
                                {
                                    File.WriteAllText(dlg.FileName, sr.ReadToEnd(), Encoding.UTF8);
                                }
                            }
                            break;
                    }
                }
            }
        }
        PathSelectorResult ShowDialog(CommandEventArgs e)
        {
            PathSelectorInfo info = new PathSelectorInfo("Select items for diffing", e.Selection.GetSelectedSvnItems(true));
            IUIShell uiShell = e.GetService<IUIShell>();
            info.VisibleFilter += delegate { return true; };
            info.CheckedFilter += delegate(SvnItem item) { return item.IsFile && (item.IsModified || item.IsDocumentDirty); };

            info.RevisionStart = SvnRevision.Base;
            info.RevisionEnd = SvnRevision.Working;

            // should we show the path selector?
            if (!CommandBase.Shift)
            {
                return uiShell.ShowPathSelector(info);
            }
            else
                return info.DefaultResult;


        }
    }
}
