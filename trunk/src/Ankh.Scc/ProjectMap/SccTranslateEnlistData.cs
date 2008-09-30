using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc.ProjectMap
{
    class SccTranslateEnlistData : SccTranslateData
    {
        public SccTranslateEnlistData(AnkhSccProvider provider, Guid projectId)
            : base(provider, projectId)
        {
        }
    }
}
