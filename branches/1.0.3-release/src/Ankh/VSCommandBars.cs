using System;
using System.Reflection;
using EnvDTE;
using Microsoft.Office.Core;
using System.Collections;
using System.Diagnostics;

namespace Ankh
{
    /// <summary>
    /// Provides a common interface to VS command bars.
    /// </summary>
    public interface ICommandBar
    {
        string Name { get; }
        int ControlCount { get; }
    }

    /// <summary>
    /// A predicate to find a specific command bar.
    /// </summary>
    public interface ICommandBarPredicate
    {
        string Name { get; }
        bool IsMatch( ICommandBar bar );
    }


    /// <summary>
    /// A class that wraps operations on the VS command bar model, enabling us
    /// to load in both VS200[23] and VS2005
    /// </summary>
    public class VSCommandBars
    {
        public VSCommandBars( IContext context )
        {
            this.context = context;

            this.commandBarCache = new Hashtable();
        }

        public const int AddCommandBarToEnd = -1;

        public virtual Command AddNamedCommand( string commandName, 
            string text, string tooltip, ResourceBitmaps bitmapId, int status )
        {
            return context.DTE.Commands.AddNamedCommand( context.AddIn, 
                commandName, text, tooltip, false, (int) bitmapId, 
                ref this.contextGuids, status );
        }

        public object GetCommandBar( ICommandBarPredicate pred)
        {
            try
            {
                object bar = this.FindCommandBarInCache( pred );
                if ( bar != null )
                {
                    return bar;
                }

                bar = SearchForCommandBar( pred );

                if ( bar != null )
                {
                    EncacheCommandBar( pred, bar );
                }
                return bar;
            }
            catch( Exception )
            {
                // nothing to see here, move along
            }

            return null;
        }

        protected virtual object SearchForCommandBar( ICommandBarPredicate pred )
        {
            foreach ( CommandBar bar in this.context.DTE.CommandBars )
            {
                if ( pred.IsMatch( new CommandBarAdapter( bar ) ) )
                {
                    return bar;
                }
            }

            return this.context.DTE.CommandBars[ pred.Name ];
        }

        public virtual object AddCommandBar( string name, vsCommandBarType type, object parent,
            int position )
        {
            if ( position == AddCommandBarToEnd && parent != null )
            {
                position = ((CommandBar)parent).Controls.Count + 1;
            }

            return this.context.DTE.Commands.AddCommandBar(name, type, 
                (CommandBar)parent, position);
            
        }

        public virtual void SetControlToolTip( object control, string tooltip )
        {
            ((CommandBarControl)control).TooltipText = tooltip;            
        }

        public virtual void SetControlCaption( object control, string caption )
        {
            ((CommandBarControl)control).Caption = caption;
        }

        public virtual object AddControl( object command, object owner, int position )
        {
            return ((Command)command).AddControl( owner, position );
        }

        public virtual void SetControlTag( object control, string tag )
        {
            ((CommandBarControl)control).Tag = tag;
        }

        public virtual object GetBarControl( object bar, string name )
        {
            try
            {
                return ((CommandBar)bar).Controls[name];
            }
            catch( Exception )
            {
                return null;
            }
        }

        public virtual object GetPopupCommandBar( object popup )
        {
            return ((CommandBarPopup)popup).CommandBar;
        }

        public virtual object FindControl( object bar, string name )
        {
            return ((CommandBar)bar).FindControl( Type.Missing, Type.Missing, 
                name, Type.Missing, Type.Missing );
        }

        public static VSCommandBars Create( IContext context )
        {
            if ( context.DTE.Version[0] == '7' )
                return new VSCommandBars(context);
            else
                return new VSCommandBars2005(context);

        }

        private object FindCommandBarInCache( ICommandBarPredicate pred )
        {
            object bar = this.commandBarCache[ pred ];
            return bar;
        }

        private void EncacheCommandBar( ICommandBarPredicate pred, object commandBar )
        {
            this.commandBarCache[ pred ] = commandBar;
        }

        private class CommandBarAdapter : ICommandBar
        {
            public CommandBarAdapter( CommandBar bar )
            {
                this.bar = bar;
            }

            #region ICommandBar Members

            public string Name
            {
                get { return this.bar.Name; }
            }

            public int ControlCount
            {
                get { return this.bar.Controls.Count; }
            }

            #endregion

            private CommandBar bar;
        }

        /// <summary>
        /// This overrides all of the methods to work on VS2005
        /// </summary>
        private class VSCommandBars2005 : VSCommandBars
        {
            public VSCommandBars2005( IContext context ) : base(context)
            {
                this.commands = context.DTE.Commands;
                this.commandsType = context.DTE.Commands.GetType();

                this.dte = context.DTE;
                this.dteType = context.DTE.GetType();

                this.commandBars = this.dte.GetType().InvokeMember( 
                    "CommandBars", BindingFlags.GetProperty, null, 
                    context.DTE, new object[]{} );
                this.commandBarsType = this.commandBars.GetType();
            }

            public override Command AddNamedCommand(string commandName, string text, string tooltip, ResourceBitmaps bitmapId, int status)
            {
                ParameterModifier pm = new ParameterModifier( 7 );
                for ( int i = 0; i < 7; i++ )
                    pm[i] = false;

                // the 7th argument is a ref parameter
                pm[6] = true;

                return (Command)this.commandsType.InvokeMember(
                    "AddNamedCommand2", BindingFlags.InvokeMethod, null, 
                    this.commands, new object[]{
                                                          context.AddIn, 
                                                          commandName, 
                                                          text, 
                                                          tooltip, 
                                                          false,
                                                          bitmapId,
                                                          contextGuids
                    // The status parameter is not passed (yet)
                    // VS 2005 beta2 breaks with this parameter
                    // we will add it when most people have switched to final
                                                      }, 
                    new ParameterModifier[]{pm}, null, null);
            }

