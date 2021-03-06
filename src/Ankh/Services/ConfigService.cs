// Copyright 2003-2008 The AnkhSVN Project
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
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

using Ankh.UI;
using Ankh.VS;

namespace Ankh.Configuration
{
    /// <summary>
    /// Contains functions used to load and save configuration data.
    /// </summary>
    [GlobalService(typeof(IAnkhConfigurationService))]
    sealed class ConfigService : AnkhService, IAnkhConfigurationService
    {
        readonly object _lock = new object();
        AnkhConfig _instance;

        public ConfigService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public AnkhConfig Instance
        {
            get { return _instance ?? (_instance = GetNewConfigInstance()); }
        }

        IAnkhSolutionSettings _settings;
        IAnkhSolutionSettings Settings
        {
            get { return _settings ?? (_settings = GetService<IAnkhSolutionSettings>()); }
        }

        /// <summary>
        /// Loads the Ankh configuration file from the given path.
        /// </summary>
        /// <returns>A Config object.</returns>
        public AnkhConfig GetNewConfigInstance()
        {
            AnkhConfig instance = new AnkhConfig();

            SetDefaultsFromRegistry(instance);
            SetSettingsFromRegistry(instance);

            return instance;
        }

        void IAnkhConfigurationService.LoadConfig()
        {
            _instance = GetNewConfigInstance();
        }

        /// <summary>
        /// Loads the default config file. Used as a fallback if the
        /// existing config file cannot be loaded.
        /// </summary>
        /// <returns></returns>
        public void LoadDefaultConfig()
        {
            lock (_lock)
            {
                _instance = new AnkhConfig();
                SetDefaultsFromRegistry(_instance);
            }
        }

        void SetDefaultsFromRegistry(AnkhConfig config)
        {
            using (RegistryKey reg = OpenHKLMCommonKey("Configuration"))
            {
                if (reg != null)
                    foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(config))
                    {
                        string value = reg.GetValue(pd.Name, null) as string;

                        if (value != null)
                            try
                            {
                                pd.SetValue(config, pd.Converter.ConvertFromInvariantString(value));
                            }
                            catch { }
                    }
            }


