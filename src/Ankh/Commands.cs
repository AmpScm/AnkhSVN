using System;
using System.Collections;
using System.Reflection;
using EnvDTE;
using System.Diagnostics;
using Microsoft.Office.Core;

namespace Ankh
{
	/// <summary>
	/// Responsible for registering the ICommand implementations in this assembly.
	/// </summary>
    internal class CommandMap : DictionaryBase
    {
        /// <summary>
        /// Private constructor to avoid instantiation.
        /// </summary>
        private  CommandMap()
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
        public static CommandMap RegisterCommands( AnkhContext context )
        {
            // delete old commands
            DeleteCommands( context );


            CommandMap commands = new CommandMap();

            // find all the ICommand subclasses in all modules
            foreach( Module module in 
                typeof( CommandMap ).Assembly.GetModules( false ) )
            {
                foreach( Type type in module.FindTypes( 
                    new TypeFilter( CommandMap.CommandTypeFilter ), null ) )
                    RegisterCommand( type, commands, context );
            }

            return commands;            
        }

        /// <summary>
        /// Get rid of any old commands hanging around.
        /// </summary>
        private static void DeleteCommands( AnkhContext context )
        {
            if ( context.DTE.Commands != null )
            {
                // we only want to delete our own commands
                foreach( Command cmd in context.DTE.Commands )
                {
                    if ( cmd.Name != null && cmd.Name.StartsWith( context.AddIn.ProgID ) )
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
        private static void RegisterCommand( Type type, CommandMap commands, AnkhContext context )
        {
            VSNetCommandAttribute attr = (VSNetCommandAttribute)(
                type.GetCustomAttributes(typeof(VSNetCommandAttribute), false) )[0];
            ICommand cmd = (ICommand)Activator.CreateInstance( type );
            commands.Dictionary[ context.AddIn.ProgID + "." + attr.Name ] = cmd;

            // register the command with the environment
            object []contextGuids = new object[] { };
            cmd.Command = context.DTE.Commands.AddNamedCommand( context.AddIn, attr.Name, attr.Text, attr.Tooltip, false,
                1, ref contextGuids, (int)vsCommandStatus.vsCommandStatusUnsupported );

            RegisterControl( cmd, type );     
      

            System.Windows.Forms.MessageBox.Show( "Registering command " + attr.Name );
        }

        /// <summary>
        /// Registers a commandbar. 
        /// </summary>
        /// <param name="command">The ICommand to attach the command bar to.</param>
        /// <param name="type">The type that handles the command.</param>
        private static void RegisterControl( ICommand cmd, Type type )
        {
            // register the command bars
            foreach( VSNetControlAttribute control in type.GetCustomAttributes( 
                typeof(VSNetControlAttribute), false) ) 
            {
                
                CommandBar cmdBar = (CommandBar)cmd.Command.DTE.CommandBars[ control.CommandBar ];
                cmd.Command.AddControl( cmdBar, control.Position );
            }
        }
	}
}
