// $Id$
using System;
using System.Collections;
using Ankh.Ids;
using Ankh.Scc;

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
            e.GetService<IProgressRunner>().Run(
                "Saving File",
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    using (FileStream fs = File.Create(toFile))
                    {
                        SvnUriTarget t;

                        if (ri.Revision != null)
                            t = new SvnUriTarget(ri.Uri, ri.Revision);
                        else
                            t = ri.Uri;

                        ee.Client.Write(t, fs);
                    }
                });
        }
    }
}



