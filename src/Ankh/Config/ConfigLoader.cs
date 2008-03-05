using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Reflection;
using System.Collections;
using Ankh.RepositoryExplorer;
using System.Threading;
using System.Diagnostics;

namespace Ankh.Config
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
	public sealed class ConfigLoader
	{

		public event EventHandler ConfigFileChanged;

		public ConfigLoader(string configDir)
		{
			this.configDir = configDir;
			this.errors = new ArrayList();

			EnsureConfig(this.ConfigPath);

			this.configFileStamp = File.GetLastWriteTime(this.ConfigPath);
			this.WatchConfigFile();
		}



		public ConfigLoader()
			: this(ConfigLoader.DefaultConfigDir)
		{
		}

		static ConfigLoader()
		{
			// load the config schema
			Assembly assembly = Assembly.GetExecutingAssembly();
			ConfigLoader.schemas = new XmlSchemaSet();

			XmlReader reader = new XmlTextReader(assembly.GetManifestResourceStream(
				ConfigLoader.configSchemaResource));
			ConfigLoader.schemas.Add(ConfigLoader.configNamespace, reader);
		}

		/// <summary>
		/// The default config directory - usually %APPDATA%\AnkhSVN
		/// </summary>
		public static string DefaultConfigDir
		{
			get
			{
				return Path.Combine(Environment.GetFolderPath(
					Environment.SpecialFolder.ApplicationData),
					ConfigLoader.CONFIGDIRNAME);
			}
		}

		public string ConfigPath
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return Path.Combine(this.configDir, CONFIGFILENAME); }
		}

		public string ConfigDir
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return this.configDir; }
		}

		/// <summary>
		/// Loads the Ankh configuration file from the given path.
		/// </summary>
		/// <returns>A Config object.</returns>
		public Config LoadConfig()
		{
			lock (this.configFileLock)
			{
				// make sure there actually is a config file
				EnsureConfig(this.ConfigPath);

				errors.Clear();
				return this.DeserializeConfig(new XmlTextReader(this.ConfigPath));
			}
		}

		/// <summary>
		/// Loads the default config file. Used as a fallback if the
		/// existing config file cannot be loaded.
		/// </summary>
		/// <returns></returns>
		public Config LoadDefaultConfig()
		{
			lock (this.configFileLock)
			{
				Assembly assembly = Assembly.GetExecutingAssembly();
				return this.DeserializeConfig(new XmlTextReader(
					assembly.GetManifestResourceStream(
					ConfigLoader.configFileResource)));
			}
		}

		/// <summary>
		/// Saves the supplied Config object
		/// </summary>
		/// <param name="config"></param>
		public void SaveConfig(Config config)
		{
			EnsureConfig(this.ConfigPath);

			lock (this.configFileLock)
			{
				using (StreamWriter writer = new StreamWriter(this.ConfigPath))
				{
					XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
					ns.Add("", ConfigLoader.configNamespace);
					XmlSerializer serializer = new XmlSerializer(typeof(Config));
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
			get { return (string[])this.errors.ToArray(typeof(string[])); }
		}


		/// <summary>
		/// Checks if the config dir and file exists at the given path and creates them if not.
		/// </summary>
		/// <param name="path">The path to the config file.</param>
		private void EnsureConfig(string path)
		{
			lock (this.configFileLock)
			{
				string dirname = Path.GetDirectoryName(path);

				// Does the config dir already exist?
				if (!Directory.Exists(dirname))
					Directory.CreateDirectory(dirname);

				// Now we have a dir - is there a config file there?
				if (!File.Exists(path))
				{
					// Create a skeleton config file.
					Assembly assembly = Assembly.GetExecutingAssembly();
					string config = "";
					using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(
							   ConfigLoader.configFileResource)))
						config = reader.ReadToEnd();

					using (StreamWriter writer = File.CreateText(path))
						writer.Write(config);
				}
			}
		}

		private Config DeserializeConfig(XmlReader reader)
		{
			this.errors.Clear();

			try
			{
				XmlReaderSettings xs = new XmlReaderSettings();
				xs.ValidationType = ValidationType.Schema;
				xs.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);
				xs.Schemas.Add(schemas);
				XmlReader vr = XmlReader.Create(reader, xs);

				XmlSerializer serializer = new XmlSerializer(typeof(Config));
				return (Config)serializer.Deserialize(vr);
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
				errors.Add(e.Message);
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
				path = Path.Combine(this.configDir, fileName);
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
			lock (this.configFileLock)
			{
				string path = Path.Combine(this.configDir, file);

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


		private void WatchConfigFile()
		{
			Debug.Assert(File.Exists(this.ConfigPath), "Config file does not exist: " + this.ConfigPath);

			this.configFileWatcher = new FileSystemWatcher(this.ConfigDir, CONFIGFILENAME);
			this.configFileWatcher.IncludeSubdirectories = false;
			this.configFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
			this.configFileWatcher.Changed += new FileSystemEventHandler(configFileWatcher_Changed);
			this.configFileWatcher.EnableRaisingEvents = true;
		}

		void configFileWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			lock (this.configFileLock)
			{
				try
				{
					if (!File.Exists(this.ConfigPath))
					{
						return;
					}

					DateTime dt = File.GetLastWriteTime(this.ConfigPath);
					if (dt != this.configFileStamp)
					{
						this.configFileStamp = dt;
						if (this.ConfigFileChanged != null)
						{
							this.ConfigFileChanged(this, EventArgs.Empty);
						}
					}
				}
				catch (Exception
#if DEBUG
                        ex
#endif
)
				{
					// swallow
#if DEBUG
                    Debug.WriteLine( ex );
#endif
				}
			}
		}

		private object configFileLock = new object();
		private DateTime configFileStamp;
		private string configDir;
		private FileSystemWatcher configFileWatcher;
		private const string REPOSROOTS = "reposroots.xml";
		private const string WorkingCopyExplorerRoots = "wcroots.xml";
		private const string CONFIGFILENAME = "ankhsvn.xml";
		private const string CONFIGDIRNAME = "AnkhSVN";
		private System.Collections.ArrayList errors;
		private static readonly XmlSchemaSet schemas;
		private const string configNamespace = "http://ankhsvn.com/Config.xsd";
		private const string configFileResource = "Ankh.Config.Config.xml";
		private const string configSchemaResource = "Ankh.Config.Config.xsd";
		private const string ReposExplorerMutex = "Ankh.Config.ConfigLoader.reposroots.xml";
		private const string WorkingCopyExplorerMutex = "Ankh.Config.ConfigLoader.reposroots.xml";



	}


	[Serializable]
	[XmlRoot("ArrayOfString")]
	public class ArrayOfStrings
	{
		[XmlElement("string")]
		public string[] Strings = new string[0];
	}
}
