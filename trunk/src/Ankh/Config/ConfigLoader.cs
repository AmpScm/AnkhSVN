using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Reflection;
using System.Collections;
using Ankh.RepositoryExplorer;

namespace Ankh.Config
{
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

        /// <summary>
        /// Loads the Ankh configuration file from the given path.
        /// </summary>
        /// <returns>A Config object.</returns>
        public Config LoadConfig()
        {
            errors.Clear();

            // make sure there actually is a config file
            EnsureConfig( this.ConfigPath );

            Assembly assembly = Assembly.GetExecutingAssembly();
            XmlTextReader reader = new XmlTextReader( this.ConfigPath );
            try                                                            
            {
                XmlValidatingReader vr = new XmlValidatingReader( reader );

                vr.ValidationType = ValidationType.Schema;
                vr.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);
                vr.Schemas.Add( schemas );

                XmlSerializer serializer = new XmlSerializer( typeof(Config) );
                return (Config)serializer.Deserialize( vr );
            }
            finally
            {
                reader.Close();
            }

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
            string reposRootPath = Path.Combine( this.configDir, REPOSROOTS );
            try
            {
                using( StreamWriter writer = new StreamWriter( reposRootPath ) )
                {
                    XmlSerializer serializer = new XmlSerializer( typeof(string[]) );
                    serializer.Serialize( writer, roots );
                }
            }
            catch( Exception )
            {
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

        private string ConfigPath
        {
            get{ return Path.Combine( this.configDir, CONFIGFILENAME ); }
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
