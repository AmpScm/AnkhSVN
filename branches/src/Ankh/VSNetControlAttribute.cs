// $Id$
using System;
using EnvDTE;
using Microsoft.Office.Core;

namespace Ankh
{
    /// <summary>
    /// An attribute used to describe where a command appears in the VS.NET IDE.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class VSNetControlAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandBar">The name of the command bar.</param>
        public VSNetControlAttribute( string commandBar )
        {	
            this.commandBar = commandBar;
        }

        /// <summary>
        /// The name of the commandbar where the command should be placed.
        /// </summary>
        public string CommandBar
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.commandBar; }
        }


        /// <summary>
        /// The position in the command bar.
        /// </summary>
        public int Position
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.position; }

            [System.Diagnostics.DebuggerStepThrough]
            set{ this.position = value; }
        }

        /// <summary>
        /// Add this control to the correct command bar.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="context"></param>
        public virtual void AddControl( ICommand cmd, IContext context, string tag )
        {
            CommandBar bar = GetCommandBar( this.commandBar, context );
            CommandBarControl cntrl = cmd.Command.AddControl( bar, this.position );      
            cntrl.Tag = tag;
        }

        /// <summary>
        /// Retrieves a command bar based on it's name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static CommandBar GetCommandBar( string name, IContext context )
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

        /// <summary>
        /// Add a control to a set of bars.
        /// </summary>
        /// <param name="baseBars"></param>
        /// <param name="cmd"></param>
        /// <param name="context"></param>
        /// <param name="tag"></param>
        protected void AddControls( string[] baseBars, ICommand cmd, 
            IContext context, string tag)
        {
            // Use each of the 
            foreach( string baseBar in baseBars )
            {
                string barName;

                // avoid a trailing period if it's to go on the base bar
                if ( this.CommandBar == String.Empty )
                    barName = baseBar;
                else
                    barName = baseBar + "." + this.CommandBar;

                CommandBar bar = 
                    VSNetControlAttribute.GetCommandBar(barName, context);
                CommandBarControl cntrl = cmd.Command.AddControl( bar, this.Position );
                cntrl.Tag = tag;
            }
        }

        private string commandBar;
        private int position;
    }
}
