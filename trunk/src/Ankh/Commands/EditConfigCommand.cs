using System;
using Ankh.Config;
using EnvDTE;
using Ankh.UI;
using System.Windows.Forms;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to show the AnkhSVN configuration dialog.
    /// </summary>
    [VSNetCommand(AnkhCommand.EditConfigFile,
		"EditConfigFile",
         Text = "&Configuration...",
         Tooltip = "Configure AnkhSVN.",
         Bitmap = ResourceBitmaps.EditConfigFile ),
         VSNetControl( "Tools.AnkhSVN", Position = 6 )]
    public class EditConfigFileCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus( IContext context )
        {
            return Enabled;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            Config.Config config = context.ConfigLoader.LoadConfig();
            using ( ConfigurationDialog dialog = new ConfigurationDialog( config ) )
            {
                if ( dialog.ShowDialog( context.HostWindow ) == DialogResult.OK )
                {
                    context.ConfigLoader.SaveConfig( config );
                }
            }
        }

        #endregion
    }
}