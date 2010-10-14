// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Configuration;
using Microsoft.Win32;

namespace Ankh.UI
{
    public enum AnkhWarningBool
    {
        FatFsFound
    }
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

        /// <summary>
        /// Save SmartColumns widths to registry
        /// </summary>
        /// <param name="subKey">SubKey name</param>
        /// <param name="widths">Dictionary of column names and widths</param>
        void SaveColumnsWidths(Type subKey, IDictionary<string, int> widths);

        /// <summary>
        /// Get SmartColumns widths from registry
        /// </summary>
        /// <param name="subKey">SybKey name</param>
        /// <returns>Dictionary of column names and widths</returns>
        IDictionary<string, int> GetColumnWidths(Type subKey);

        /// <summary>
        /// Save window size and position in registry
        /// </summary>
        /// <param name="subKey">SubKey name</param>
        /// <param name="placement">Dictionary of window size and posiotion</param>
        void SaveWindowPlacement(Type subKey, IDictionary<string, int> placement);

        /// <summary>
        /// Get window size and position from registry
        /// </summary>
        /// <param name="subKey">SybKey name</param>
        /// <returns>Dictionary of window size and position</returns>
        IDictionary<string, int> GetWindowPlacement(Type subKey);

        bool GetWarningBool(AnkhWarningBool ankhWarningBool);
        void SetWarningBool(AnkhWarningBool ankhWarningBool, bool value);
    }
}

