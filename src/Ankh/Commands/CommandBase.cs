// $Id$
using System;
using EnvDTE;
using NSvn.Core;
using Ankh.UI;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.IO;
using System.Diagnostics;

namespace Ankh.Commands
{
    /// <summary>
    /// Base class for ICommand instances
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        /// <summary>
        /// Get the status of the command
        /// </summary>
        public abstract vsCommandStatus QueryStatus( IContext context );

        /// <summary>
        /// Execute the command
        /// </summary>
        public abstract void Execute( IContext context, string parameters );

        /// <summary>
        /// The EnvDTE.Command instance corresponding to this command.
        /// </summary>
        public EnvDTE.Command Command
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return this.command; 
            }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                this.command = value;
            }
        }

        /// <summary>
        /// Whether the Shift key is down.
        /// </summary>
        public static bool Shift
        {
            get
            { 
                return Utils.Win32.Win32.GetAsyncKeyState( 
                    (int)System.Windows.Forms.Keys.ShiftKey ) != 0;
            }
        }

        protected void SaveAllDirtyDocuments( IContext context )
        {
            context.DTE.ExecuteCommand( "File.SaveAll", "" );
        }

        protected const vsCommandStatus Enabled = 
            vsCommandStatus.vsCommandStatusEnabled |
            vsCommandStatus.vsCommandStatusSupported;

        protected const vsCommandStatus Disabled = 
            vsCommandStatus.vsCommandStatusSupported;

        protected object GetControl(IContext context, string barName, string name )
        {
            // TODO: either preload this or find a better way to map to 
            // the commandbarcontrols for a command
            object bar = VSNetControlAttribute.GetCommandBar( barName, context );           
            return context.CommandBars.FindControl(bar, barName + "." + name);
        }

        protected static XslTransform GetTransform( IContext context, string name )
        {
            // is the file already there?
            string configDir = Path.GetDirectoryName(context.ConfigLoader.ConfigPath);
            string path = Path.Combine( configDir, name  );

            if ( !File.Exists( path ) )
                CreateTransformFile( path, name );

            Debug.Assert( File.Exists( path ) );

            XPathDocument doc = new XPathDocument( new StreamReader( path) );
            
            XslTransform transform = new XslTransform();
            transform.Load( doc );

            return transform;
        }

        protected static void CreateTransformFile( string path, string name )
        {
            // get the embedded resource and copy it to path
            string resourceName = "Ankh.Commands." + name;
            Stream ins = 
                typeof(CommandBase).Assembly.GetManifestResourceStream( resourceName );
            int len;
            byte[] buffer = new byte[ 4096 ];
            using( FileStream outs = new FileStream( path, FileMode.Create, FileAccess.Write ) )
            {
                while( (len = ins.Read( buffer, 0, 4096 )) > 0 )
                {
                    outs.Write( buffer, 0, len );
                }
            }
        }

        private EnvDTE.Command command;
    }
}