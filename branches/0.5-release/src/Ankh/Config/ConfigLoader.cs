using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Reflection;
using System.Collections;
using Ankh.RepositoryExplorer;
using System.Threading;

namespace Ankh.Config
{
    /// <summary>
    /// Represents an error in the configuration file.
    /// </summary>
    internal class ConfigException : ApplicationException
    {
        public ConfigException( string msg ) : base(msg)
        {}

        public ConfigException( string msg, Exception innerException ) : 
            base(msg, innerException)
        {}
    }

    /// <summary>
    /// Contains functions used to load and save configuration data.
    /// </summary>
    internal sealed class ConfigLoader
    {
        public ConfigLoader( string configDir )
        {
            this.configDir = configDir;
            this.errors = new ArrayList();
        }

        public ConfigLoader() : this( ConfigLoader.DefaultConfigDir )
        {
        }

        static ConfigLoader()
        {
            // load the config schema
            Assembly assembly = Assembly.GetExecutingAssembly();
            ConfigLoader.schemas = new XmlSchemaCollection();

            XmlReader reader = new XmlTextReader( assembly.GetManifestResourceStream(
                ConfigLoader.configSchemaResource ) );
            ConfigLoader.schemas.Add( ConfigLoader.configNamespace, reader );            
        }

        /// <summary>
        /// The default config directory - usually %APPDATA%\AnkhSVN
        /// </summary>
        public static string DefaultConfigDir
        {
            get
            {
                return Path.Combine( Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData ), 
                    ConfigLoader.CONFIGDIRNAME );
            }
        }

        public string ConfigPath
        {
            get{ return Path.Combine( this.configDir, CONFIGFILENAME ); }
        }

        /// <summary>
        /// Loads the Ankh configuration file from the given path.
        /// </summary>
        /// <returns>A Config object.</returns>
        public Config LoadConfig()
        {
            errors.Clear();

            // make sure there actually is a config file
            EnsureConfig( this.ConfigPath );
            return this.DeserializeConfig( new XmlTextReader( this.ConfigPath ) );
        }

        /// <summary>
        /// Loads the default config file. Used as a fallback if the
        /// existing config file cannot be loaded.
        /// </summary>
        /// <returns></returns>
        public Config LoadDefaultConfig()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return this.DeserializeConfig( new XmlTextReader(
                assembly.GetManifestResourceStream( 
                ConfigLoader.configFileResource )));         
        }
       
        /// <summary>
        /// Saves the supplied Config object
        /// </summary>
        /// <param name="config"></param>
        public void SaveConfig( Config config )
        {
            EnsureConfig( this.ConfigPath );

            using( StreamWriter writer = new StreamWriter( this.ConfigPath  ) )
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add( "", ConfigLoader.configNamespace );
                XmlSerializer serializer = new XmlSerializer( typeof(Config) );
                serializer.Serialize( writer, config, ns );
            }
        }

        /// <summary>
        /// Load the repos explorer roots from a file in the config dir.
        /// </summary>
        /// <returns></returns>
        public string[] LoadReposExplorerRoots()
        {
            string reposRootPath = Path.Combine( this.configDir, REPOSROOTS );

            if ( !File.Exists(reposRootPath) )
                return new string[]{};

            XmlTextReader reader = new XmlTextReader( reposRootPath );
            try
            {
                XmlSerializer serializer = new XmlSerializer( typeof(string[]) );
                return (string[])serializer.Deserialize( reader );
            }
            catch( InvalidOperationException ex )
            {
                throw new ConfigException( "Xml error: " + ex.InnerException.Message );
            }
            catch( XmlException ex )
            {
                throw new ConfigException( "Xml error: " + ex.Message );
            }
            finally
            {
                reader.Close();
            }

        }

        /// <summary>
        /// Store the repository explorer roots in a file in the config dir.
        /// </summary>
        /// <param name="roots"></param>
        public void SaveReposExplorerRoots( string[] roots  )
        {
            // Make sure only one process tries to write to this.
            Mutex mutex = new Mutex( false, "Ankh.Config.ConfigLoader.reposroots.xml" );
            bool ownsMutex = false;
            for( int i = 0; i < 3 && !ownsMutex; i++ )
            {
                ownsMutex = mutex.WaitOne( 1000, false );
            }
            // If we didn't get it by now, give up
            if ( !ownsMutex )
                return;

            string reposRootPath = null;
            try
            {
                reposRootPath = Path.Combine( this.configDir, REPOSROOTS );
                using( StreamWriter writer = new StreamWriter( reposRootPath ) )
                {
                    XmlSerializer serializer = new XmlSerializer( typeof(string[]) );
                    serializer.Serialize( writer, roots );
                }
            }
            catch( Exception )
            {
                mutex.ReleaseMutex();
                if ( reposRootPath != null )
                    File.Delete( reposRootPath );
                throw;
            }
        }

        /// <summary>
        /// Returns the errors from the last attempt to load a configuration file.
        /// </summary>
        public string[] Errors
        {
            get{ return (string[])this.errors.ToArray( typeof(string[]) ); }
        }


        /// <summary>
        /// Checks if the config dir and file exists at the given path and creates them if not.
        /// </summary>
        /// <param name="path">The path to the config file.</param>
        private static void EnsureConfig( string path )
        {
            string dirname = Path.GetDirectoryName( path );

            // Does the config dir already exist?
            if ( !Directory.Exists( dirname ) )
                Directory.CreateDirectory( dirname );

            // Now we have a dir - is there a config file there?
            if ( !File.Exists( path ) )
            {
                // Create a skeleton config file.
                Assembly assembly = Assembly.GetExecutingAssembly();
                string config = "";
                using( StreamReader reader = new StreamReader( assembly.GetManifestResourceStream( 
                           ConfigLoader.configFileResource ) ) )
                    config = reader.ReadToEnd();

                using( StreamWriter writer = File.CreateText( path ) )
                    writer.Write( config );
            }
        }

        private Config DeserializeConfig( XmlReader reader )
        {
            this.errors.Clear();

            try                                                            
            {                
                XmlValidatingReader vr = new XmlValidatingReader( reader );

                vr.ValidationType = ValidationType.Schema;
                vr.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);
                vr.Schemas.Add( schemas );

                XmlSerializer serializer = new XmlSerializer( typeof(Config) );
                return (Config)serializer.Deserialize( vr );
            }
            catch( InvalidOperationException ex )
            {
                throw new ConfigException( "Xml error: " + ex.InnerException.Message );
            }
            catch( XmlException ex )
            {
                throw new ConfigException( "Xml error: " + ex.Message );
            }
            finally
            {
                if ( reader != null )
                    reader.Close();
            }
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if ( e.Severity == XmlSeverityType.Error )
                errors.Add( e.Message );
        }

        private string configDir;

        private const string REPOSROOTS="reposroots.xml";
        private const string CONFIGFILENAME = "ankhsvn.xml";
        private const string CONFIGDIRNAME = "AnkhSVN";
        private System.Collections.ArrayList errors;
        private static readonly XmlSchemaCollection schemas;
        private const string configNamespace = "http://ankhsvn.com/Config.xsd";
        private const string configFileResource = "Ankh.Config.Config.xml";
        private const string configSchemaResource = "Ankh.Config.Config.xsd";

    }
}
