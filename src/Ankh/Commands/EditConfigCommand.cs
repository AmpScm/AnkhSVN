using System;
using Ankh.Configuration;
using Ankh.UI;
using System.Windows.Forms;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to show the AnkhSVN configuration dialog.
    /// </summary>
    [Command(AnkhCommand.EditConfigFile)]
    public class EditConfigFileCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            AnkhConfig config = context.Configuration.GetNewConfigInstance();
            using ( ConfigurationDialog dialog = new ConfigurationDialog( config ) )
            {
                if (dialog.ShowDialog(e.Context.DialogOwner) == DialogResult.OK)
                {
                    context.Configuration.SaveConfig( config );
                }
            }
        }
    }
}