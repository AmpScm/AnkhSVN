// $Id$
//
// Copyright 2004-2009 The AnkhSVN Project
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

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Windows.Forms;
using SharpSvn;

using Ankh.Scc.UI;
using Ankh.Selection;
using Ankh.UI.PathSelector;
using Ankh.VS;
using System.Collections.Generic;

namespace Ankh.Commands
{
    /// <summary>
    /// Base class for the DiffLocalItem and CreatePatch commands
    /// </summary>
    public abstract class LocalDiffCommandBase : CommandBase
    {
        readonly TempFileCollection _tempFileCollection = new TempFileCollection();

        /// <summary>
        /// Gets the temp file collection.
        /// </summary>
        /// <value>The temp file collection.</value>
        protected TempFileCollection TempFileCollection
        {
            get { return _tempFileCollection; }
        }

        private static string DoExternalDiff(IAnkhServiceProvider context, IEnumerable<SvnItem> selection, SvnRevision start, SvnRevision end)
        {
            foreach (SvnItem item in selection)
            {
                // skip unmodified for a diff against the textbase
                if (start == SvnRevision.Base && end == SvnRevision.Working && !item.IsModified)
                    continue;

                string tempDir = context.GetService<IAnkhTempDirManager>().GetTempDir();

                AnkhDiffArgs da = new AnkhDiffArgs();

                da.BaseFile = GetPath(context, start, item, tempDir);
                da.MineFile = GetPath(context, end, item, tempDir);


                context.GetService<IAnkhDiffHandler>().RunDiff(da);
            }

            return null;
        }

        private static string GetPath(IAnkhServiceProvider context, SvnRevision revision, SvnItem item, string tempDir)
        {
            if (revision == SvnRevision.Working)
            {
                return item.FullPath;
            }

            string strRevision;
            if (revision.RevisionType == SvnRevisionType.Time)
                strRevision = revision.Time.ToLocalTime().ToString("yyyyMMdd_hhmmss");
            else
                strRevision = revision.ToString();
            string tempFile = Path.GetFileNameWithoutExtension(item.Name) + "." + strRevision + Path.GetExtension(item.Name);
            tempFile = Path.Combine(tempDir, tempFile);
            // we need to get it from the repos
            context.GetService<IProgressRunner>().RunModal(CommandStrings.RetrievingFileForComparison, delegate(object o, ProgressWorkerArgs ee)
            {
                SvnTarget target;

                switch (revision.RevisionType)
                {
                    case SvnRevisionType.Head:
                    case SvnRevisionType.Number:
                    case SvnRevisionType.Time:
                        target = item.Uri;
                        break;
                    default:
                        target = item.FullPath;
                        break;
                }
                SvnWriteArgs args = new SvnWriteArgs();
                args.Revision = revision;
                args.AddExpectedError(SvnErrorCode.SVN_ERR_CLIENT_UNRELATED_RESOURCES);

                using (FileStream stream = File.Create(tempFile))
                {
                    ee.Client.Write(target, stream, args);
                }
            });

            return tempFile;
        }
    }
}
