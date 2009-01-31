using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc.ProjectMap;

namespace Ankh.Scc
{
    /// <summary>
    /// This class is responsible for mapping the settings stored in the solution file
    /// to the real values used by the project system
    /// </summary>
    [GlobalService(typeof(ISccSettingsStore))]
    sealed class SccSettingStorage : AnkhService
    {
        Dictionary<string, SccTranslateData> _solutionToData;
        Dictionary<string, SccTranslateData> _userToData;

        public SccSettingStorage(IAnkhServiceProvider context)
            : base(context)
        {
        }


    }
}
