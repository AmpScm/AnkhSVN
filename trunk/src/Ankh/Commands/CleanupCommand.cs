// $Id$
using System;
using System.Collections;
using Ankh.Ids;
using System.Collections.Generic;
using System.IO;
using SharpSvn;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to cleanup the working copy.
    /// </summary>
    [Command(AnkhCommand.Cleanup)]
    public class Cleanup : CommandBase
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
            IContext context = e.Context.GetService<IContext>();
            using (SvnClient client = e.Context.GetService<ISvnClientPool>().GetClient())
            using (context.StartOperation("Running cleanup"))
            {
                SortedList<string, SvnItem> list = new SortedList<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
                {
                    if (!item.IsVersioned)
                        continue;

                    string path = item.IsDirectory ? item.FullPath : Path.GetDirectoryName(item.FullPath);
                    if (list.ContainsKey(path))
                        continue;

                    list.Add(path, item);
                }

                foreach (string path in new List<string>(list.Keys))
                {
                    string parentPath = Path.GetDirectoryName(path);
                    if (list.ContainsKey(parentPath) && parentPath != path)
                        list.Remove(path);
                }

                SvnCleanUpArgs args = new SvnCleanUpArgs();
                args.ThrowOnError = false;
                foreach (string path in list.Keys)
                    client.CleanUp(path, args);
            }
        }
    }
}



