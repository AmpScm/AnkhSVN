using System;
using Ankh.Config;
using EnvDTE;

namespace Ankh.Commands
{
	/// <summary>
	/// A command that lets you edit the AnkhSVN config file.
	/// </summary>
    [VSNetCommand("EditConfigFile", Tooltip="Edit the AnkhSVN config file", 
         Text = "Edit the AnkhSVN config file", Bitmap = ResourceBitmaps.EditConfigFile ),
    VSNetControl( "Tools.AnkhSVN", Position = 1 ) ]
	public class EditConfigFileCommand : CommandBase
	{
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            return Enabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            context.DTE.ItemOperations.OpenFile( context.ConfigLoader.ConfigPath,
                Constants.vsViewKindPrimary );
        }
	}
}
