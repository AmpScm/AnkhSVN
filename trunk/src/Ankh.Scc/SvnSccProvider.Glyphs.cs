// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using SharpSvn;
using System.Windows.Forms;
using Ankh.Scc.ProjectMap;
using System.Diagnostics;

namespace Ankh.Scc
{
    partial class SvnSccProvider : IVsSccManagerTooltip
    {
        public override AnkhGlyph GetPathGlyph(string path)
        {
            return GetPathGlyph(path, true);
        }

        AnkhGlyph GetPathGlyph(string path, bool lookForChildren)
        {
            SvnItem item = StatusCache[path];

            if (item == null  || StatusImages == null)
                return AnkhGlyph.None;

            AnkhGlyph glyph = StatusImages.GetStatusImageForSvnItem(item);

            switch (glyph)
            {
                case AnkhGlyph.Normal:
                    break; // See below
                default:
                    return glyph;
            }

            // Let's try to do some simple inheritance trick on scc-special files with a normal icon 
            // as those are collapsed by default

            SccProjectFile file;
            if (!lookForChildren || !_fileMap.TryGetValue(item.FullPath, out file))
                return glyph;

            SccProjectFileReference rf = file.FirstReference;

            if (rf != null)
            {
                foreach (string fn in rf.GetSubFiles())
                {
                    if (IsChildChanged(fn))
                        return AnkhGlyph.ChildChanged;
                }

                // TODO: Make configurable and review missing/lock etc.
                //if (ProjectGlyphRecursive && rf.IsProjectFile)
                //{
                //    foreach (string fn in rf.Project.GetAllFiles())
                //    {
                //        if (IsChildChanged(fn))
                //            return AnkhGlyph.ChildChanged;
                //    }
                //}
            }

            return AnkhGlyph.Normal;
        }

        private bool IsChildChanged(string path)
        {
            SvnItem item = StatusCache[path];

            if (item == null)
                return false;

            return PendingChange.IsPending(item);
        }

        bool ShouldIgnore(SvnItem item)
        {
            while (item != null)
            {
                SvnStatus lc = item.Status.LocalNodeStatus;
                if (lc == SvnStatus.Ignored)
                    return true;
                else if (lc != SvnStatus.NotVersioned)
                    return false;

                item = item.Parent;
            }
            return false;
        }

        ISccProjectWalker _walker;
        ISccProjectWalker Walker
        {
            get { return _walker ?? (_walker = GetService<ISccProjectWalker>()); }
        }

        /// <summary>
        /// Provides ToolTip text based on the source control data for a specific node in the project's hierarchy Solution Explorer.
        /// </summary>
        /// <param name="phierHierarchy">[in] Owner hierarchy of node (null if it is a solution).</param>
        /// <param name="itemidNode">[in] The ID of the node for which the ToolTip is requested.</param>
        /// <param name="pbstrTooltipText">[out] ToolTip text.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSErr.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int GetGlyphTipText(IVsHierarchy phierHierarchy, uint itemidNode, out string pbstrTooltipText)
        {
            pbstrTooltipText = null;

            if (Walker == null || StatusCache == null || phierHierarchy == null)
                return VSErr.S_OK;

            HybridCollection<string> files = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

            int n = 0;
            foreach (string file in Walker.GetSccFiles(phierHierarchy, itemidNode, ProjectWalkDepth.Empty, null))
            {
                if (files.Contains(file) || !SvnItem.IsValidPath(file))
                    continue;

                files.Add(file);

                SccProjectFile spf;
                if (_fileMap.TryGetValue(file, out spf))
                {
                    foreach (string subfile in spf.FirstReference.GetSubFiles())
                    {
                        if (!files.Contains(subfile))
                            files.Add(subfile);
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            string format = (files.Count > 0) ? "{0}: {1}" : "{1}";
            int i = 0;
            foreach (string file in files)
            {
                SvnItem item = StatusCache[file];

                if (i >= n) // This is a subitem!
                {
                    if (item.IsModified)
                        sb.AppendFormat(format, item.Name, Resources.ToolTipModified).AppendLine();
                }

                if (item.IsConflicted)
                    sb.AppendFormat(format, item.Name, Resources.ToolTipConflict).AppendLine();

                if (item.IsObstructed)
                    sb.AppendFormat(format, item.Name, item.IsFile ? Resources.ToolTipFileObstructed : Resources.ToolTipDirObstructed).AppendLine();

                if (!item.Exists && item.IsVersioned)
                    sb.AppendFormat(format, item.Name, Resources.ToolTipDoesNotExist).AppendLine();

                if (item.IsLocked)
                    sb.AppendFormat(format, item.Name, Resources.ToolTipLocked).AppendLine();
                i++;

                if (sb.Length > 2048)
                    break;
            }

            if (sb.Length > 0)
                pbstrTooltipText = sb.ToString().Trim(); // We added newlines

            return VSErr.S_OK;
        }


    }
}
