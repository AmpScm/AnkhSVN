using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc.UI
{
    public interface ILogControl
    {
        bool ShowChangedPaths { get; set; }
        bool ShowLogMessage { get; set; }
        bool StrictNodeHistory { get; set; }
        bool IncludeMergedRevisions { get; set; }
        void FetchAll();
        void Restart();

        IList<SvnOrigin> Origins { get; }        
    }
}
