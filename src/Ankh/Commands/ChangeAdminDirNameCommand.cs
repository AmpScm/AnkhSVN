using System;
using Ankh.UI;
using NSvn.Core;
using System.Windows.Forms;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that allows you to temporarily change the name of 
    /// Subversion's admin directory.
    /// </summary>
    [VSNetCommand("ChangeAdminDirName", 
         Text="Temporarily change the Subversion admin directory...", 
         Tooltip= "Temporarily change the name of the Subversion administrative directory", 
         Bitmap=ResourceBitmaps.ChangeAdminDirName ),
    VSNetControl( "MenuBar.Tools.AnkhSVN", Position=1 ),]
	internal class ChangeAdminDirNameCommand : CommandBase
	{
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            // we don't want to allow this while a solution is already open
            // it could lead to weird behavior
            return context.AnkhLoadedForSolution ? Disabled : Enabled;
        }

        public override void Execute(AnkhContext context, string parameters)
        {
            using( AdminDirDialog dlg = new AdminDirDialog() )
            {
                dlg.AdminDirName = Client.AdminDirectoryName;
                if ( dlg.ShowDialog( context.HostWindow ) != DialogResult.OK )
                    return;
                Client.AdminDirectoryName = dlg.AdminDirName;
            }
        }		
	}
}
