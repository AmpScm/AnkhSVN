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
    [GlobalService(typeof(IAnkhConfigurationService))]
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
            if(config == null)
                throw new ArgumentNullException("config");

            lock (this._lock)
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
                        if(!pd.ShouldSerializeValue(config) && !pd.ShouldSerializeValue(defaultConfig))
                        {
                            reg.DeleteValue(pd.Name, false);
                        }
                        else
                            reg.SetValue(pd.Name, pd.Converter.ConvertToInvariantString(value));
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
        /// <param name="suffix"></param>
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

        #endregion
    }
}
