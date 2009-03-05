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
    /// <summary>
    /// Identical to Microsoft.VisualStudio.Shell.Interop.__SccStatus 
    /// in Microsoft.VisualStudio.Shell.Interop.9.0
    /// </summary>
    enum SccStatus
    {
        SCC_STATUS_INVALID = -1,
        SCC_STATUS_NOTCONTROLLED = 0x0000,
        SCC_STATUS_CONTROLLED = 0x0001,
        SCC_STATUS_CHECKEDOUT = 0x0002,
        SCC_STATUS_OUTOTHER = 0x0004,
        SCC_STATUS_OUTEXCLUSIVE = 0x0008,
        SCC_STATUS_OUTMULTIPLE = 0x0010,
        SCC_STATUS_OUTOFDATE = 0x0020,
        SCC_STATUS_DELETED = 0x0040,
        SCC_STATUS_LOCKED = 0x0080,
        SCC_STATUS_MERGED = 0x0100,
        SCC_STATUS_SHARED = 0x0200,
        SCC_STATUS_PINNED = 0x0400,
        SCC_STATUS_MODIFIED = 0x0800,
        SCC_STATUS_OUTBYUSER = 0x1000,
        SCC_STATUS_NOMERGE = 0x2000,
        SCC_STATUS_RESERVED_1 = 0x4000,
        SCC_STATUS_RESERVED_2 = 0x8000
    }

    partial class AnkhSccProvider : IVsSccManager2, IVsSccManagerTooltip, IVsSccGlyphs
    {
        IStatusImageMapper _statusImages;
        IStatusImageMapper StatusImages
        {
            [DebuggerStepThrough]
            get { return _statusImages ?? (_statusImages = GetService<IStatusImageMapper>()); }
        }

        public AnkhGlyph GetPathGlyph(string path)
        {
            return GetPathGlyph(path, true);
        }

        AnkhGlyph GetPathGlyph(string path, bool lookForChildren)
        {
            SvnItem item = StatusCache[path];

            if (item == null)
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
                SvnStatus lc = item.Status.LocalContentStatus;
                if (lc == SvnStatus.Ignored)
                    return true;
                else if (lc != SvnStatus.NotVersioned)
                    return false;

                item = item.Parent;
            }
            return false;
        }

        uint GlyphToStatus(AnkhGlyph glyph)
        {
            SccStatus status;
            switch (glyph)
            {
                case AnkhGlyph.MustLock:
                    status = SccStatus.SCC_STATUS_CONTROLLED | SccStatus.SCC_STATUS_LOCKED;
                    break;
                case AnkhGlyph.None:
                case AnkhGlyph.Blank:
                case AnkhGlyph.Ignored:
                case AnkhGlyph.FileMissing:
                    status = SccStatus.SCC_STATUS_NOTCONTROLLED;
                    break;
                case AnkhGlyph.LockedModified:
                case AnkhGlyph.LockedNormal:
                    status = SccStatus.SCC_STATUS_CONTROLLED | SccStatus.SCC_STATUS_CHECKEDOUT
                        | SccStatus.SCC_STATUS_OUTBYUSER | SccStatus.SCC_STATUS_OUTEXCLUSIVE;
                    break;
                default:
                    status = SccStatus.SCC_STATUS_CONTROLLED | SccStatus.SCC_STATUS_CHECKEDOUT
                        | SccStatus.SCC_STATUS_OUTBYUSER;
                    break;
            }

            return (uint)status;
        }

        /// <summary>
        /// This method is called by projects to discover the source control glyphs 
        /// to use on files and the files' source control status; this is the only way to get status.
        /// </summary>
        /// <param name="cFiles">The c files.</param>
        /// <param name="rgpszFullPaths">The RGPSZ full paths.</param>
        /// <param name="rgsiGlyphs">The rgsi glyphs.</param>
        /// <param name="rgdwSccStatus">The RGDW SCC status.</param>
        /// <returns></returns>
        public int GetSccGlyph(int cFiles, string[] rgpszFullPaths, VsStateIcon[] rgsiGlyphs, uint[] rgdwSccStatus)
        {
            if (rgpszFullPaths == null || rgsiGlyphs == null)
                return VSConstants.E_POINTER; // Documented as impossible

            if (!_active)
            {
                for (int i = 0; i < cFiles; i++)
                {
                    rgsiGlyphs[i] = VsStateIcon.STATEICON_NOSTATEICON;
                    if(rgdwSccStatus != null)
                        rgdwSccStatus[i] = (uint)SccStatus.SCC_STATUS_NOTCONTROLLED;
                }
                return VSConstants.S_OK;
            }

            for (int i = 0; i < cFiles; i++)
            {
                string file = rgpszFullPaths[i];
                if (!IsSafeSccPath(file))
                {
                    rgsiGlyphs[i] = VsStateIcon.STATEICON_BLANK;
                    if (rgdwSccStatus != null)
                        rgdwSccStatus[i] = (uint)SccStatus.SCC_STATUS_NOTCONTROLLED;
                    continue;
                }

                AnkhGlyph glyph = GetPathGlyph(file);

                if (rgsiGlyphs != null)
                    rgsiGlyphs[i] = (VsStateIcon)glyph;

                if (rgdwSccStatus != null)
                {
                    // This will make VS use the right texts on refactor, replace, etc.
                    rgdwSccStatus[i] = GlyphToStatus(glyph);
                }
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// This function determines which glyph to display, given a combination of status flags.
        /// </summary>
        /// <param name="dwSccStatus">The dw SCC status.</param>
        /// <param name="psiGlyph">The psi glyph.</param>
        /// <returns></returns>
        public int GetSccGlyphFromStatus(uint dwSccStatus, VsStateIcon[] psiGlyph)
        {
            // This method is called when some user (e.g. like classview) wants to combine icons
            // (Unfortunately classview uses a hardcoded mapping)
            psiGlyph[0] = VsStateIcon.STATEICON_BLANK;

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Provides ToolTip text based on the source control data for a specific node in the project's hierarchy Solution Explorer.
        /// </summary>
        /// <param name="phierHierarchy">[in] Owner hierarchy of node (null if it is a solution).</param>
        /// <param name="itemidNode">[in] The ID of the node for which the ToolTip is requested.</param>
        /// <param name="pbstrTooltipText">[out] ToolTip text.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int GetGlyphTipText(IVsHierarchy phierHierarchy, uint itemidNode, out string pbstrTooltipText)
        {
            ISccProjectWalker walker = Context.GetService<ISccProjectWalker>();
            pbstrTooltipText = null;

            if (walker == null || StatusCache == null)
                return VSConstants.S_OK;

            if (phierHierarchy == null)
                phierHierarchy = GetService<IVsHierarchy>(typeof(SVsSolution));

            if (phierHierarchy == null)
                return VSConstants.S_OK;

            StringBuilder sb = new StringBuilder();

            List<SccProjectFile> files = new List<SccProjectFile>();

            int n = 0;
            // pHierarchy = null if it is t
            foreach (string file in walker.GetSccFiles(phierHierarchy, itemidNode, ProjectWalkDepth.Empty, null))
            {
                SccProjectFile spf;
                if (_fileMap.TryGetValue(file, out spf))
                {
                    if (files.Contains(spf))
                        files.Remove(spf); // Must have been added as a subfile and normal file :(

                    files.Insert(n++, spf);

                    foreach (string subfile in spf.FirstReference.GetSubFiles())
                    {
                        if (_fileMap.TryGetValue(subfile, out spf))
                        {
                            if (!files.Contains(spf))
                                files.Add(spf);
                        }
                    }
                }
            }

            string format = (files.Count > 0) ? "{0}: {1}" : "{1}";
            for (int i = 0; i < files.Count; i++)
            {
                SvnItem item = StatusCache[files[i].FullPath];

                if (i >= n) // This is a subitem!
                {
                    if (item.IsModified)
                        sb.AppendFormat(format, item.Name, Resources.ToolTipModified).AppendLine();
                }

                if (item.IsConflicted)
                    sb.AppendFormat(format, item.Name, Resources.ToolTipConflict).AppendLine();

                if (item.IsObstructed)
                    sb.AppendFormat(format, item.Name, item.IsFile ? Resources.ToolTipFileObstructed : Resources.ToolTipDirObstructed).AppendLine();

                if (item.IsReadOnlyMustLock && !item.IsLocked)
                    sb.AppendFormat(format, item.Name, Resources.ToolTipMustLock).AppendLine();

                if (!item.Exists)
                    sb.AppendFormat(format, item.Name, Resources.ToolTipDoesNotExist).AppendLine();

                if (item.IsLocked)
                    sb.AppendFormat(format, item.Name, Resources.ToolTipLocked).AppendLine();
            }

            if (sb.Length > 0)
                pbstrTooltipText = sb.ToString().Trim(); // We added newlines

            return VSConstants.S_OK;
        }

        public void UpdateSolutionGlyph()
        {
            if (!IsActive)
                return;

            string sf = SolutionFilename;
            if (string.IsNullOrEmpty(sf))
                return;

            IVsHierarchy hier = GetService<IVsHierarchy>(typeof(SVsSolution));

            if (hier == null)
                return;

            AnkhGlyph glp = GetPathGlyph(sf);

            hier.SetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_StateIconIndex, (int)glp);
        }

        void ClearSolutionGlyph()
        {
            IVsHierarchy hier = GetService<IVsHierarchy>(typeof(SVsSolution));

            if (hier == null)
                return;

            hier.SetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_StateIconIndex, (int)AnkhGlyph.Blank);
        }

        uint _baseIndex;
        ImageList _glyphList;
        public int GetCustomGlyphList(uint BaseIndex, out uint pdwImageListHandle)
        {
            if (_glyphList != null && BaseIndex != _baseIndex)
            {
                _glyphList.Dispose();
                _glyphList = null;
            }

            // We give VS all our custom glyphs from baseindex upwards
            if (_glyphList == null)
            {
                _baseIndex = BaseIndex;
                _glyphList = StatusImages.CreateStatusImageList();
                for (int i = (int)BaseIndex - 1; i >= 0; i--)
                {
                    _glyphList.Images.RemoveAt(i);
                }
            }
            pdwImageListHandle = unchecked((uint)_glyphList.Handle);

            return VSConstants.S_OK;
        }
    }
}
