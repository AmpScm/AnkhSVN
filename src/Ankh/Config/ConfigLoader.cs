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
using Ankh.RepositoryExplorer;
using Ankh.UI;
using Microsoft.Win32;
using System.ComponentModel;


namespace Ankh.Configuration
{
    /// <summary>
    /// Represents an error in the configuration file.
    /// </summary>
    public class ConfigException : ApplicationException
    {
        public ConfigException(string msg)
            : base(msg)
        { }

        public ConfigException(string msg, Exception innerException) :
            base(msg, innerException)
        { }
    }

    /// <summary>
    /// Contains functions used to load and save configuration data.
    /// </summary>
    sealed class ConfigLoader : IAnkhConfigurationService, IDisposable
    {
        readonly IAnkhServiceProvider _context;
        readonly object _lock = new object();
        uint _cookie;
        AnkhConfig _instance;

        public ConfigLoader(IAnkhServiceProvider context)
        {

            if (context == null)
                throw new ArgumentNullException("context");


            _context = context;


            EnsureConfig();
        }

        public void Dispose()
        {
            if (_cookie != 0)
            {
                IVsFileChangeEx changeMonitor = (IVsFileChangeEx)_context.GetService(typeof(SVsFileChangeEx));

                if (changeMonitor != null)
                {
                    changeMonitor.UnadviseFileChange(_cookie);
                    _cookie = 0;
                }
            }
        }

        public AnkhConfig Instance
        {
            get { return _instance ?? (_instance = GetSafeConfigInstance()); }
        }

        private AnkhConfig GetSafeConfigInstance()
        {
            try
            {
                return GetNewConfigInstance();
            }
            catch
            {
                LoadDefaultConfig();
                return _instance;
            }
        }





        /// <summary>
        /// Loads the Ankh configuration file from the given path.
        /// </summary>
        /// <returns>A Config object.</returns>
        public AnkhConfig GetNewConfigInstance()
        {
            EnsureConfig();

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
            using (RegistryKey reg = OpenHKLMKey())
            {
                if (reg == null)
                    return;

                foreach (PropertyDescriptor pd in config.GetProperties(null))
                {
                    object value = reg.GetValue(pd.Name, pd.GetValue(config));
                    pd.SetValue(config, value);
                }
            }
        }

        void SetSettingsFromRegistry(AnkhConfig config)
        {
            using (RegistryKey reg = OpenHKCUKey())
            {
                List<string> names = new List<string>(reg.GetValueNames());

                foreach (PropertyDescriptor pd in config.GetProperties(null))
                {
                    if (!names.Contains(pd.Name))
                        continue;
                    object value = reg.GetValue(pd.Name, pd.GetValue(config));
                    pd.SetValue(config, value);
                }
            }
        }

        /// <summary>
        /// Saves the supplied Config object
        /// </summary>
        /// <param name="config"></param>
        public void SaveConfig(AnkhConfig config)
        {
            EnsureConfig();

            lock (this._lock)
            {
                AnkhConfig defaultConfig = new AnkhConfig();
                SetDefaultsFromRegistry(defaultConfig);
                PropertyDescriptorCollection defaultsProps = defaultConfig.GetProperties(null);

                using (RegistryKey reg = OpenHKCUKey())
                {
                    List<string> valueNames = new List<string>(reg.GetValueNames());
                    foreach (PropertyDescriptor pd in config.GetProperties(null))
                    {
                        object value = pd.GetValue(config);
                        object defaultVal = defaultsProps[pd.Name].GetValue(defaultConfig);

                        if (value != null)
                        {
                            // Set the value only if it is already set previously, or if it's different from the default
                            if (valueNames.Contains(pd.Name) || !value.Equals(defaultVal))
                                reg.SetValue(pd.Name, value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ensures the config.
        /// </summary>
        private void EnsureConfig()
        {
            lock (this._lock)
            {
                using (RegistryKey k = Registry.CurrentUser.CreateSubKey(RegistryRoot + "\\AnkhSVN"))
                {
                    GC.KeepAlive(k);
                }
            }
        }

        

        string _registryRoot;
        string RegistryRoot
        {
            get
            {
                if (_registryRoot == null)
                {
                    ILocalRegistry3 regSvc = _context.GetService<ILocalRegistry3>(typeof(SLocalRegistry));
                    ErrorHandler.ThrowOnFailure(regSvc.GetLocalRegistryRoot(out _registryRoot));
                }
                return _registryRoot;
            }
        }


        RegistryKey OpenHKLMKey()
        {
            return Registry.LocalMachine.OpenSubKey(RegistryRoot + "\\AnkhSVN");
        }

        RegistryKey OpenHKCUKey()
        {
            return Registry.CurrentUser.OpenSubKey(RegistryRoot + "\\AnkhSVN", true);
        }
    }
}
