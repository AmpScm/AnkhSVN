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
        /// Gets the user configuration path.
        /// </summary>
        /// <value>The user configuration path.</value>
        string UserConfigurationPath
        {
            get;
        }

        /// <summary>
        /// Occurs when the configuration file changed
        /// </summary>
        event EventHandler ConfigFileChanged;

        bool LoadConfig();

        AnkhConfig GetNewConfigInstance();

        void SaveConfig(AnkhConfig config);

        void SaveReposExplorerRoots(string[] p);

        string[] LoadReposExplorerRoots();

        bool LoadDefaultConfig();

        void SaveWorkingCopyExplorerRoots(string[] rootPaths);

        string[] LoadWorkingCopyExplorerRoots();
    }
}
