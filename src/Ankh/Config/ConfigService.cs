using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.Configuration;
using Ankh.UI;
using Microsoft.Win32;
using System.ComponentModel;
using Ankh.VS;


namespace Ankh.Configuration
{
    /// <summary>
    /// Contains functions used to load and save configuration data.
    /// </summary>
    sealed class ConfigService : AnkhService, IAnkhConfigurationService, IDisposable
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
            lock (this._lock)
            {
                _instance = new AnkhConfig();
                SetDefaultsFromRegistry(_instance);
            }
        }

        void SetDefaultsFromRegistry(AnkhConfig config)
        {
            using (RegistryKey reg = OpenHKLMKey("Configuration"))
            {
                if (reg == null)
                    return;

                foreach (PropertyDescriptor pd in config.GetProperties(null))
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

                foreach (PropertyDescriptor pd in config.GetProperties(null))
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

        /// <summary>
        /// Saves the supplied Config object
        /// </summary>
        /// <param name="config"></param>
        public void SaveConfig(AnkhConfig config)
        {
            lock (this._lock)
            {
                AnkhConfig defaultConfig = new AnkhConfig();
                SetDefaultsFromRegistry(defaultConfig);
                PropertyDescriptorCollection defaultsProps = defaultConfig.GetProperties(null);

                using (RegistryKey reg = OpenHKCUKey("Configuration"))
                {
                    HybridCollection<string> names = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                    names.AddRange(reg.GetValueNames());

                    foreach (PropertyDescriptor pd in config.GetProperties(null))
                    {
                        object value = pd.GetValue(config);
                        object defaultVal = defaultsProps[pd.Name].GetValue(defaultConfig);

                        // Set the value only if it is already set previously, or if it's different from the default
                        if(value == null || value.Equals(defaultVal))
                        {
                            if(names.Contains(pd.Name))
                                reg.DeleteValue(pd.Name);
                        }
                        else
                            reg.SetValue(pd.Name, pd.Converter.ConvertToInvariantString(value));
                    }
                }
            }
        }

        RegistryKey OpenHKLMKey(string suffix)
        {
            if (string.IsNullOrEmpty("suffix"))
                throw new ArgumentNullException("suffix");

            // Opens the specified key or returns null
            return Registry.LocalMachine.OpenSubKey("SOFTWARE\\AnkhSVN\\AnkhSVN\\" + Settings.RegistryHiveSuffix + "\\" + suffix, RegistryKeyPermissionCheck.ReadSubTree);
        }

        /// <summary>
        /// Opens or creates the HKCU key with the specified name
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        RegistryKey OpenHKCUKey(string suffix)
        {
            if (string.IsNullOrEmpty("suffix"))
                throw new ArgumentNullException("suffix");

            // Opens or creates the specified key
            return Registry.CurrentUser.CreateSubKey("SOFTWARE\\AnkhSVN\\AnkhSVN\\" + Settings.RegistryHiveSuffix + "\\" + suffix);
        }
    }
}
