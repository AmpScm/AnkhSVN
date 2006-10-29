using System;
using System.Collections.Generic;
using System.Text;
using Meebey.SmartIrc4net;
using System.Reflection;

namespace AnkhBot
{
    class CommandDispatcher
    {
        public CommandDispatcher( AnkhBot bot, Assembly[] assemblies )
        {
            this.bot = bot;
            this.assemblies = assemblies;
            this.FindCommands();
        }

        public CommandDispatcher( AnkhBot bot )
            :
            this( bot, new Assembly[] { typeof( CommandDispatcher ).Assembly } )
        {
        }


        public void Dispatch( string line, string user, string channel, bool isPm )
        {
            if (!line.StartsWith( CommandPrefix ))
                return;
            int space = line.IndexOf( ' ' );
            string commandString = space >= 0 ?
                line.Substring( CommandPrefix.Length, space - CommandPrefix.Length ) :
                line.Substring( CommandPrefix.Length, line.Length - CommandPrefix.Length );

            string argString = line.Substring( CommandPrefix.Length + commandString.Length );
            string[] args = new List<string>( new ArgSplitter( argString ) ).ToArray();

            ICommand command = this.FindCommand( commandString );
            if (command != null)
                command.Execute( new CommandArgs( bot, args, user, channel, isPm ) );

        }

        private ICommand FindCommand( string commandString )
        {
            if (this.mapping.ContainsKey( commandString ))
                return this.mapping[commandString];
            else
                return null;
        }

        private void FindCommands()
        {
            this.mapping = new Dictionary<string, ICommand>();

            foreach (Assembly asm in this.assemblies)
            {
                foreach (Module module in asm.GetModules( false ))
                {
                    foreach (Type type in module.FindTypes(
                        delegate( Type t, object obj )
                        {
                            return typeof( ICommand ).IsAssignableFrom( t ) && !t.IsAbstract &&
                                t.GetCustomAttributes( typeof( CommandAttribute ), false ).Length > 0;
                        }, null ))
                    {
                        CommandAttribute attr = (CommandAttribute)
                            type.GetCustomAttributes( typeof( CommandAttribute ), false )[0];
                        ICommand command = (ICommand)Activator.CreateInstance( type );
                        mapping[attr.Command] = command;
                    }
                }
            }
        }




        private Assembly[] assemblies;
        private Dictionary<string, ICommand> mapping;
        private AnkhBot bot;
        private const string CommandPrefix = ".";
    }
}
