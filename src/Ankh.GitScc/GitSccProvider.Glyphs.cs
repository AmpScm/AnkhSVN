using System;
using System.Collections.Generic;
using Ankh.Scc;

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
            GitItem item = StatusCache[path];

            return AnkhGlyph.Added;
        }

        protected override string GetGlyphTipText(Selection.SccHierarchy phierHierarchy, uint itemidNode)
        {
            return base.GetGlyphTipText(phierHierarchy, itemidNode);
        }
    }
}
