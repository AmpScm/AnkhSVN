// $Id$
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
            CommandMap commands = new CommandMap();

            // find all the ICommand subclasses in all modules
            foreach( Module module in 
                typeof( CommandMap ).Assembly.GetModules( false ) )
            {
                foreach( Type type in module.FindTypes( 
                    new TypeFilter( CommandMap.CommandTypeFilter ), null ) )
                {
                    try
                    {
                        // is this a VS.NET command?
                        VSNetCommandAttribute[] vsattrs = (VSNetCommandAttribute[])(
                            type.GetCustomAttributes(typeof(VSNetCommandAttribute), false) );
                        if ( vsattrs.Length > 0 )
                            RegisterVSNetCommand( vsattrs[0], type, commands, context );
                    }
                    catch( Exception ex )
                    {
                        Connect.HandleError( ex );
                    }

                    // solution explorer?
                    RepositoryExplorerMenuAttribute [] reattrs = 
                        (RepositoryExplorerMenuAttribute[])(type.GetCustomAttributes(
                        typeof(RepositoryExplorerMenuAttribute), false ) );
                    if ( reattrs.Length > 0 )
                        RegisterRepositoryExplorerCommand( reattrs[0], type, context );
                }
            }

            return commands;            
        }

        /// <summary>
        /// Get rid of any old commands hanging around.
        /// </summary>
        public static void DeleteCommands( AnkhContext context )
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
        private static void RegisterVSNetCommand( VSNetCommandAttribute attr, 
            Type type, CommandMap commands, AnkhContext context )
        {
            
            ICommand cmd = (ICommand)Activator.CreateInstance( type );
            commands.Dictionary[ context.AddIn.ProgID + "." + attr.Name ] = cmd;

            // register the command with the environment
            object []contextGuids = new object[] { };
            cmd.Command = context.DTE.Commands.AddNamedCommand( context.AddIn, attr.Name, attr.Text, attr.Tooltip, false,
                attr.Bitmap, ref contextGuids, (int)vsCommandStatus.vsCommandStatusUnsupported );

            RegisterControl( cmd, type, context );     
        }

        /// <summary>
        /// Registers a commandbar. 
        /// </summary>
        /// <param name="command">The ICommand to attach the command bar to.</param>
        /// <param name="type">The type that handles the command.</param>
        private static void RegisterControl( ICommand cmd, Type type, AnkhContext context )
        {
            // register the command bars
            foreach( VSNetControlAttribute control in type.GetCustomAttributes( 
                typeof(VSNetControlAttribute), false) ) 
            {
             
                CommandBar cmdBar = GetCommandBar( control.CommandBar, context );
                cmd.Command.AddControl( cmdBar, control.Position );
                
            }
        }

        /// <summary>
        /// Registers a right click menu item for the repository explorer control.
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="type"></param>
        /// <param name="context"></param>
        private static void RegisterRepositoryExplorerCommand( 
            RepositoryExplorerMenuAttribute attr, Type type, AnkhContext context )
        {
            ICommand cmd = (ICommand)Activator.CreateInstance( type );
            RepositoryExplorerMenuItem item = new RepositoryExplorerMenuItem( context,
                cmd );

            item.Text = attr.Text;

            context.RepositoryExplorer.AddMenuItem( item, attr.Position );

            item.RegisterWithParent();           
        }

        /// <summary>
        /// Retrieve the command bar associated with a given path, creating them if missing.
        /// </summary>
        /// <param name="name">The path to the command bar, components separated by .</param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static CommandBar GetCommandBar( string name, AnkhContext context )
        {
            string[] path = name.Split( '.' );
            CommandBar bar = (CommandBar)context.DTE.CommandBars[ path[0] ];;
            for( int i = 1; i < path.Length; i++ )
            {
                try
                {
                    // does this command bar already exist?
                    CommandBarControl ctrl = bar.Controls[ path[i] ];
                    bar = (CommandBar)((CommandBarPopup)ctrl).CommandBar;
                }
                catch( Exception )
                {
                    // no, create it
                    bar = (CommandBar)context.DTE.Commands.AddCommandBar( path[i], 
                        vsCommandBarType.vsCommandBarTypeMenu, bar, bar.Controls.Count + 1 );
                }                
            }

            return bar;
        }

	}
}
