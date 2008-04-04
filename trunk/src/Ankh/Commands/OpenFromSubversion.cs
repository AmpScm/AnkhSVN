using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.UI.RepositoryOpen;
using System.Windows.Forms;
using Ankh.VS;

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

                if (dlg.ShowDialog(e.Context.DialogOwner) == DialogResult.OK)
                {
                }
            }
        }
    }
}
