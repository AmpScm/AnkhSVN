using System;
using System.Reflection;
using EnvDTE;
using Microsoft.Office.Core;

namespace Ankh
{
    /// <summary>
    /// A class that wraps operations on the VS command bar model, enabling us
    /// to load in both VS200[23] and VS2005
    /// </summary>
    public class VSCommandBars
    {
        public VSCommandBars( IContext context )
        {
            this.context = context;
        }

        public const int AddCommandBarToEnd = -1;

        public virtual Command AddNamedCommand( string commandName, 
            string text, string tooltip, int bitmapId, int status )
        {
            return context.DTE.Commands.AddNamedCommand( context.AddIn, 
                commandName, text, tooltip, false, bitmapId, 
                ref this.contextGuids, status );
        }

        public virtual object GetCommandBar( string name )
        {
            try
            {
                return this.context.DTE.CommandBars[name];
            }
            catch( Exception )
            {
                return null;
            }
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

            public override Command AddNamedCommand(string commandName, string text, string tooltip, int bitmapId, int status)
            {
                ParameterModifier pm = new ParameterModifier( 8 );
                for ( int i = 0; i < 8; i++ )
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
                                                          true,
                                                          null,
                                                          contextGuids
                                                      }, 
                    new ParameterModifier[]{pm}, null, null);
            }

            public override object GetCommandBar(string name)
            {
                try
                {
                    return this.commandBarsType.InvokeMember(
                        "Item", BindingFlags.GetProperty, null, 
                        this.commandBars, new object[]{name} );                
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
            

        }

        protected object[] contextGuids = new object[] { };
        private IContext context;
    }

    
}
