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
    VSNetControl( "MenuBar.Tools.AnkhSVN", Position = 1 ) ]
	internal class EditConfigFileCommand : CommandBase
	{
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            return Enabled;
        }

        public override void Execute(AnkhContext context, string parameters)
        {
            context.DTE.ItemOperations.OpenFile( context.ConfigLoader.ConfigPath,
                Constants.vsViewKindPrimary );
        }
	}
}
