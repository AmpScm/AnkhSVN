using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;

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
    partial class SccProvider : IVsSccGlyphs
    {
        IStatusImageMapper _statusImages;
        protected IStatusImageMapper StatusImages
        {
            [DebuggerStepThrough]
            get { return _statusImages ?? (_statusImages = GetService<IStatusImageMapper>()); }
        }

        protected uint GlyphToStatus(AnkhGlyph glyph)
        {
            SccStatus status;
            switch (glyph)
            {
                case AnkhGlyph.None:
                case AnkhGlyph.Blank:
                case AnkhGlyph.Ignored:
                case AnkhGlyph.FileMissing:
                    // Not versioned
                    status = SccStatus.SCC_STATUS_NOTCONTROLLED;
                    break;
                case AnkhGlyph.Normal:
                case AnkhGlyph.LockedNormal:
                case AnkhGlyph.ChildChanged:
                    // Not changed / no real pending change by itself
                    // (Some tools keep track of checked out as a pending change)
                    status = SccStatus.SCC_STATUS_CONTROLLED;
                    break;
                case AnkhGlyph.MustLock:
                    // Not changed, but needs a lock before editting
                    status = SccStatus.SCC_STATUS_CONTROLLED | SccStatus.SCC_STATUS_LOCKED;
                    break;
                case AnkhGlyph.LockedModified:
                    // Modified under a lock
                    status = SccStatus.SCC_STATUS_CONTROLLED | SccStatus.SCC_STATUS_CHECKEDOUT
                        | SccStatus.SCC_STATUS_OUTBYUSER | SccStatus.SCC_STATUS_OUTEXCLUSIVE;
                    break;
                case AnkhGlyph.InConflict:
                    // Needs fixups after merging. Probably ignored
                    status = SccStatus.SCC_STATUS_CONTROLLED | SccStatus.SCC_STATUS_CHECKEDOUT
                        | SccStatus.SCC_STATUS_OUTBYUSER | SccStatus.SCC_STATUS_MERGED;
                    break;
                //case AnkhGlyph.Added:
                //case AnkhGlyph.ShouldBeAdded:
                //case AnkhGlyph.Deleted:
                //case AnkhGlyph.FileDirty:
                //case AnkhGlyph.CopiedOrMoved:
                default:
                    // Pending change + Checked out
                    status = SccStatus.SCC_STATUS_CONTROLLED | SccStatus.SCC_STATUS_CHECKEDOUT
                        | SccStatus.SCC_STATUS_OUTBYUSER;
                    break;
            }

            return (uint)status;
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

            return VSErr.S_OK;
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
            try
            {
                if (rgpszFullPaths == null || rgsiGlyphs == null)
                    return VSErr.E_POINTER; // Documented as impossible

                if (!IsActive)
                {
                    for (int i = 0; i < cFiles; i++)
                    {
                        if (rgsiGlyphs != null)
                            rgsiGlyphs[i] = VsStateIcon.STATEICON_NOSTATEICON;
                        if (rgdwSccStatus != null)
                            rgdwSccStatus[i] = (uint)SccStatus.SCC_STATUS_NOTCONTROLLED;
                    }
                    return VSErr.S_OK;
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
                    {
                        VsStateIcon icon = (VsStateIcon)glyph;

                        if (icon == VsStateIcon.STATEICON_BLANK || icon == VsStateIcon.STATEICON_NOSTATEICON)
                            rgsiGlyphs[i] = icon;
                        else
                            rgsiGlyphs[i] = (VsStateIcon)((int)icon + _glyphOffset);
                    }

                    if (rgdwSccStatus != null)
                    {
                        // This will make VS use the right texts on refactor, replace, etc.
                        rgdwSccStatus[i] = GlyphToStatus(glyph);
                    }
                }

                return VSErr.S_OK;
            }
            catch(Exception e)
            {
                return VSErr.GetHRForException(e);
            }
        }

        public abstract AnkhGlyph GetPathGlyph(string path);

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

            int glyph = (int)GetPathGlyph(sf) + _glyphOffset;

            hier.SetProperty(VSItemId.Root, (int)__VSHPROPID.VSHPROPID_StateIconIndex, glyph);
        }

        public void ClearSolutionGlyph()
        {
            IVsHierarchy hier = GetService<IVsHierarchy>(typeof(SVsSolution));

            if (hier == null)
                return;

            hier.SetProperty(VSItemId.Root, (int)__VSHPROPID.VSHPROPID_StateIconIndex, (int)AnkhGlyph.Blank);
        }

        int _glyphOffset;
        uint _baseIndex;
        System.Windows.Forms.ImageList _glyphList;

        protected void DisposeGlyphList()
        {
            if (_glyphList != null)
                try
                {
                    _glyphList.Dispose();
                }
                finally
                {
                    _glyphList = null;
                }
        }

        int IVsSccGlyphs.GetCustomGlyphList(uint baseIndex, out uint pdwImageListHandle)
        {
            if (baseIndex == _baseIndex && _glyphList != null)
            {
                pdwImageListHandle = unchecked((uint)_glyphList.Handle);

                return VSErr.S_OK;
            }

            if (_glyphList != null)
            {
                _glyphList.Dispose();
                _glyphList = null;
            }

            // Visual Studio 2002-2010 use a System TreeView control which
            // supports up to 16 glyph images. 12 of those are filled by
            // Visual Studio and the other 4 are overridable.
            // In these versions AnkhSVN provides its 4 images here. (It also
            // forces its own imagelist in the treeview to support more glyphs)
            //
            // 'Visual Studio 11' switched to a WPF control which supports more
            // common controls, but the api isn't extended (yet). We now provide
            // the same 4 glyphs, but add the entire list of images at the end
            // (offset + 16)
            //
            // In VS11 when someone asks for a glyph we provide the higher value.
            // If a user uses the old control (read=Classviewer) it will overflow
            // and provide the default icon, but if it is the new solution explorer
            // it won't overflow and provide our nice glyph.

            // In an attempt to trick the VS2010 'Solution navigator' extension we
            // try to do the same overflow trick in 2010. The solution explorer will
            // then just fall back to our forced image control

            if (StatusImages == null)
            {
                pdwImageListHandle = 0;
                return VSErr.E_FAIL; // Vital service missing
            }

            _glyphList = StatusImages.CreateStatusImageList();

            if (VSVersion.VS2012OrLater || SolutionNavigatorInstalled())
            {
                for (int i = 0; i < 16; i++)
                {
                    using (System.Drawing.Image img = _glyphList.Images[i])
                    {
                        _glyphList.Images.Add(img);
                    }
                }
                _glyphOffset = 16;
            }

            // Now we delete all images before BaseIndex, to properly align our images
            for (int i = (int)baseIndex - 1; i >= 0; i--)
            {
                _glyphList.Images.RemoveAt(i);
            }

            _baseIndex = baseIndex;
            pdwImageListHandle = unchecked((uint)_glyphList.Handle);

            return VSErr.S_OK;
        }

        private bool SolutionNavigatorInstalled()
        {
            if (!VSVersion.VS2010)
                return false;

            IVsShell shell = GetService<IVsShell>(typeof(SVsShell));

            if (shell == null)
                return false;

            Guid solutionNavigatorPackage = new Guid("{cf6a5c16-83b0-4d04-b702-195c35c6e887}");

            int bInstalled;
            if (!VSErr.Succeeded(shell.IsPackageInstalled(ref solutionNavigatorPackage, out bInstalled)))
                return false;

            return (bInstalled != 0);
        }
    }
}
