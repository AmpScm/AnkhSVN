using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using SharpSvn;

namespace Ankh.Scc
{
    enum SccStatus 
    {
       SCC_STATUS_INVALID          = -1,
       SCC_STATUS_NOTCONTROLLED    = 0x0000,
       SCC_STATUS_CONTROLLED       = 0x0001,
       SCC_STATUS_CHECKEDOUT       = 0x0002,
       SCC_STATUS_OUTOTHER         = 0x0004,
       SCC_STATUS_OUTEXCLUSIVE     = 0x0008,
       SCC_STATUS_OUTMULTIPLE      = 0x0010,
       SCC_STATUS_OUTOFDATE        = 0x0020,
       SCC_STATUS_DELETED          = 0x0040,
       SCC_STATUS_LOCKED           = 0x0080,
       SCC_STATUS_MERGED           = 0x0100,
       SCC_STATUS_SHARED           = 0x0200,
       SCC_STATUS_PINNED           = 0x0400,
       SCC_STATUS_MODIFIED         = 0x0800,
       SCC_STATUS_OUTBYUSER        = 0x1000,
       SCC_STATUS_NOMERGE          = 0x2000,
       SCC_STATUS_RESERVED_1       = 0x4000,
       SCC_STATUS_RESERVED_2       = 0x8000
    }
    
    partial class AnkhSccProvider : IVsSccManager2, IVsSccManagerTooltip, IVsSccGlyphs
    {
        IStatusImageMapper _statusImages;

        IStatusImageMapper StatusImages
        {
            get { return _statusImages ?? (_statusImages = _context.GetService<IStatusImageMapper>()); }
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
            if (rgpszFullPaths == null)
                return VSConstants.E_POINTER; // Documented as impossible

            for (int i = 0; i < cFiles; i++)
            {
                string path = rgpszFullPaths[i];

                AnkhGlyph glyph;
                SvnItem item = StatusCache[rgpszFullPaths[i]];

                if (item != null)
                    glyph = StatusImages.GetStatusImageForSvnItem(item);
                else
                    glyph = AnkhGlyph.None;

                if (glyph == AnkhGlyph.Blank && _fileMap.ContainsKey(path))
                    glyph = AnkhGlyph.ShouldBeAdded;

                if (rgsiGlyphs != null)
                    rgsiGlyphs[i] = (VsStateIcon)glyph;

                if (rgdwSccStatus != null)
                {
                    // This will make VS use the right texts on refactor, replace, etc.

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

                    rgdwSccStatus[i] = (uint)status;
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
            psiGlyph[0] = VsStateIcon.STATEICON_CHECKEDIN;

            return VSConstants.S_OK;
        }

        public int GetGlyphTipText(IVsHierarchy phierHierarchy, uint itemidNode, out string pbstrTooltipText)
        {
            IFileStatusCache cache = StatusCache;
            ISccProjectWalker walker = _context.GetService<ISccProjectWalker>();
            pbstrTooltipText = null;

            if((walker == null) || (cache == null))
                return VSConstants.S_OK;

            
            foreach(string file in walker.GetSccFiles(phierHierarchy, itemidNode, ProjectWalkDepth.SpecialFiles))
            {
                SvnItem item = cache[file];

                if(item.ReadOnlyMustLock)
                {
                    pbstrTooltipText = Resources.ToolTipMustLock;
                    return VSConstants.S_OK;
                }
                else if(item.InConflict)
                {
                    pbstrTooltipText = Resources.ToolTipConflict;
                    return VSConstants.S_OK;
                }        
                else if(!item.Exists)
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

        public int GetCustomGlyphList(uint BaseIndex, out uint pdwImageListHandle)
        {
            pdwImageListHandle = (uint)StatusImages.StatusImageList.Handle.ToInt32();

            this.baseIndex = BaseIndex;

            return VSConstants.S_OK;
        }        
    }
}
