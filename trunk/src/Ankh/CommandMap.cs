// $Id$
using System;
using System.Collections;
using System.Reflection;
using EnvDTE;
using System.Diagnostics;
using Microsoft.Office.Core;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Ankh
{
    /// <summary>
    /// Responsible for registering the ICommand implementations in this assembly.
    /// </summary>
    public class CommandMap : DictionaryBase
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
        public static CommandMap LoadCommands( IContext context, bool register )
        {
            // change the culture, so we don't have to deal with localized names
            // for command bars
            CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo( "en-US", false );
            try
            {
                CreatePopups( context );
                CreateAnkhSubMenu( context );

                return AddCommands(context, register); 
            }
            finally
            {
                // restore the old culture
                System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        private static CommandMap AddCommands(IContext context, bool register)
        {
            CommandMap commandMap = new CommandMap();
            ArrayList commandControlList = new ArrayList();

            foreach (Module module in typeof(CommandMap).Assembly.GetModules(false)) {
                foreach (Type type in module.FindTypes(new TypeFilter(CommandMap.CommandTypeFilter), null)) {
                    VSNetCommandAttribute[] vsattrs =
                        (VSNetCommandAttribute[])(type.GetCustomAttributes(typeof(VSNetCommandAttribute), false));
                    if (vsattrs.Length > 0) {
                        ICommand cmd = (ICommand)Activator.CreateInstance(type);
                        commandMap.Dictionary[context.AddIn.ProgID + "." + vsattrs[0].Name] = cmd;

                        if (register) {
                            RegisterVSNetCommand(vsattrs[0], cmd, context);
                            foreach( VSNetControlAttribute control in cmd.GetType().GetCustomAttributes( 
                                typeof(VSNetControlAttribute), false) )
                            {
                                commandControlList.Add(new CommandControl(cmd, control));
                            }
                        }
                    }
                }
            }

            if (register) 
            {
                commandControlList.Sort();

                foreach (CommandControl commandControl in commandControlList)
                    RegisterControl(commandControl.Command, commandControl.Control, context);
            }

            return commandMap;
        }


        private class CommandControl : IComparable
        {
            public ICommand Command;
            public VSNetControlAttribute Control;

            public CommandControl(ICommand command, VSNetControlAttribute control)
            {
                Command = command;
                Control = control;
            }

            public int CompareTo( object obj )
            {
                CommandControl other = obj as CommandControl;
                if ( other == null )
                {
                    return -1;
                }

                return Comparer.Default.Compare( this.Control.Position, other.Control.Position );
            }
        }

        /// <summary>
        /// Get rid of any old commands hanging around.
        /// </summary>
        public static void DeleteCommands( IContext context )
        {
           
            if ( context.DTE.Commands != null )
            {
                // we only want to delete our own commands
                foreach( Command cmd in context.DTE.Commands )
                {
                    try
                    {
						if (cmd.Name != null && cmd.Name.StartsWith(context.AddIn.ProgID))
						{
							Debug.Write( "Deleting command " + cmd.Name + ".", "Ankh" );
							cmd.Delete();
                            Debug.WriteLine( "Deleted!", "Ankh" );
						}
                    }
                    catch( System.IO.FileNotFoundException fex )
                    {
                        Trace.WriteLine( "FileNotFoundException thrown in DeleteCommands:" 
                            + fex.Message );
                        // swallow
                        // HACK: find out why FileNotFoundException is thrown
                    }
                    catch( COMException cex )
                    {
                        Trace.WriteLine( "COMExceptionthrown in DeleteCommands:" + 
                            cex.Message );
                    }
                    catch( Exception ex )
                    {
                        Trace.WriteLine( "Exception thrown in DeleteCommands:" + 
                            ex.Message );
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
            ICommand cmd, IContext context )
        {
            cmd.Command = context.CommandBars.AddNamedCommand(
                attr.Name, attr.Text, attr.Tooltip, attr.Bitmap, 0 );
        }

        /// <summary>
        /// Registers a commandbar. 
        /// </summary>
        /// <param name="command">The ICommand to attach the command bar to.</param>
        /// <param name="type">The type that handles the command.</param>
        private static void RegisterControl( ICommand cmd, VSNetControlAttribute control, IContext context )
        {
            // get the actual name of the command
            string name = ((VSNetCommandAttribute)cmd.GetType().GetCustomAttributes(
                typeof(VSNetCommandAttribute), false )[0]).Name;

            control.AddControl( cmd, context, control.CommandBar + "." + name );
        }

        private static void CreatePopups( IContext context )
        {
            context.RepositoryExplorer.CommandBar = 
                context.CommandBars.AddCommandBar( "ReposExplorer", 
                    vsCommandBarType.vsCommandBarTypePopup, null, 
                    VSCommandBars.AddCommandBarToEnd );
        
            context.WorkingCopyExplorer.ContextMenu = new CommandBarContextMenu(
                context.CommandBars.AddCommandBar( "WorkingCopyExplorer", 
                    vsCommandBarType.vsCommandBarTypePopup, null, 
                    VSCommandBars.AddCommandBarToEnd ));
        }

        /// <summary>
        /// Creates an "AnkhSVN" submenu on the Tools menu.
        /// </summary>
        /// <param name="context"></param>
        private static void CreateAnkhSubMenu( IContext context )
        {
            object toolMenu = context.CommandBars.GetCommandBar( "Tools" );

            // check that the menu isn't already there (only necessary in 2005)
            object ankhMenu = context.CommandBars.GetBarControl( toolMenu, "AnkhSVN" );
            if ( ankhMenu == null )
            {
                context.CommandBars.AddCommandBar( "An&khSVN", 
                    vsCommandBarType.vsCommandBarTypeMenu, toolMenu, 1 );
            }
        }
    }
}
