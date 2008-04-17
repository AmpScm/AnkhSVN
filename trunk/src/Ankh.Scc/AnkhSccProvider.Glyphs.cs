﻿using System;
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

        IPendingChangesManager _pendingChanges;
        IPendingChangesManager PendingChanges
        {
            [DebuggerStepThrough]
            get { return _pendingChanges ?? (_pendingChanges = GetService<IPendingChangesManager>()); }
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

            switch(glyph)
            {
                case AnkhGlyph.Normal:
                    break; // See below
                case AnkhGlyph.Blank:
                    if (ContainsPath(path) && item.IsVersionable)
                        return AnkhGlyph.ShouldBeAdded;
                    goto default;
                default:
                    return glyph;
            }
                                
            if (DocumentTracker.IsDocumentDirty(item.FullPath))
                return AnkhGlyph.FileDirty;            

            // Let's try to do some simple inheritance trick on scc-special files

            SccProjectFile file;
            if(!lookForChildren || !_fileMap.TryGetValue(item.FullPath, out file))
                return glyph;

            SccProjectFileReference rf = file.FirstReference;

            if(rf != null)
                foreach (string fn in rf.GetSubFiles())
                {
                    AnkhGlyph gl = GetPathGlyph(fn, false);

                    if (gl != AnkhGlyph.Normal)
                        return AnkhGlyph.Free1;
                }

            return AnkhGlyph.Normal;
        }

        private bool ShouldIgnore(SvnItem item)
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
                }
                return VSConstants.S_OK;
            }

            for (int i = 0; i < cFiles; i++)
            {
                string fileName = rgpszFullPaths[i];
                AnkhGlyph glyph = GetPathGlyph(fileName);

                if (rgsiGlyphs != null)
                    rgsiGlyphs[i] = (VsStateIcon)glyph;

                SccProjectFile file;

                if (_fileMap.TryGetValue(fileName, out file))
                {
                    if (file.LastGlyph != glyph)
                    {
                        file.LastGlyph = glyph;
                        PendingChanges.Refresh(fileName);                        
                    }
                }

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

        public int GetGlyphTipText(IVsHierarchy phierHierarchy, uint itemidNode, out string pbstrTooltipText)
        {
            IFileStatusCache cache = StatusCache;
            ISccProjectWalker walker = Context.GetService<ISccProjectWalker>();
            pbstrTooltipText = null;

            if ((walker == null) || (cache == null))
                return VSConstants.S_OK;

            foreach (string file in walker.GetSccFiles(phierHierarchy, itemidNode, ProjectWalkDepth.SpecialFiles))
            {
                SvnItem item = cache[file];

                if (item.IsConflicted)
                {
                    pbstrTooltipText = Resources.ToolTipConflict;
                    return VSConstants.S_OK;
                }
                else if (item.IsObstructed)
                {
                    pbstrTooltipText = item.IsFile ? Resources.ToolTipFileObstructed : Resources.ToolTipDirObstructed;
                }
                else if (item.ReadOnlyMustLock)
                {
                    pbstrTooltipText = Resources.ToolTipMustLock;
                    return VSConstants.S_OK;
                }
                else if (!item.Exists)
                {
                    pbstrTooltipText = Resources.ToolTipDoesNotExist;
                    return VSConstants.S_OK;
                }
                else if (item.IsLocked)
                {
                    pbstrTooltipText = Resources.ToolTipLocked;
                    return VSConstants.S_OK;
                }
            }

            return VSConstants.S_OK;
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
                for(int i = (int)BaseIndex-1; i >= 0; i--)
                {
                    _glyphList.Images.RemoveAt(i);
                }                
            }
            pdwImageListHandle = unchecked((uint)_glyphList.Handle);

            return VSConstants.S_OK;
        }
    }
}
