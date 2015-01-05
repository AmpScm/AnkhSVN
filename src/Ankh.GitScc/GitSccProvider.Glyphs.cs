using System;
using System.Collections.Generic;
using Ankh.Scc;
using Ankh.Scc.ProjectMap;

namespace Ankh.GitScc
{
    partial class GitSccProvider
    {
        IGitStatusCache _cache;

        IGitStatusCache StatusCache
        {
            get { return _cache ?? (_cache = GetService<IGitStatusCache>());  }
        }

        public override AnkhGlyph GetPathGlyph(string path)
        {
            return GetPathGlyph(path, true);
        }

        AnkhGlyph GetPathGlyph(string path, bool lookForChildren)
        {
            GitItem item = StatusCache[path];

            if (item == null || StatusImages == null)
                return AnkhGlyph.None;

            AnkhGlyph glyph = StatusImages.GetStatusImageForGitItem(item);

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
            if (!lookForChildren || !ProjectMap.TryGetFile(item.FullPath, out file))
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
            GitItem item = StatusCache[path];

            if (item == null)
                return false;

            return PendingChange.IsPending(item);
        }

        protected override string GetGlyphTipText(Selection.SccHierarchy phierHierarchy, uint itemidNode)
        {
            return base.GetGlyphTipText(phierHierarchy, itemidNode);
        }
    }
}
