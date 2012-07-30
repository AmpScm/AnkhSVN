using System;
using System.Collections.Generic;

namespace Ankh.Scc.SettingMap
{
    class SccTranslatePathInfo
    {
        public string EnlistmentPath
        {
            get { return EnlistmentPathUNC; }
        }

        public string EnlistmentPathUNC
        { 
            get { return ""; }
        }

        public string SolutionPath
        {
            get { return ""; } 
        }
    }
}
