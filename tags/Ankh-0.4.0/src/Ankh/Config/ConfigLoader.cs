using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Reflection;

namespace Ankh.Config
{
    /// <summary>
    /// Contains functions used to load and save configuration data.
    /// </summary>
    internal sealed class ConfigLoader
    {
        private ConfigLoader()
        {
            // nada
        }

        static ConfigLoader()
        {
            // load the config schema
            Assembly assembly = Assembly.GetExecutingAssembly();
            ConfigLoader.schemas = new XmlSchemaCollection();

            XmlReader reader = new XmlTextReader( assembly.GetManifestResourceStream(
                ConfigLoader.configSchemaResource ) );
            ConfigLoader.schemas.Add( ConfigLoader.configNamespace, reader );


            ConfigLoader.errors = new System.Collections.ArrayList();
        }

        /// <summary>
        /// Loads the Ankh configuration file from the given path.
        /// </summary>
        /// <returns>A Config object.</returns>
        public static Config LoadConfig( string path )
        {
            errors.Clear();

            // make sure there actually is a config file
            EnsureConfig( path );

            Assembly assembly = Assembly.GetExecutingAssembly();
            XmlTextReader reader = new XmlTextReader( path );
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
        /// Loads a Config object from the dir subdir of the user's configuration directory.
        /// </summary>
        /// <param name="config">The Config object to save.</param>
        /// <param name="dir">The subdirectory of the user's configuration directory.</param>
        /// <param name="file">The filename of the config file.</param>
        public static Config LoadConfig( string dir, string file )
        {
            return LoadConfig( BuildConfigDirectoryPath(dir, file) );
        }

        /// <summary>
        /// Saves the supplied Config object to the given path.
        /// </summary>
        /// <param name="config"></param>
        public static void SaveConfig( Config config, string path )
        {
            EnsureConfig( path );

            using( StreamWriter writer = new StreamWriter( path ) )
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add( "", ConfigLoader.configNamespace );
                XmlSerializer serializer = new XmlSerializer( typeof(Config) );
                serializer.Serialize( writer, config, ns );
            }
        }

        /// <summary>
        /// Saves the config in the dir subdir of the user's configuration directory.
        /// </summary>
        /// <param name="config">The Config object to save.</param>
        /// <param name="dir">The subdirectory of the user's configuration directory.</param>
        /// <param name="file">The filename of the config file.</param>
        public static void SaveConfig( Config config, string dir, string file )
        {
            string path = BuildConfigDirectoryPath(dir, file);
            SaveConfig( config, path );
        }

        

        /// <summary>
        /// Returns the errors from the last attempt to load a configuration file.
        /// </summary>
        public static string[] Errors
        {
            get{ return (string[])ConfigLoader.errors.ToArray( typeof(string[]) ); }
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

        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if ( e.Severity == XmlSeverityType.Error )
                errors.Add( e.Message );
        }

        static private string BuildConfigDirectoryPath(string dir, string file)
        {
            return Path.Combine( Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData ), Path.Combine( dir, file ) );
        }


        private static System.Collections.ArrayList errors;
        private static readonly XmlSchemaCollection schemas;
        private const string configNamespace = "http://ankhsvn.com/Config.xsd";
        private const string configFileResource = "Ankh.Config.Config.xml";
        private const string configSchemaResource = "Ankh.Config.Config.xsd";

    }
}
