// $Id$
using System;
using System.Collections;
using Ankh.Ids;
using Ankh.Scc;
using System.IO;
using SharpSvn;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you view a repository file.
    /// </summary>
    abstract class ViewRepositoryFileCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool foundOne = false;
            foreach (ISvnRepositoryItem it in e.Selection.GetSelection<ISvnRepositoryItem>())
            {
                if (it.NodeKind != SharpSvn.SvnNodeKind.File || foundOne)
                {
                    // Not a file or multiselect
                    e.Enabled = false;
                    return;
                }

                foundOne = true;
            }

            if (!foundOne)
                e.Enabled = false;
        }

        protected static void SaveFile(CommandEventArgs e, ISvnRepositoryItem ri, string toFile)
        {
            e.GetService<IProgressRunner>().RunModal(
                "Saving File",
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    using (FileStream fs = File.Create(toFile))
                    {
                        ee.Client.Write(ri.Origin.Target, fs);
                    }
                });
        }
    }
}



