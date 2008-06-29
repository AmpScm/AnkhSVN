using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Ids;
using Ankh.Scc;
using Ankh.Selection;
using Ankh.UI;
using System.Windows.Forms;
using System.IO;
using SharpSvn;
using Ankh.VS;
using Ankh.UI.SccManagement;
using System.Collections.ObjectModel;

namespace Ankh.Commands
{
    [Command(AnkhCommand.FileSccAddProjectToSubversion, HideWhenDisabled = true)]
    [Command(AnkhCommand.FileSccAddSolutionToSubversion, AlwaysAvailable=true, HideWhenDisabled=true)]
    sealed class AddToSccCommands : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SolutionExists || (e.Command == AnkhCommand.FileSccAddProjectToSubversion && e.State.EmptySolution))
            {
                e.Enabled = false;
                return;
            }

            if (e.State.OtherSccProviderActive)
            {
                e.Enabled = false;
                return; // Only one scc provider can be active at a time
            }

            IAnkhSccService scc = e.GetService<IAnkhSccService>();
            if (scc == null)
            {
                e.Enabled = false;
                return;
            }


            if (!scc.IsSolutionManaged)
                return; // Nothing is added unless the solution is added

            if (e.Command == AnkhCommand.FileSccAddSolutionToSubversion)
            {
                e.Enabled = false;
                return;
            }

            foreach (SvnProject p in GetSelection(e.Selection))
            {
                if (!scc.IsProjectManaged(p))
                    return; // Something to enable
            }

            e.Enabled = false;
        }

        private IEnumerable<SvnProject> GetSelection(ISelectionContext iSelectionContext)
        {
            bool foundOne = false;
            foreach (SvnProject pr in iSelectionContext.GetSelectedProjects(true))
            {
                yield return pr;
                foundOne = true;
            }

            if (foundOne)
                yield break;

            foreach (SvnProject pr in iSelectionContext.GetOwnerProjects(false))
            {
                yield return pr;
            }
        }

        static bool IsVersionable(SvnItem item)
        {
            // HACK: remove when IsVersionable behavior is fixed
            item.MarkDirty();
            return item.IsVersionable;
        }

        static Uri Canonicalize(Uri uri)
        {
            String path = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
            if (path.Length > 0 && (path[path.Length - 1] == '/' || path.IndexOf('\\') >= 0))
            {
                // Create a new uri with all / and \ characters at the end removed
                return new Uri(uri, path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            }

            return uri;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IAnkhSccService scc = e.GetService<IAnkhSccService>();
            IFileStatusCache cache = e.GetService<IFileStatusCache>();
            if (scc == null || cache == null || e.Selection.SolutionFilename == null)
                return;

            bool shouldActivate = false;
            if (!scc.IsActive)
            {
                if (e.State.OtherSccProviderActive)
                    return; // Can't switch in this case.. Nothing to do

                shouldActivate = true;
            }

            AnkhMessageBox mb = new AnkhMessageBox(e.Context);

            if (!scc.IsSolutionManaged)
            {
                bool confirmed = false;
                SvnItem item = cache[e.Selection.SolutionFilename];

                IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();

                if (item.IsVersioned)
                { /* File is in subversion; just enable */ }
                else if (IsVersionable(item))
                {
                    if (e.IsInAutomation)
                        confirmed = true;
                    else if (DialogResult.Yes != mb.Show(string.Format(CommandResources.AddSolutionXToSubversion,
                        Path.GetFileName(e.Selection.SolutionFilename)), AnkhId.PlkProduct, MessageBoxButtons.YesNo))
                    {
                        return;
                    }
                    else
                        confirmed = true;

                    using (SvnClient cl = e.GetService<ISvnClientPool>().GetNoUIClient())
                    {
                        SvnAddArgs aa = new SvnAddArgs();
                        aa.AddParents = true;
                        cl.Add(e.Selection.SolutionFilename, aa);

                        //settings.ProjectRoot = Path.GetFullPath(dialog.WorkingCopyDir);
                    }
                }
                else
                {
                    using (SvnClient cl = e.GetService<ISvnClientPool>().GetClient())
                    using (Ankh.UI.SccManagement.AddToSubversion dialog = new Ankh.UI.SccManagement.AddToSubversion())
                    {
                        dialog.PathToAdd = e.Selection.SolutionFilename;
                        if (dialog.ShowDialog(e.Context) == DialogResult.OK)
                        {
                            confirmed = true;
                            Collection<SvnInfoEventArgs> info;
                            SvnInfoArgs ia = new SvnInfoArgs();
                            ia.ThrowOnError = false;
                            if (!cl.GetInfo(new SvnUriTarget(dialog.RepositoryAddUrl), ia, out info))
                            {
                                using (CreateDirectory createDialog = new CreateDirectory())
                                {
                                    createDialog.NewDirectoryName = dialog.RepositoryAddUrl.ToString();
                                    createDialog.NewDirectoryReadonly = true;
                                    if (createDialog.ShowDialog(e.Context) == DialogResult.OK)
                                    {
                                        // Create uri (including optional /trunk if required)
                                        SvnCreateDirectoryArgs cdArg = new SvnCreateDirectoryArgs();
                                        cdArg.MakeParents = true;
                                        cdArg.LogMessage = createDialog.LogMessage;

                                        cl.RemoteCreateDirectory(Canonicalize(dialog.RepositoryAddUrl), cdArg);
                                    }
                                    else
                                        return; // bail out, we cannot continue without directory in the repository
                                }
                            }

                            // Create working copy
                            SvnCheckOutArgs coArg = new SvnCheckOutArgs();
                            coArg.AllowObstructions = true;
                            cl.CheckOut(Canonicalize(dialog.RepositoryAddUrl), dialog.WorkingCopyDir, coArg);

                            // Add solutionfile so we can set properties (set managed)
                            SvnAddArgs aa = new SvnAddArgs();
                            aa.AddParents = true;
                            cl.Add(e.Selection.SolutionFilename, aa);

                            settings.ProjectRoot = Path.GetFullPath(dialog.WorkingCopyDir);
                        }
                        else
                        {
                            return; // User cancelled the "Add to subversion" dialog, don't set as managed by Ankh or anything else
                        }
                    }
                }

                if (!confirmed && !e.IsInAutomation &&
                    DialogResult.Yes != mb.Show(string.Format(CommandResources.MarkXAsManaged,
                    Path.GetFileName(e.Selection.SolutionFilename)), AnkhId.PlkProduct, MessageBoxButtons.YesNo))
                {
                    return;
                }

                if (shouldActivate)
                    scc.RegisterAsPrimarySccProvider();

                scc.SetProjectManaged(null, true);
                item.MarkDirty(); // This clears the solution settings cache to retrieve its properties
            }

            if (e.Command == AnkhCommand.FileSccAddSolutionToSubversion)
                return;

            IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();

            if (mapper != null)
            {
                if (!e.IsInAutomation)
                {
                    StringBuilder sb = new StringBuilder();
                    bool foundOne = false;
                    foreach (SvnProject project in GetSelection(e.Selection))
                    {
                        ISvnProjectInfo info;
                        if (!scc.IsProjectManaged(project) && null != (info = mapper.GetProjectInfo(project)))
                        {
                            if (sb.Length > 0)
                                sb.Append("', '");

                            sb.Append(info.ProjectName);
                        }

                        foundOne = true;
                    }
                    string txt = sb.ToString();
                    int li = txt.LastIndexOf("', '");
                    if (li > 0)
                        txt = txt.Substring(0, li + 1) + CommandResources.FileAnd + txt.Substring(li + 3);

                    if (foundOne && DialogResult.Yes != mb.Show(string.Format(CommandResources.MarkXAsManaged,
                        txt), AnkhId.PlkProduct, MessageBoxButtons.YesNo))
                    {
                        return;
                    }
                }

                foreach (SvnProject project in GetSelection(e.Selection))
                {
                    if (!scc.IsProjectManaged(project))
                    {
                        scc.SetProjectManaged(project, true);
                    }
                }
            }
        }
    }
}
