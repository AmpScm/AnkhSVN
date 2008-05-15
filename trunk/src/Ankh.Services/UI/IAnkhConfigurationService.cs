using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Configuration;

namespace Ankh.UI
{
    public interface IAnkhConfigurationService
    {
        Configuration.AnkhConfig Instance
        {
            get;
        }

        /// <summary>
        /// Occurs when the configuration file changed
        /// </summary>
        event EventHandler ConfigFileChanged;

        void LoadConfig();

        AnkhConfig GetNewConfigInstance();

        void SaveConfig(AnkhConfig config);

        void SaveReposExplorerRoots(string[] p);

        string[] LoadReposExplorerRoots();

        void LoadDefaultConfig();

        void SaveWorkingCopyExplorerRoots(string[] rootPaths);

        string[] LoadWorkingCopyExplorerRoots();
    }
}
