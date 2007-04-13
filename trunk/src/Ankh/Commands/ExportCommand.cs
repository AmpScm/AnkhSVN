// $Id: ExportCommand.cs 1410 2004-06-24 01:45:04Z Arild $
using System;
using Ankh.UI;
using System.Windows.Forms;
using Utils;


namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you export a repository directory.
    /// </summary>
    [VSNetCommand("Export", Tooltip="Export a repository directory", 
         Text = "E&xport a repository or local directory...", 
         Bitmap = ResourceBitmaps.Export),
    VSNetControl( "Tools.AnkhSVN", Position = 1 ) ]
    public class ExportCommand : CommandBase
    {
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
    }
}
