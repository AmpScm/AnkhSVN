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

        bool GetWarningBool(AnkhWarningBool ankhWarningBool);
        void SetWarningBool(AnkhWarningBool ankhWarningBool, bool value);
    }
}