            protected override object SearchForCommandBar(ICommandBarPredicate pred)
            {
                try
                {
                    IEnumerable enumerable = this.commandBars as IEnumerable;
                    foreach ( object bar in enumerable )
                    {
                        if ( pred.IsMatch( new CommandBarAdapter2005( bar ) ) )
                        {
                            return bar;
                        }
                    }

                    // fall back to the "old" way
                    return this.commandBarsType.InvokeMember(
                       "Item", BindingFlags.GetProperty, null,
                       this.commandBars, new object[] { pred.Name } );     
                }
                catch( TargetInvocationException ex )
                {
                    // swallow, command bar not found
                    if ( ex.InnerException is ArgumentException )
                        return null;
                    else 
                        throw;
                }
            }

            public override object AddCommandBar(string name, 
                vsCommandBarType type, object parent, int position)
            {
                if ( position == AddCommandBarToEnd && parent != null )
                {
                    object controls = parent.GetType().InvokeMember( "Controls", 
                        BindingFlags.GetProperty, null, parent, new object[]{} );
                    position = (int)controls.GetType().InvokeMember( "Count", 
                        BindingFlags.GetProperty, null, controls, new object[]{} );
                    position = position + 1;
                }
                parent = parent == null ? Type.Missing : parent;
                object objPosition = parent == null ? Type.Missing : position;
                return this.commandsType.InvokeMember( "AddCommandBar", 
                    BindingFlags.InvokeMethod, null, this.commands,
                    new object[]{ name, type,
                                    parent, objPosition } );
            }

            public override void SetControlToolTip(object control, string tooltip)
            {
                control.GetType().InvokeMember( "TooltipText", BindingFlags.SetProperty, null, 
                    control, new object[]{ tooltip } );
            }

            public override void SetControlCaption(object control, string caption)
            {
                control.GetType().InvokeMember( "Caption", BindingFlags.SetProperty, null, 
                    control, new object[]{ caption } );
            }

            public override object AddControl(object command, object owner, int position)
            {
                return command.GetType().InvokeMember( "AddControl", 
                    BindingFlags.InvokeMethod, null, command, 
                    new object[]{ owner, position } );
            }

            public override void SetControlTag(object control, string tag)
            {
                control.GetType().InvokeMember( "Tag", 
                    BindingFlags.SetProperty, null, control, new object[]{tag} );
            }

            public override object GetBarControl(object bar, string name)
            {
                try
                {
                    return bar.GetType().InvokeMember(
                        "Controls", BindingFlags.GetProperty, null,
                        bar, new object[]{ name } );
                }
                catch( TargetInvocationException ex )
                {
                    // swallow, means the control wasn't found
                    if ( ex.InnerException is ArgumentException )
                        return null;
                    else
                        throw;
                }

            }

            public override object GetPopupCommandBar(object popup)
            {
                return popup.GetType().InvokeMember(
                    "CommandBar", BindingFlags.GetProperty, null,
                    popup, new object[]{} );
            }

            public override object FindControl(object bar, string name)
            {
                return bar.GetType().InvokeMember( "FindControl", 
                    BindingFlags.InvokeMethod, null, bar, 
                    new object[]{
                                    Type.Missing, 
                                    Type.Missing, 
                                    name, 
                                    Type.Missing, 
                                    Type.Missing } );
            }


            
            private object commands;
            private Type commandsType;
            private Type dteType;
            private _DTE dte;

            private object commandBars;
            private Type commandBarsType;

            private class CommandBarAdapter2005 : ICommandBar
            {
                public CommandBarAdapter2005( object bar )
                {
                    this.bar = bar;
                }

                #region ICommandBar Members

                public string Name
                {
                    get 
                    {

                        return this.bar.GetType().InvokeMember( "Name",
                            BindingFlags.GetProperty, null, this.bar,
                            new object[] { } ) as string;
                    }
                }

                public int ControlCount
                {
                    get 
                    {
                        object controls = this.bar.GetType().InvokeMember( "Controls",
                            BindingFlags.GetProperty, null, this.bar, new object[] { } );
                        return (int)controls.GetType().InvokeMember( "Count",
                            BindingFlags.GetProperty, null, controls, new object[] { } );
                    }
                }

                #endregion

                private object bar;
            }


            

        }

        protected object[] contextGuids = new object[] { };
        private IContext context;
        private Hashtable commandBarCache;

    }

    
    internal abstract class CommandBarPredicate
    {
        public static ICommandBarPredicate Create( string name )
        {
            if ( name == "Project" )
            {
                return new ProjectPredicate();
            }
            else
            {
                return new NormalPredicate( name );
            }
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            CommandBarPredicate other = obj as CommandBarPredicate;
            return this.Name.Equals( other.Name );
        }

        public abstract string Name { get; }

        private class NormalPredicate : CommandBarPredicate, ICommandBarPredicate
        {
            public NormalPredicate( string name )
            {
                this.name = name;
            }

            public override string Name
            {
                get { return this.name; }
            }

            public bool IsMatch( ICommandBar bar )
            {
                return bar.Name == this.name;
            }

            private string name; 
        }

        private class ProjectPredicate : CommandBarPredicate, ICommandBarPredicate
        {
            public override string Name
            {
                get { return "Project"; }
            }

            public bool IsMatch( ICommandBar bar )
            {
                // the real one has about 70, the "fake" one around 10. 42 seems like a safe bet
                return bar.Name == "Project" && bar.ControlCount > 42;
            }
        }


    }

    
}
