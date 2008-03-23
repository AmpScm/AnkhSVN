// $Id: DiffLocalItemCommand.cs 1503 2004-07-07 04:33:39Z Arild $
using System.IO;
using Ankh.UI;
using AnkhSvn.Ids;
using Ankh.Selection;
using Ankh.Configuration;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to diff against local text base using external tool.
    /// </summary>
    [Command(AnkhCommand.DiffExternalLocalItem)]
    public class DiffExternalLocalItem : DiffLocalItem
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            Config config = context.Configuration.Instance;
            // Allow external diff if enabled in config file
            if (!config.ChooseDiffMergeManual || config.DiffExePath == null)
                e.Enabled = e.Visible = false;
        }

        #endregion

        /// <summary>
        /// Gets path to the diff executable while taking care of config file settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The exe path.</returns>
        protected override string GetExe(ISelectionContext selection, IContext context)
        {            
            return context.Configuration.Instance.DiffExePath;
        }
    }
}



