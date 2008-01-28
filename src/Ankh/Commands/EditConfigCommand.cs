using System;
using Ankh.Config;
using EnvDTE;
using Ankh.UI;
using System.Windows.Forms;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you edit the AnkhSVN config file.
    /// </summary>
    [VSNetCommand( "EditConfigFile", Tooltip = "Edit the AnkhSVN configuration",
         Text = "Edit the AnkhSVN configuration", Bitmap = ResourceBitmaps.EditConfigFile ),
    VSNetControl( "Tools.AnkhSVN", Position = 1 )]
    public class EditConfigFileCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus( IContext context )
        {
            return Enabled;
        }

        public override void Execute( IContext context, string parameters )
        {
            Config.Config config = context.ConfigLoader.LoadConfig();
            using ( ConfigurationDialog dialog = new ConfigurationDialog( config ) )
            {
                if ( dialog.ShowDialog( context.HostWindow ) == DialogResult.OK )
                {
                    context.ConfigLoader.SaveConfig( config );
                }
            }
        }
    }
}
