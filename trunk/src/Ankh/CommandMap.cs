// $Id$
using System;
using System.Collections;
using System.Reflection;
using EnvDTE;
using System.Diagnostics;
using Microsoft.Office.Core;
using System.Runtime.InteropServices;

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
        public static CommandMap LoadCommands( AnkhContext context, bool register )
        {
            CreateReposExplorerPopup( context );
            CreateAnkhSubMenu( context );

            CommandMap commands = new CommandMap();

            // find all the ICommand subclasses in all modules
            foreach( Module module in 
                typeof( CommandMap ).Assembly.GetModules( false ) )
            {
                foreach( Type type in module.FindTypes( 
                    new TypeFilter( CommandMap.CommandTypeFilter ), null ) )
                {  
                    // is this a VS.NET command?
                    VSNetCommandAttribute[] vsattrs = (VSNetCommandAttribute[])(
                        type.GetCustomAttributes(typeof(VSNetCommandAttribute), false) );
                    if ( vsattrs.Length > 0 )
                    {
                        // put it in the dict
                        ICommand cmd = (ICommand)Activator.CreateInstance( type );
                        commands.Dictionary[ context.AddIn.ProgID + "." + vsattrs[0].Name ] = cmd;

                        // do we want to register it?
                        if ( register )
                            RegisterVSNetCommand( vsattrs[0], cmd,  context );
                    }
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
                    try
                    {
                        if ( cmd.Name != null && cmd.Name.StartsWith( context.AddIn.ProgID ) )
                            cmd.Delete();
                    }
                    catch( System.IO.FileNotFoundException )
                    {
                        // swallow
                        // HACK: find out why FileNotFoundException is thrown
                    }
                    catch( COMException )
                    {
                        // swallow
                        // HACK: ditto
                    }
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
            ICommand cmd, AnkhContext context )
        {
            // register the command with the environment
            object []contextGuids = new object[] { };
            cmd.Command = context.DTE.Commands.AddNamedCommand( context.AddIn, attr.Name, attr.Text, attr.Tooltip, false,
                attr.Bitmap, ref contextGuids, (int)vsCommandStatus.vsCommandStatusUnsupported );

            RegisterControl( cmd, context );     
        }

        /// <summary>
        /// Registers a commandbar. 
        /// </summary>
        /// <param name="command">The ICommand to attach the command bar to.</param>
        /// <param name="type">The type that handles the command.</param>
        private static void RegisterControl( ICommand cmd, AnkhContext context )
        {
            // register the command bars
            foreach( VSNetControlAttribute control in cmd.GetType().GetCustomAttributes( 
                typeof(VSNetControlAttribute), false) ) 
            {
             
                string name = ((VSNetCommandAttribute)cmd.GetType().GetCustomAttributes(
                    typeof(VSNetCommandAttribute), false )[0]).Name;
                CommandBar cmdBar = GetCommandBar( control.CommandBar, context );
                CommandBarControl cntrl = cmd.Command.AddControl( cmdBar, control.Position );
                cntrl.Tag = control.CommandBar + "." + name;                
            }
        }

        /// <summary>
        /// Retrieve the command bar associated with a given path, creating them if missing.
        /// </summary>
        /// <param name="name">The path to the command bar, components separated by .</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static CommandBar GetCommandBar( string name, AnkhContext context )
        {
            string[] path = name.Split( '.' );
            CommandBar bar;

            //TODO: is this really necessary?
            if ( path[0] == context.RepositoryExplorer.CommandBar.Name )
                bar = context.RepositoryExplorer.CommandBar;
            else
                bar = (CommandBar)context.DTE.CommandBars[ path[0] ];;
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

        private static void CreateReposExplorerPopup( AnkhContext context )
        {
            context.RepositoryExplorer.CommandBar = (CommandBar)
                context.DTE.Commands.AddCommandBar( "ReposExplorer", vsCommandBarType.vsCommandBarTypePopup,
                    null, 1 );
        }

        /// <summary>
        /// Creates an "AnkhSVN" submenu on the Tools menu.
        /// </summary>
        /// <param name="context"></param>
        private static void CreateAnkhSubMenu( AnkhContext context )
        {
            CommandBar menuBar = (CommandBar)
                context.DTE.CommandBars[ "MenuBar" ];
            
            CommandBar toolMenu = 
                ((CommandBarPopup)menuBar.Controls[ "Tools" ]).CommandBar;

            context.DTE.Commands.AddCommandBar( "AnkhSVN", 
                vsCommandBarType.vsCommandBarTypeMenu,
                toolMenu, 1 );
        }
    }
}
