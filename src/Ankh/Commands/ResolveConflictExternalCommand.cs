// $Id: ResolveConflictCommand.cs 1580 2004-07-24 01:44:31Z Arild $
using System;
using Ankh.UI;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections;
using Ankh.Ids;
using Ankh.Configuration;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to resolve conflict between changes using external tool.
    /// </summary>
    [Command(AnkhCommand.ResolveConflictExternal)]
    class ResolveConflictExternalCommand : ResolveConflictCommand
    {
        

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IAnkhConfigurationService cs = e.Context.GetService<IAnkhConfigurationService>();

            AnkhConfig config = cs.Instance;

            if (config.MergeExePath == null)
            {
                e.Enabled = e.Visible = false;
            }
        }

        /// <summary>
        /// Gets path to the diff executable while taking care of config file settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The exe path.</returns>
        protected override string GetExe(IAnkhServiceProvider context)
        {
            return context.GetService<IAnkhConfigurationService>().Instance.MergeExePath;
        }
    }
}