using System;
using System.Collections;
using System.Reflection;
using EnvDTE;
using System.Diagnostics;

namespace Ankh.Commands
{
	/// <summary>
	/// Responsible for registering the ICommand implementations in this assembly.
	/// </summary>
    public class Commands : DictionaryBase
    {
        /// <summary>
        /// Private constructor to avoid instantiation.
        /// </summary>
        private  Commands()
        {			
        }

        /// <summary>
        /// Returns the ICommand object corresponding to the name.
        /// </summary>
        public ICommand this[ string name ]
        {
            get{ return (ICommand)this.Dictionary[name]; }
        }


        /// <summary>
        /// Registers all commands present in this DLL.
        /// </summary>
        /// <param name="dte">TODO: what to do, what to do?</param>
        public static Commands RegisterCommands( _DTE dte, AddIn addin )
        {
            // delete old commands
            DeleteCommands( dte, addin );


            Commands commands = new Commands();

            // find all the ICommand subclasses in all modules
            foreach( Module module in 
                typeof( Commands ).Assembly.GetModules( false ) )
            {
                foreach( Type type in module.FindTypes( 
                    new TypeFilter( Commands.CommandTypeFilter ), null ) )
                    RegisterCommand( type, commands, dte, addin );
            }

            return commands;            
        }

        /// <summary>
        /// Get rid of any old commands hanging around.
        /// </summary>
        private static void DeleteCommands( _DTE dte, AddIn addin )
        {
            if ( dte.Commands != null )
            {
                // we only want to delete our own commands
                foreach( Command cmd in dte.Commands )
                {
                    if ( cmd.Name != null && cmd.Name.StartsWith( addin.ProgID ) )
                        cmd.Delete();
                }
            }
        }

        /// <summary>
        /// Callback used to filter the type list.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool CommandTypeFilter( Type type, object obj )
        {
            return typeof(ICommand).IsAssignableFrom(type) && !type.IsAbstract;
        }

        /// <summary>
        /// Register a command.
        /// </summary>
        /// <param name="type">A Type object representing the command to register.</param>
        /// <param name="commands">A Commands collection in which to put the command.</param>
        private static void RegisterCommand( Type type, Commands commands, _DTE dte, AddIn addin )
        {
            VSNetCommandAttribute attr = (VSNetCommandAttribute)(
                type.GetCustomAttributes(typeof(VSNetCommandAttribute), false) )[0];
            ICommand cmd = (ICommand)Activator.CreateInstance( type );
            commands.Dictionary[ addin.ProgID + "." + attr.Name ] = cmd;


            // register the command with the environment
            object []contextGuids = new object[] { };
            dte.Commands.AddNamedCommand( addin, attr.Name, attr.Text, attr.Tooltip, false,
                1, ref contextGuids, (int)vsCommandStatus.vsCommandStatusUnsupported );

            System.Windows.Forms.MessageBox.Show( "Registering command " + attr.Name );
        }
	}
}
