using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using SharpSvn;

namespace Ankh.VSPackage.Scc
{
    partial class AnkhSccProvider : IVsSccManager2, IVsSccManagerTooltip
    {
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
            for (int i = 0; i < cFiles; i++)
            {
                SvnItem item = this.context.StatusCache[rgpszFullPaths[i]];
                NodeStatus nodeStatus = GenerateStatus(item);
                rgsiGlyphs[i] = (VsStateIcon)
                    StatusImages.GetStatusImageForNodeStatus(nodeStatus);
                rgdwSccStatus[i] = 21;
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
            // Todo find file via hierarchy, reusing code from SelectionContext 
            pbstrTooltipText = "AnkhSvn";
            return VSConstants.S_OK;
        }

        public int GetCustomGlyphList(uint BaseIndex, out uint pdwImageListHandle)
        {
            pdwImageListHandle = (uint)StatusImages.StatusImageList.Handle.ToInt32();

            this.baseIndex = BaseIndex;

            return VSConstants.S_OK;
        }

        protected static NodeStatus GenerateStatus(SvnItem item)
        {
            AnkhStatus status = item.Status;
            NodeStatusKind kind;
            if (status.LocalContentStatus != SvnStatus.Normal)
            {
                kind = (NodeStatusKind)status.LocalContentStatus;
            }
            else if (status.LocalPropertyStatus != SvnStatus.Normal &&
                status.LocalPropertyStatus != SvnStatus.None)
            {
                kind = (NodeStatusKind)status.LocalPropertyStatus;
            }
            else
            {
                kind = NodeStatusKind.Normal;
            }

            return new NodeStatus(kind, item.IsReadOnly, item.IsLocked);
        }
    }
}
