// $Id: DiffLocalItemCommand.cs 1503 2004-07-07 04:33:39Z Arild $
using System.IO;
using Ankh.UI;
using EnvDTE;
using SHDocVw;

namespace Ankh.Commands
{
    /// <summary>
    /// Summary description for DiffLocalItem.
    /// </summary>
    [VSNetCommand( "DiffExternalLocalItem", Text="Diff External", 
         Tooltip="Use External Diff against local text base.", 
         Bitmap = ResourceBitmaps.Diff),
   VSNetItemControl( "", Position = 1 )]
    public class DiffExternalLocalItem : DiffLocalItem
    {
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            // Allow external diff if enabled in config file
            if ( context.Config.ChooseDiffMergeManual && context.Config.DiffExePath != null )
                return Enabled;
            else 
                return vsCommandStatus.vsCommandStatusInvisible;
        }

        /// <summary>
        /// Gets path to the diff executable while taking care of config file settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The exe path.</returns>
        protected override string GetExe( Ankh.IContext context )
        {
            return context.Config.DiffExePath;
        }
    }
}



