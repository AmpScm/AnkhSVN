// $Id$
//
// Copyright 2003-2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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
                        SvnWriteArgs args = new SvnWriteArgs();
                        if(ri.Revision != null)
                            args.Revision = ri.Revision;

                        ee.Client.Write(ri.Origin.Target, fs, args);
                    }
                });
        }
    }
}



