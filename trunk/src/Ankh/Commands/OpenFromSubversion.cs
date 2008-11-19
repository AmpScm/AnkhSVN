using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.UI.RepositoryOpen;
using System.Windows.Forms;
using Ankh.VS;
using SharpSvn;
using System.IO;
using System.Windows.Forms.Design;
using Ankh.Scc;
using System.Diagnostics;

namespace Ankh.Commands
{
    [Command(AnkhCommand.FileFileOpenFromSubversion, AlwaysAvailable=true, ArgumentDefinition="u")]
    [Command(AnkhCommand.FileFileAddFromSubversion, AlwaysAvailable=true, ArgumentDefinition="u")]
    [Command(AnkhCommand.FileSccOpenFromSubversion)]
    [Command(AnkhCommand.FileSccAddFromSubversion)]
    class OpenFromSubversion : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Command == AnkhCommand.FileFileAddFromSubversion || e.Command == AnkhCommand.FileSccAddFromSubversion)
            {
                if(!e.State.SolutionExists || e.State.SolutionBuilding || e.State.Debugging)
                    e.Enabled = e.Visible = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IUIService ui = e.GetService<IUIService>();
            Uri selectedUri = null;
            Uri rootUri;

            if (e.Argument is string && Uri.TryCreate((string)e.Argument, UriKind.Absolute, out selectedUri))
            { }
            else if (e.Argument is Uri)
                selectedUri = (Uri)e.Argument;


            IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();

            if (e.PromptUser || selectedUri == null)
            {
                using (RepositoryOpenDialog dlg = new RepositoryOpenDialog())
                {                    
                    dlg.Context = e.Context;
                    dlg.Filter = settings.OpenProjectFilterName + "|" + settings.AllProjectExtensionsFilter + "|All Files (*.*)|*";

                    if (selectedUri != null)
                        dlg.SelectedUri = selectedUri;

                    if (e.Command != AnkhCommand.FileFileOpenFromSubversion && e.Command != AnkhCommand.FileSccOpenFromSubversion)
                    {
                        foreach (string ext in settings.SolutionFilter.Split(';'))
                        {
                            dlg.Filter = dlg.Filter.Replace(ext.Trim() + ';', "");
                        }
                    }

                    DialogResult dr;

                    if (ui != null)
                        dr = ui.ShowDialog(dlg);
                    else
                        dr = dlg.ShowDialog(e.Context.DialogOwner);

                    if (dr != DialogResult.OK)
                        return;

                    selectedUri = dlg.SelectedUri;
                    rootUri = dlg.SelectedRepositoryRoot;
                }
            }
            else
            {
                using (SvnClient client = e.GetService<ISvnClientPool>().GetClient())
                {
                    rootUri = client.GetRepositoryRoot(selectedUri);
                }
            }

            string path = settings.NewProjectLocation;

            string name = Path.GetFileNameWithoutExtension(SvnTools.GetFileName(selectedUri));

            string newPath;
            int n = 0;
            do
            {
                newPath = Path.Combine(path, name);
                if (n > 0)
                    newPath += string.Format("({0})", n);
                n++;
            }
            while (File.Exists(newPath) || Directory.Exists(newPath));

            using (CheckoutProject dlg = new CheckoutProject())
            {
                dlg.Context = e.Context;
                dlg.ProjectUri = selectedUri;
                dlg.RepositoryRootUri = rootUri;
                dlg.SelectedPath = newPath;
                dlg.SvnOrigin = new SvnOrigin(selectedUri, rootUri);
                dlg.HandleCreated += delegate
                {
                    FindRoot(e.Context, selectedUri, dlg);
                };

                DialogResult dr;

                if (ui != null)
                    dr = ui.ShowDialog(dlg);
                else
                    dr = dlg.ShowDialog(e.Context.DialogOwner);

                if (dr != DialogResult.OK)
                    return;

                IVsSolution2 sol = e.GetService<IVsSolution2>(typeof(SVsSolution));

                if (sol != null)
                {
                    sol.CloseSolutionElement(VSConstants.VSITEMID_ROOT, null, 0); // Closes the current solution

                    sol = null;
                }

                IAnkhSccService scc = e.GetService<IAnkhSccService>();

                if (scc != null)
                    scc.RegisterAsPrimarySccProvider(); // Make us the current SCC provider!

                CheckOutAndOpenSolution(e, dlg.ProjectTop, null, dlg.ProjectTop, dlg.SelectedPath, dlg.ProjectUri);

                sol = e.GetService<IVsSolution2>(typeof(SVsSolution));

                if (sol != null)
                {
                    string file, user, dir;

                    if (ErrorHandler.Succeeded(sol.GetSolutionInfo(out dir, out file, out user))
                        && !string.IsNullOrEmpty(file))
                    {
                        scc.SetProjectManaged(null, true);
                    }
                }
            }
        }

        private void CheckOutAndOpenSolution(CommandEventArgs e, SvnUriTarget checkoutLocation, SvnRevision revision, Uri projectTop, string localDir, Uri projectUri)
        {
            IProgressRunner runner = e.GetService<IProgressRunner>();

            runner.Run("Checking Out Solution", delegate(object sender, ProgressWorkerArgs ee) { PerformCheckout(ee, checkoutLocation, revision, localDir); });

            Uri file = projectTop.MakeRelativeUri(projectUri);

            string projectFile = SvnTools.GetNormalizedFullPath(Path.Combine(localDir, SvnTools.UriPartToPath(file.ToString())));

            OpenProject(e, projectFile);
        }

        private void OpenProject(CommandEventArgs e, string projectFile)
        {
            IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();
            IVsSolution solution = e.GetService<IVsSolution>(typeof(SVsSolution));

            if (!File.Exists(projectFile))
                return;

            string ext = Path.GetExtension(projectFile);
            bool isSolution = false;
            foreach (string x in ss.SolutionFilter.Split(';'))
            {
                if (string.Equals(ext, Path.GetExtension(x).Replace('*', '!').Replace('?', '!'), StringComparison.OrdinalIgnoreCase))
                {
                    isSolution = true;
                    break;
                }
            }

            if (isSolution)
                ErrorHandler.ThrowOnFailure(solution.OpenSolutionFile(0, projectFile));
            else
            {
                Guid gnull = Guid.Empty;
                Guid gInterface = Guid.Empty;
                IntPtr pProj = IntPtr.Zero;

                ErrorHandler.ThrowOnFailure(solution.CreateProject(ref gnull, projectFile, null, null, (uint)__VSCREATEPROJFLAGS.CPF_OPENFILE, ref gInterface, out pProj));

                Debug.Assert(pProj == IntPtr.Zero); // no pProj as gInterface = Guid.Empty
            }
        }

        private void PerformCheckout(ProgressWorkerArgs e, SvnUriTarget projectTop, SvnRevision revision, string localDir)
        {
            SvnCheckOutArgs a = new SvnCheckOutArgs();
            a.Revision = revision;

            e.Client.CheckOut(projectTop, localDir, a);            
        }

        delegate void DoSomething();

        private void FindRoot(IAnkhServiceProvider context, Uri selectedUri, CheckoutProject dlg)
        {
            DoSomething ds = delegate
            {
                using (SvnClient client = context.GetService<ISvnClientPool>().GetClient())
                {
                    string value;
                    if (client.TryGetProperty(selectedUri, "vs:project-root", out value))
                    {
                        if (dlg.IsHandleCreated)
                            dlg.Invoke((DoSomething)delegate
                            {
                                try
                                {
                                    dlg.ProjectTop = new Uri(selectedUri, value);
                                }
                                catch { };
                            });
                    }
                }
            };

            ds.BeginInvoke(null, null);
        }
    }
}
