// $Id$
using System;
using System.Collections;
using Ankh.Ids;
using System.Collections.Generic;
using System.IO;
using SharpSvn;
using Ankh.Scc;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to cleanup the working copy.
    /// </summary>
    [Command(AnkhCommand.Cleanup)]
    class Cleanup : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned)
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            List<SvnItem> items = new List<SvnItem>(e.Selection.GetSelectedSvnItems(true));

            e.GetService<IProgressRunner>().RunModal("Running Cleanup",
                delegate(object sender, ProgressWorkerArgs a)
                {
                    HybridCollection<string> wcs = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

                    foreach (SvnItem item in items)
                    {
                        if (!item.IsVersioned)
                            continue;

                        SvnWorkingCopy wc = item.WorkingCopy;

                        if (wc != null && !wcs.Contains(wc.FullPath))
                            wcs.Add(wc.FullPath);
                    }

                    SvnCleanUpArgs args = new SvnCleanUpArgs();
                    args.ThrowOnError = false;
                    foreach (string path in wcs)
                        a.Client.CleanUp(path, args);
                });
        }
    }
}