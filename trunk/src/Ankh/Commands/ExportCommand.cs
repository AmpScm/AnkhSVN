// $Id: ExportCommand.cs 1410 2004-06-24 01:45:04Z Arild $
using System;
using Ankh.UI;
using System.Windows.Forms;
using Utils;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to export a Subversion repository or local folder.
    /// </summary>
    [VSNetCommand("Export",
         Text = "E&xport a Repository or Local Folder...", 
         Tooltip = "Export a Subversion repository or local folder.", 
         Bitmap = ResourceBitmaps.Export),
         VSNetControl( "Tools.AnkhSVN", Position = 3 )]
    public class ExportCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            return Enabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            using(ExportDialog dlg = new ExportDialog())
            {
                if ( dlg.ShowDialog( context.HostWindow ) != DialogResult.OK )
                    return;

                context.StartOperation( "Exporting" );
                try
                {
                    ExportRunner runner = new ExportRunner( 
                        dlg.LocalPath, dlg.Revision, dlg.Source, !dlg.NonRecursive );
                    context.UIShell.RunWithProgressDialog( runner, "Exporting" );

                    // make sure it's remembered
                    RegistryUtils.CreateNewTypedUrl( dlg.Source);
                }
                finally
                {
                    context.EndOperation();
                }
            }
        }

        #endregion
    }
}