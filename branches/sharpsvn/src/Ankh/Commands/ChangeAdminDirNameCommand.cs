using System;
using Ankh.UI;
using NSvn.Core;
using System.Windows.Forms;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to temporarily change the Subversion administrative folder.
    /// </summary>
    [VSNetCommand("ChangeAdminDirName", 
         Text = "Tempor&arily Change Subversion Admin Folder...", 
         Tooltip = "Temporarily change the Subversion administrative folder.", 
         Bitmap = ResourceBitmaps.ChangeAdminDirName ),
         VSNetControl( "Tools.AnkhSVN", Position = 8 )]
    public class ChangeAdminDirNameCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            // we don't want to allow this while a solution is already open
            // it could lead to weird behavior
            return context.AnkhLoadedForSolution ? Disabled : Enabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            using( AdminDirDialog dlg = new AdminDirDialog() )
            {
                dlg.AdminDirName = Client.AdminDirectoryName;
                if ( dlg.ShowDialog( context.HostWindow ) != DialogResult.OK )
                    return;

                Client.AdminDirectoryName = dlg.AdminDirName;
            }
        }

        #endregion
    }
}