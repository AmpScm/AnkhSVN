using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.Configuration;
using Ankh.RepositoryExplorer;
using Ankh.UI;


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
    sealed class ConfigLoader : IAnkhConfigurationService, IVsFileChangeEvents, IDisposable
	{
        const string _CONFIGDIRNAME = "AnkhSVN";
        const string _CONFIGFILENAME = "AnkhSVN.user.xml";
        private static readonly XmlSchemaSet _schemas;

        readonly IAnkhServiceProvider _context;
        readonly string _userConfigDir;
        readonly List<string> _errors;
        readonly object _lock = new object();
        uint _cookie;
        AnkhConfig _instance;		

        static ConfigLoader()
        {
            // load the config schema
            Assembly assembly = typeof(AnkhConfig).Assembly;
            ConfigLoader._schemas = new XmlSchemaSet();

            XmlReader reader = new XmlTextReader(assembly.GetManifestResourceStream(
                ConfigLoader.configSchemaResource));
            ConfigLoader._schemas.Add(ConfigLoader.configNamespace, reader);
        }

        public ConfigLoader(IAnkhServiceProvider context)
            : this(context, ConfigLoader.DefaultUserConfigurationPath)
        {
        }

		public ConfigLoader(IAnkhServiceProvider context, string userConfigurationDir)
		{
            if (context == null)
                throw new ArgumentNullException("context");
            else if (string.IsNullOrEmpty(userConfigurationDir))
                throw new ArgumentNullException("userConfigurationDir");

            _context = context;
            _userConfigDir = userConfigurationDir;

            IVsFileChangeEx changeMonitor = (IVsFileChangeEx)_context.GetService(typeof(SVsFileChangeEx));

            if (changeMonitor != null)
                Marshal.ThrowExceptionForHR(changeMonitor.AdviseFileChange(AnkhConfigurationFile, (uint)_VSFILECHANGEFLAGS.VSFILECHG_Time, this, out _cookie));

            _errors = new List<string>();

			EnsureConfig(this.AnkhConfigurationFile);
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
        /// Gets the user configuration path.
        /// </summary>
        /// <value>The user configuration path.</value>
        public string UserConfigurationPath
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _userConfigDir; }
        }

        public string AnkhConfigurationFile
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return Path.Combine(UserConfigurationPath, _CONFIGFILENAME); }
        }
     		
		/// <summary>
		/// The default config directory - usually %APPDATA%\AnkhSVN
		/// </summary>
		public static string DefaultUserConfigurationPath
		{
			get
			{
				return Path.Combine(Environment.GetFolderPath(
					Environment.SpecialFolder.ApplicationData), ConfigLoader._CONFIGDIRNAME);
			}
		}		

		/// <summary>
		/// Loads the Ankh configuration file from the given path.
		/// </summary>
		/// <returns>A Config object.</returns>
		public AnkhConfig GetNewConfigInstance()
        {
			// make sure there actually is a config file
			EnsureConfig(this.AnkhConfigurationFile);

			_errors.Clear();
			return this.DeserializeConfig(new XmlTextReader(this.AnkhConfigurationFile));
		}

        bool IAnkhConfigurationService.LoadConfig()
        {
            _instance = GetNewConfigInstance();
            return true;
        }

		/// <summary>
		/// Loads the default config file. Used as a fallback if the
		/// existing config file cannot be loaded.
		/// </summary>
		/// <returns></returns>
		public bool LoadDefaultConfig()
		{
			lock (this._lock)
			{
				Assembly assembly = typeof(AnkhConfig).Assembly;
				_instance = this.DeserializeConfig(new XmlTextReader(
					assembly.GetManifestResourceStream(
					ConfigLoader.configFileResource)));

                return true;
			}
		}

		/// <summary>
		/// Saves the supplied Config object
		/// </summary>
		/// <param name="config"></param>
		public void SaveConfig(AnkhConfig config)
		{
			EnsureConfig(this.AnkhConfigurationFile);

			lock (this._lock)
			{
				using (StreamWriter writer = File.CreateText(this.AnkhConfigurationFile))
				{
					XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
					ns.Add("", ConfigLoader.configNamespace);
					XmlSerializer serializer = new XmlSerializer(typeof(AnkhConfig));
					serializer.Serialize(writer, config, ns);
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
			get { return _errors.ToArray(); }
		}


		/// <summary>
		/// Checks if the config dir and file exists at the given path and creates them if not.
		/// </summary>
		/// <param name="path">The path to the config file.</param>
		private void EnsureConfig(string path)
		{
			lock (this._lock)
			{
				string dirname = Path.GetDirectoryName(path);

				// Does the config dir already exist?
				if (!Directory.Exists(dirname))
					Directory.CreateDirectory(dirname);

				// Now we have a dir - is there a config file there?
				if (!File.Exists(path))
				{
					// Create a skeleton config file.
					Assembly assembly = typeof(AnkhConfig).Assembly;
					string config = "";
					using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(
							   ConfigLoader.configFileResource)))
						config = reader.ReadToEnd();

					using (StreamWriter writer = File.CreateText(path))
						writer.Write(config);
				}
			}
		}

		private AnkhConfig DeserializeConfig(XmlReader reader)
		{
			_errors.Clear();

			try
			{
				XmlReaderSettings xs = new XmlReaderSettings();
				xs.ValidationType = ValidationType.Schema;
				xs.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);
				xs.Schemas.Add(_schemas);
				XmlReader vr = XmlReader.Create(reader, xs);

				XmlSerializer serializer = new XmlSerializer(typeof(AnkhConfig));
				return (AnkhConfig)serializer.Deserialize(vr);
			}
			catch (InvalidOperationException ex)
			{
				throw new ConfigException("Xml error: " + ex.InnerException.Message);
			}
			catch (XmlException ex)
			{
				throw new ConfigException("Xml error: " + ex.Message);
			}
			finally
			{
				if (reader != null)
					reader.Close();
			}
		}

		private void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			if (e.Severity == XmlSeverityType.Error)
				_errors.Add(e.Message);
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
			Mutex mutex;
			if (!WaitForNamedMutex(mutexName, out mutex))
			{
				return;
			}

			// Use a helper object to work around a bug in the runtime caused by a specific hotfix (see issue #188 for details)
			ArrayOfStrings helperArray = new ArrayOfStrings();
			helperArray.Strings = roots;

			string path = null;
			try
			{
				path = Path.Combine(UserConfigurationPath, fileName);
				using (StreamWriter writer = new StreamWriter(path))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ArrayOfStrings));
					serializer.Serialize(writer, helperArray);
				}
			}
			catch (Exception)
			{
				if (path != null && File.Exists(path))
					File.Delete(path);
				throw;
			}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Loads an array of strings from the specified file.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		private string[] LoadStrings(string file)
		{
			lock (this._lock)
			{
				string path = Path.Combine(UserConfigurationPath, file);

				if (!File.Exists(path))
					return new string[] { };

				XmlTextReader reader = new XmlTextReader(path);
				try
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ArrayOfStrings));
					// Use a helper object to work around a bug in the runtime caused by a specific hotfix (see issue #188 for details)
					return ((ArrayOfStrings)serializer.Deserialize(reader)).Strings;
				}
				catch (InvalidOperationException ex)
				{
					throw new ConfigException("Xml error: " + ex.InnerException.Message);
				}
				catch (XmlException ex)
				{
					throw new ConfigException("Xml error: " + ex.Message);
				}
				finally
				{
					reader.Close();
				}
			}
		}

        int IVsFileChangeEvents.DirectoryChanged(string pszDirectory)
        {
            return VSConstants.S_OK;
        }

        int IVsFileChangeEvents.FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
        {
            if (rgpszFile == null)
                return VSConstants.E_POINTER;

            bool found = false;

            foreach (string file in rgpszFile)
            {
                if (string.Equals(file, AnkhConfigurationFile, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return VSConstants.S_OK;
            
			lock (this._lock) // BH: Why do we wrap this
			{
                try
                {
                    if (ConfigFileChanged != null)
                        ConfigFileChanged(this, EventArgs.Empty);                    
                }
                catch (Exception ex)
                {
                    IAnkhErrorHandler errorHandler = _context.GetService<IAnkhErrorHandler>();

                    if (errorHandler != null)
                        errorHandler.OnError(ex);
                    else
                        throw;
                }
			}

            return VSConstants.S_OK;
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
