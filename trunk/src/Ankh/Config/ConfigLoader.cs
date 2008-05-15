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
        const string _CONFIGDIRNAME = "AnkhSVN";
        const string _CONFIGFILENAME = "AnkhSVN.user.xml";

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

        public event EventHandler ConfigFileChanged;

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
        /// Load the repos explorer roots from a file in the config dir.
        /// </summary>
        /// <returns></returns>
        public string[] LoadReposExplorerRoots()
        {
            return LoadStrings(REPOSROOTS);
        }


        /// <summary>
        /// Store the repository explorer roots in a file in the config dir.
        /// </summary>
        /// <param name="roots"></param>
        public void SaveReposExplorerRoots(string[] roots)
        {
            SaveStrings(roots, REPOSROOTS, ReposExplorerMutex);
        }


        public void SaveWorkingCopyExplorerRoots(string[] roots)
        {
            SaveStrings(roots, WorkingCopyExplorerRoots, WorkingCopyExplorerMutex);
        }

        public string[] LoadWorkingCopyExplorerRoots()
        {
            return LoadStrings(WorkingCopyExplorerRoots);
        }


        /// <summary>
        /// Returns the errors from the last attempt to load a configuration file.
        /// </summary>
        public string[] Errors
        {
            get { return new string[] { }; }
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



        private static bool WaitForNamedMutex(string mutexName, out Mutex mutex)
        {
            // Make sure only one process tries to write to this.
            mutex = new Mutex(false, mutexName);
            bool ownsMutex = false;
            for (int i = 0; i < 3 && !ownsMutex; i++)
            {
                ownsMutex = mutex.WaitOne(1000, false);
            }
            return ownsMutex;
        }

        /// <summary>
        /// Saves a bunch of strings to the specified file.
        /// </summary>
        /// <param name="roots"></param>
        /// <param name="fileName"></param>
        /// <param name="mutex"></param>
        private void SaveStrings(string[] roots, string fileName, string mutexName)
        {
            //Mutex mutex;
            //if (!WaitForNamedMutex(mutexName, out mutex))
            //{
            //    return;
            //}

            //// Use a helper object to work around a bug in the runtime caused by a specific hotfix (see issue #188 for details)
            //ArrayOfStrings helperArray = new ArrayOfStrings();
            //helperArray.Strings = roots;

            //string path = null;
            //try
            //{
            //    path = Path.Combine(UserConfigurationPath, fileName);
            //    using (StreamWriter writer = new StreamWriter(path))
            //    {
            //        XmlSerializer serializer = new XmlSerializer(typeof(ArrayOfStrings));
            //        serializer.Serialize(writer, helperArray);
            //    }
            //}
            //catch (Exception)
            //{
            //    if (path != null && File.Exists(path))
            //        File.Delete(path);
            //    throw;
            //}
            //finally
            //{
            //    mutex.ReleaseMutex();
            //}
        }

        /// <summary>
        /// Loads an array of strings from the specified file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string[] LoadStrings(string file)
        {
            //lock (this._lock)
            //{
            //    string path = Path.Combine(UserConfigurationPath, file);

            //    if (!File.Exists(path))
            //        return new string[] { };

            //    XmlTextReader reader = new XmlTextReader(path);
            //    try
            //    {
            //        XmlSerializer serializer = new XmlSerializer(typeof(ArrayOfStrings));
            //        // Use a helper object to work around a bug in the runtime caused by a specific hotfix (see issue #188 for details)
            //        return ((ArrayOfStrings)serializer.Deserialize(reader)).Strings;
            //    }
            //    catch (InvalidOperationException ex)
            //    {
            //        throw new ConfigException("Xml error: " + ex.InnerException.Message);
            //    }
            //    catch (XmlException ex)
            //    {
            //        throw new ConfigException("Xml error: " + ex.Message);
            //    }
            //    finally
            //    {
            //        reader.Close();
            //    }
            //}
            return new string[] { };
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

        
        private const string REPOSROOTS = "reposroots.xml";
        private const string WorkingCopyExplorerRoots = "wcroots.xml";

        private const string configNamespace = "http://ankhsvn.com/Config.xsd";
        private const string configFileResource = "Ankh.Configuration.Config.xml";
        private const string configSchemaResource = "Ankh.Configuration.Config.xsd";
        private const string ReposExplorerMutex = "Ankh.Config.ConfigLoader.reposroots.xml";
        private const string WorkingCopyExplorerMutex = "Ankh.Config.ConfigLoader.reposroots.xml";
    }
}
