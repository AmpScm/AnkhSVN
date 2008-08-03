using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace AnkhBot
{
    public class CommandBase : ICommand
    {
        public CommandBase()
        {
            this.FindSubCommands();
        }


        public void Execute( CommandArgs args )
        {
            MethodInfo info = this.GetSubCommand( args.Args );
            if (info != null)
            {
                info.Invoke( this, new object[] { args } );
            }
            else
            {
                this.NoSubCommand( args );
            }
        }

        protected virtual void NoSubCommand( CommandArgs args )
        {

        }

        private MethodInfo GetSubCommand( string[] args )
        {
            if (args.Length > 0 && this.methodMap.ContainsKey( args[0] ))
                return this.methodMap[args[0]];
            else
                return null;
        }

        private void FindSubCommands()
        {
            this.methodMap = new Dictionary<string, MethodInfo>();
            foreach (MethodInfo method in this.GetType().GetMethods(
                                BindingFlags.Instance | BindingFlags.NonPublic ))
            {
                object[] attrs = method.GetCustomAttributes( typeof( SubCommandAttribute ), false );
                if (attrs.Length > 0)
                {
                    SubCommandAttribute subcommand = (SubCommandAttribute)attrs[0];
                    this.methodMap[subcommand.Name] = method;
                }
            }
        }

        private Dictionary<string, MethodInfo> methodMap;
    }
}
