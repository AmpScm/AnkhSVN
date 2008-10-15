using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Configuration;
using Microsoft.Win32;

namespace Ankh.UI
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAnkhConfigurationService
    {
        Configuration.AnkhConfig Instance
        {
            get;
        }

        void LoadConfig();

        void SaveConfig(AnkhConfig config);

        void LoadDefaultConfig();

        /// <summary>
        /// Opens the user instance key (per hive + per user)
        /// </summary>
        /// <param name="subKey">The sub key.</param>
        /// <returns></returns>
        RegistryKey OpenUserInstanceKey(string subKey);

        /// <summary>
        /// Opens the instance key (per hive)
        /// </summary>
        /// <param name="subKey">The sub key.</param>
        /// <returns></returns>
        RegistryKey OpenInstanceKey(string subKey);

        /// <summary>
        /// Opens the global key (one hklm key)
        /// </summary>
        /// <param name="subKey">The sub key.</param>
        /// <returns></returns>
        RegistryKey OpenGlobalKey(string subKey);


        /// <summary>
        /// Gets the recent log messages.
        /// </summary>
        /// <returns></returns>
        RegistryLifoList GetRecentLogMessages();

        /// <summary>
        /// Gets the recent Repository Urls
        /// </summary>
        /// <returns></returns>
        RegistryLifoList GetRecentReposUrls();
    }
}