            using (RegistryKey reg = OpenHKLMKey("Configuration"))
            {
                if (reg != null)
                    foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(config))
                    {
                        string value = reg.GetValue(pd.Name, null) as string;

                        if (value != null)
                            try
                            {
                                pd.SetValue(config, pd.Converter.ConvertFromInvariantString(value));
                            }
                            catch { }
                    }
            }
        }

        void SetSettingsFromRegistry(AnkhConfig config)
        {
            using (RegistryKey reg = OpenHKCUKey("Configuration"))
            {
                if (reg == null)
                    return;

                foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(config))
                {
                    string value = reg.GetValue(pd.Name, null) as string;

                    if (pd.Name == "DiffExePaths")
                    {
                        RegistryKey diffRegKey = OpenHKCUKey("Configuration");
                        diffRegKey = diffRegKey.OpenSubKey(pd.Name);

                        if (diffRegKey != null)
                        {
                            config.DiffExePaths.Clear();

                            foreach (string regKeyName in diffRegKey.GetValueNames())
                            {
                                string regValue = diffRegKey.GetValue(regKeyName) as string;

                                try
                                {
                                    ExtToolDefinition extToolDef = new ExtToolDefinition();
                                    extToolDef.extension = regKeyName;
                                    extToolDef.exePath = regValue;

                                    config.DiffExePaths.Add(extToolDef);
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    else if (pd.Name == "MergeExePaths")
                    {
                        RegistryKey mergeRegKey = OpenHKCUKey("Configuration");
                        mergeRegKey = mergeRegKey.OpenSubKey(pd.Name);

                        if (mergeRegKey != null)
                        {
                            config.MergeExePaths.Clear();

                            foreach (string regKeyName in mergeRegKey.GetValueNames())
                            {
                                string regValue = mergeRegKey.GetValue(regKeyName) as string;

                                try
                                {
                                    ExtToolDefinition extToolDef = new ExtToolDefinition();
                                    extToolDef.extension = regKeyName;
                                    extToolDef.exePath = regValue;

                                    config.MergeExePaths.Add(extToolDef);
                                }
                                catch
                                {

                                }
                            }
                        }
                    }


                    else if (value != null)
                        try
                        {
                            pd.SetValue(config, pd.Converter.ConvertFromInvariantString(value));
                        }
                        catch { }
                }
            }
        }

        /// <summary>
        /// Saves the supplied Config object
        /// </summary>
        /// <param name="config"></param>
        public void SaveConfig(AnkhConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            lock (_lock)
            {
                AnkhConfig defaultConfig = new AnkhConfig();
                SetDefaultsFromRegistry(defaultConfig);

                using (RegistryKey reg = OpenHKCUKey("Configuration"))
                {
                    PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(defaultConfig);
                    foreach (PropertyDescriptor pd in pdc)
                    {
                        object value = pd.GetValue(config);

                        // Set the value only if it is already set previously, or if it's different from the default
                        if (!pd.ShouldSerializeValue(config) && !pd.ShouldSerializeValue(defaultConfig))
                        {
                            reg.DeleteValue(pd.Name, false);
                        }
                        else
                        {
                            if (value.GetType() == typeof(List<ExtToolDefinition>))
                            {
                                List<ExtToolDefinition> myExtToolList = value as List<ExtToolDefinition>;
                                reg.CreateSubKey(pd.Name);
                                RegistryKey extToolReg = OpenHKCUKey("Configuration");
                                extToolReg = extToolReg.OpenSubKey(pd.Name, true);

                                if (extToolReg != null)
                                {
                                    foreach (string extToolDef in extToolReg.GetValueNames())
                                    {
                                        extToolReg.DeleteValue(extToolDef, false);
                                    }

                                    foreach (ExtToolDefinition extTool in myExtToolList)
                                    {
                                        extToolReg.SetValue(extTool.extension, extTool.exePath);
                                    }
                                }
                            }
                            else
                            {
                                reg.SetValue(pd.Name, pd.Converter.ConvertToInvariantString(value));
                            }
                        }
                    }
                }
            }
        }

        RegistryKey OpenHKLMCommonKey(string subKey)
        {
            if (string.IsNullOrEmpty(subKey))
                throw new ArgumentNullException("subKey");

            // Opens the specified key or returns null
            return Registry.LocalMachine.OpenSubKey("SOFTWARE\\AnkhSVN\\AnkhSVN\\Global\\" + subKey, RegistryKeyPermissionCheck.ReadSubTree);
        }

        RegistryKey OpenHKLMKey(string subKey)
        {
            if (string.IsNullOrEmpty(subKey))
                throw new ArgumentNullException("subKey");

            // Opens the specified key or returns null
            return Registry.LocalMachine.OpenSubKey("SOFTWARE\\AnkhSVN\\AnkhSVN\\" + Settings.RegistryHiveSuffix + "\\" + subKey, RegistryKeyPermissionCheck.ReadSubTree);
        }

        /// <summary>
        /// Opens or creates the HKCU key with the specified name
        /// </summary>
        /// <param name="subKey"></param>
        /// <returns></returns>
        RegistryKey OpenHKCUKey(string subKey)
        {
            if (string.IsNullOrEmpty(subKey))
                throw new ArgumentNullException("subKey");

            // Opens or creates the specified key
            return Registry.CurrentUser.CreateSubKey("SOFTWARE\\AnkhSVN\\AnkhSVN\\" + Settings.RegistryHiveSuffix + "\\" + subKey);
        }

        #region IAnkhConfigurationService Members

        RegistryKey IAnkhConfigurationService.OpenUserInstanceKey(string subKey)
        {
            return OpenHKCUKey(subKey);
        }

        RegistryKey IAnkhConfigurationService.OpenInstanceKey(string subKey)
        {
            return OpenHKLMKey(subKey);
        }

        RegistryKey IAnkhConfigurationService.OpenGlobalKey(string subKey)
        {
            return OpenHKLMCommonKey(subKey);
        }


        /// <summary>
        /// Gets the recent log messages.
        /// </summary>
        /// <returns></returns>
        public RegistryLifoList GetRecentLogMessages()
        {
            return new RegistryLifoList(Context, "RecentLogMessages", 32);
        }

        /// <summary>
        /// Gets the recent Repository Urls
        /// </summary>
        /// <returns></returns>
        public RegistryLifoList GetRecentReposUrls()
        {
            return new RegistryLifoList(Context, "RecentRepositoryUrls", 32);
        }

        public void SaveColumnsWidths(Type controlType, IDictionary<string, int> widths)
        {
            if (controlType == null)
                throw new ArgumentNullException("controlType");

            if (widths == null || widths.Count == 0)
                return;

            SaveNumberValues("ColumnWidths", controlType.FullName, widths);
        }

        public IDictionary<string, int> GetColumnWidths(Type controlType)
        {
            if (controlType == null)
                throw new ArgumentNullException("controlType");

            return GetNumberValues("ColumnWidths", controlType.FullName);
        }

        public void SaveWindowPlacement(Type controlType, IDictionary<string, int> placement)
        {
            if (controlType == null)
                throw new ArgumentNullException("controlType");

            SaveNumberValues("WindowPlacements", controlType.FullName, placement);
        }

        public IDictionary<string, int> GetWindowPlacement(Type controlType)
        {
            if (controlType == null)
                throw new ArgumentNullException("controlType");

            return GetNumberValues("WindowPlacements", controlType.FullName);
        }

        void SaveNumberValues(string regKey, string subKey, IDictionary<string, int> values)
        {
            if (string.IsNullOrEmpty(regKey))
                throw new ArgumentNullException("regKey");
            if (string.IsNullOrEmpty(subKey))
                throw new ArgumentNullException("subKey");
            if (values == null)
                throw new ArgumentNullException("values");

            lock (_lock)
            {
                subKey = regKey + "\\" + subKey;
                using (RegistryKey reg = OpenHKCUKey(subKey))
                {
                    if (reg == null)
                        return;
                    foreach (KeyValuePair<string, int> item in values)
                    {
                        if (item.Value <= 0)
                        {
                            reg.DeleteValue(item.Key, false);
                        }
                        else
                        {
                            reg.SetValue(item.Key, item.Value, RegistryValueKind.DWord);
                        }
                    }
                }
            }
        }

        IDictionary<string, int> GetNumberValues(string regKey, string subKey)
        {
            if (string.IsNullOrEmpty(regKey))
                throw new ArgumentNullException("regKey");
            if (string.IsNullOrEmpty(subKey))
                throw new ArgumentNullException("subKey");
            IDictionary<string, int> values;
            lock (_lock)
            {
                subKey = regKey + "\\" + subKey;
                using (RegistryKey reg = OpenHKCUKey(subKey))
                {
                    if (reg == null)
                        return null;
                    HybridCollection<string> hs = new HybridCollection<string>();
                    hs.AddRange(reg.GetValueNames());

                    values = new Dictionary<string, int>(hs.Count);

                    foreach (string item in hs)
                    {
                        int width;
                        if (RegistryUtils.TryGetIntValue(reg, item, out width) && width > 0)
                            values.Add(item, width);
                    }
                }
            }
            return values;
        }

        public bool GetWarningBool(AnkhWarningBool ankhWarningBool)
        {
            using (RegistryKey rk = OpenHKCUKey("Warnings\\Bools"))
            {
                if (rk == null)
                    return false;

                object v = rk.GetValue(ankhWarningBool.ToString());

                if (!(v is int))
                    return false;

                return ((int)v) != 0;
            }
        }

        public void SetWarningBool(AnkhWarningBool ankhWarningBool, bool value)
        {
            using (RegistryKey rk = OpenHKCUKey("Warnings\\Bools"))
            {
                if (rk == null)
                    return;

                if (value)
                    rk.SetValue(ankhWarningBool.ToString(), 1);
                else
                    rk.DeleteValue(ankhWarningBool.ToString());
            }
        }

        #endregion


        #region IAnkhConfigurationService Members

        public RegistryKey OpenVSInstanceKey(string name)
        {
            RegistryKey rootKey = null;

            ILocalRegistry4 lr4 = GetService<ILocalRegistry4>(typeof(SLocalRegistry));

            if (lr4 == null)
                return null;

            uint type;
            const uint VsLocalRegistryRootHandle_CURRENT_USER = unchecked((uint)-2147483647);
            string root;
            if (!VSErr.Succeeded(lr4.GetLocalRegistryRootEx(2 /* _VsLocalRegistryType.Configuration */, out type, out root)))
                return null;

            rootKey = ((type == VsLocalRegistryRootHandle_CURRENT_USER) ? Registry.CurrentUser : Registry.LocalMachine).OpenSubKey(root);

            if (rootKey == null)
                return null;
            else if (string.IsNullOrEmpty(name))
                return rootKey;

            using (rootKey)
            {
                return rootKey.OpenSubKey(name);
            }
        }

        public RegistryKey OpenVSUserKey(string name)
        {
            RegistryKey rootKey = null;

            ILocalRegistry4 lr4 = GetService<ILocalRegistry4>(typeof(SLocalRegistry));

            if (lr4 == null)
                return null;

            uint type;
            const uint VsLocalRegistryRootHandle_CURRENT_USER = unchecked((uint)-2147483647);
            string root;
            if (!VSErr.Succeeded(lr4.GetLocalRegistryRootEx(1 /* _VsLocalRegistryType.UserSettings */, out type, out root)))
                return null;

            rootKey = ((type == VsLocalRegistryRootHandle_CURRENT_USER) ? Registry.CurrentUser : Registry.LocalMachine).OpenSubKey(root);

            if (rootKey == null)
                return null;
            else if (string.IsNullOrEmpty(name))
                return rootKey;

            using (rootKey)
            {
                return rootKey.OpenSubKey(name);
            }
        }

        #endregion
    }
}
