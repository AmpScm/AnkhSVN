﻿using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.UI.RepositoryOpen;
using System.Windows.Forms;
using Ankh.VS;
using SharpSvn;
using System.IO;

namespace Ankh.Commands
{
    [Command(AnkhCommand.FileFileOpenFromSubversion)]
    [Command(AnkhCommand.FileFileAddFromSubversion)]
    [Command(AnkhCommand.FileSccOpenFromSubversion)]
    [Command(AnkhCommand.FileSccAddFromSubversion)]
    class OpenFromSubversion : CommandBase
    {
        readonly static Guid UICONTEXT_SolutionExistsAndNotBuildingAndNotDebugging = new Guid("d0e4deec1b534cda8559d454583ad23b");
        uint _cookie;
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Command == AnkhCommand.FileFileAddFromSubversion || e.Command == AnkhCommand.FileSccAddFromSubversion)
            {
                e.Enabled = e.Visible = false;

                // We match the behaviour of the command in unloaded status
                IVsMonitorSelection sel = e.GetService<IVsMonitorSelection>();

                if (sel != null)
                {
                    if (_cookie == 0)
                    {
                        Guid g = UICONTEXT_SolutionExistsAndNotBuildingAndNotDebugging;
                        uint cookie;
                        if (ErrorHandler.Succeeded(sel.GetCmdUIContextCookie(ref g, out cookie)))
                            _cookie = cookie;
                    }

                    int active;
                    if (_cookie != 0 && ErrorHandler.Succeeded(sel.IsCmdUIContextActive(_cookie, out active)))
                        e.Enabled = e.Visible = (active != 0);
                }
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            Uri selectedUri;
            Uri rootUri;
            IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();
            using (RepositoryOpenDialog dlg = new RepositoryOpenDialog())
            {
                dlg.Context = e.Context;
                string filters = settings.AllProjectExtensionsFilter;

                if (e.Command == AnkhCommand.FileFileOpenFromSubversion || e.Command == AnkhCommand.FileSccOpenFromSubversion)
                {
                    filters = "*.sln;*.dsw;" + filters;

                    dlg.Filter = "All Projects and Solutions (" + filters + ")|" + filters + "|All Files (*.*)|*";
                }
                else
                {
                    dlg.Filter = "All Projects  (" + filters + ")|" + filters + "|All Files (*.*)|*";
                }

                if (dlg.ShowDialog(e.Context.DialogOwner) != DialogResult.OK)
                    return;

                selectedUri = dlg.SelectedUri;
                rootUri = dlg.SelectedRepositoryRoot;
            }

            string path = settings.NewProjectLocation;

            string name = Path.GetFileNameWithoutExtension(selectedUri.PathAndQuery.Split('?')[0].Trim('/'));

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
                dlg.HandleCreated += delegate
                {
                    FindRoot(e.Context, selectedUri, dlg);
                };

                if (dlg.ShowDialog(e.Context.DialogOwner) != DialogResult.OK)
                    return;

                CheckOutAndOpenSolution(e, dlg.ProjectTop, dlg.ProjectTop, dlg.SelectedPath, dlg.ProjectUri);
            }
        }

        private void CheckOutAndOpenSolution(CommandEventArgs e, SvnUriTarget checkoutLocation, Uri projectTop, string localDir, Uri projectUri)
        {
            IProgressRunner runner = e.GetService<IProgressRunner>();

            runner.Run("Checking Out Solution", delegate(object sender, ProgressWorkerArgs ee) { PerformCheckout(ee, checkoutLocation, localDir); });

            Uri file = projectTop.MakeRelativeUri(projectUri);

            string projectFile = Path.Combine(localDir, file.ToString().Replace('/', Path.DirectorySeparatorChar));

            OpenProject(e, projectFile);
        }

        private void OpenProject(CommandEventArgs e, string projectFile)
        {
            IVsSolution solution = e.GetService<IVsSolution>(typeof(SVsSolution));

            if (File.Exists(projectFile))
            {
                solution.OpenSolutionFile(0, projectFile);
            }
        }

        private void PerformCheckout(ProgressWorkerArgs e, SvnUriTarget projectTop, string localDir)
        {
            SvnCheckOutArgs a = new SvnCheckOutArgs();

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
