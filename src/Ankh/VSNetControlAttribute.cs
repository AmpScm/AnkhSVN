// $Id$
using System;
using EnvDTE;
//using Microsoft.Office.Core;
using System.Reflection;
using AnkhSvn.Ids;
using System.Diagnostics;

namespace Ankh
{
    /// <summary>
    /// An attribute used to describe where a command appears in the VS.NET IDE.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class VSNetControlAttribute : Attribute
    {
		readonly AnkhCommand _command;
        public const string AnkhSubMenu = "An&kh";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="commandBar">The name of the command bar.</param>
		public VSNetControlAttribute(string commandBar)
		{
			this.commandBar = commandBar;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandBar">The name of the command bar.</param>
        public VSNetControlAttribute(AnkhCommand command, string commandBar )
        {
			_command = command;
            this.commandBar = commandBar;
        }

		public AnkhCommand Command
		{
			[DebuggerStepThrough]
			get { return _command; }
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
            /*object bar = GetCommandBar( this.commandBar, context );
            if ( bar != null )
            {
                object cntrl = context.CommandBars.AddControl(cmd.Command, bar, 1);
                context.CommandBars.SetControlTag(cntrl, tag);
            }*/
            
        }

        /// <summary>
        /// Retrieves a command bar based on it's name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static object GetCommandBar( string name, IContext context )
        {
            /*string[] path = name.Split( '.' );
            object bar;

            //TODO: is this really necessary?
            if ( path[ 0 ] == "ReposExplorer" )
                bar = context.RepositoryExplorer.CommandBar;
            else if ( path[ 0 ] == "WorkingCopyExplorer" )
                bar = ((CommandBarContextMenu)context.WorkingCopyExplorer.ContextMenu).CommandBar;
            else
                bar = context.CommandBars.GetCommandBar( CommandBarPredicate.Create(path[ 0 ]) );

            if ( bar == null )
                return null;

            for( int i = 1; i < path.Length; i++ )
            {
            
                // does this command bar already exist?
                try
                {
                    object ctrl = context.CommandBars.GetBarControl(bar, path[i]);

                    if (ctrl != null)
                        bar = context.CommandBars.GetPopupCommandBar(ctrl);
                    else
                        context.CommandBars.AddCommandBar(path[i],
                            vsCommandBarType.vsCommandBarTypeMenu, bar,
                            VSCommandBars.AddCommandBarToEnd);
                }
                catch ( Exception )
                {
                    context.CommandBars.AddCommandBar(path[i],
                            vsCommandBarType.vsCommandBarTypeMenu, bar,
                            VSCommandBars.AddCommandBarToEnd);
                }
                            
            }

            return bar;*/
            return null;
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
            /*// Use each of the 
            foreach( string baseBar in baseBars )
            {
                string barName;

                // avoid a trailing period if it's to go on the base bar
                if ( this.CommandBar == String.Empty )
                    barName = baseBar;
                else
                    barName = baseBar + "." + this.CommandBar;

                object bar = 
                    VSNetControlAttribute.GetCommandBar(barName, context);
                if ( bar != null )
                {
                    object cntrl = context.CommandBars.AddControl( cmd.Command, bar, 
                        1 );
                    context.CommandBars.SetControlTag( cntrl, tag );
                }
            }*/
        }

        private string commandBar;
        private int position;
    }
}
