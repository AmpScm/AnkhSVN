using EnvDTE;
using System.Diagnostics;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Utils;

namespace Ankh.Commands
{
    /// <summary>
    /// A command used to run svn.exe directly from the VS.NET command window.
    /// This command should be aliased as "svn".
    /// </summary>
    [VSNetCommand("RunSvn")]
    public class RunSvnCommand : CommandBase
    {
        public RunSvnCommand()
        {
            this.workingDirectory = Environment.CurrentDirectory;
        }

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            return Enabled;
        }
       
        public override void Execute(IContext context, string parameters)
        {
            CommandWindow window = (CommandWindow)((Window)context.DTE.Windows.Item( 
                EnvDTE.Constants.vsWindowKindCommandWindow )).Object;

            this.context = context;

            // is it one of the intrinsic commands?
            string output = ParseIntrinsicCommand( parameters );
            if ( output == null )
                output = RunCommand( this.SvnExePath, parameters );

            window.OutputString( output );
        }

        private string SvnExePath
        {
            get
            {
                // use the path in the config file if it's specified
                // if not, just try to find it in PATH
                string path = this.context.Config.Subversion.SvnExePath;
                return path != null ? path : "svn.exe";
            }
        }

        private string ParseIntrinsicCommand( string parameters )
        {
            string[] args = this.ParseArguments( parameters );

            if ( args.Length < 1 )
                return null;

            string output;
            switch( args[0] )
            {
                case "/?":
                    output = Usage();
                    break;
                case "pwd":
                    output = WorkingDirectory();
                    break;
                case "cd":
                    output = ChangeWorkingDirectory( args );
                    break;
                case "dir":
                    output = DirectoryListing();
                    break;
                default:
                    output = null;
                    break;
            }

            return output;         
        }

        /// <summary>
        /// Runs a command.
        /// </summary>
        /// <param name="command">The command to run.</param>
        /// <param name="parameters">The parameters to use.</param>
        /// <returns>The output from the command.</returns>
        private string RunCommand(string command, string parameters)
        {
            ProcessStartInfo info = new ProcessStartInfo( command, parameters );
            info.CreateNoWindow = true;
            info.ErrorDialog = false;
            info.WorkingDirectory = this.workingDirectory;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.Start( info );
            }
            catch( System.ComponentModel.Win32Exception )
            {
                return String.Format( 
                    "Unable to launch {0}.{1}{1}",
                    command, Environment.NewLine );
            }
            ProcessReader stdout = new ProcessReader( process.StandardOutput );
            ProcessReader stderr = new ProcessReader( process.StandardError );

            stdout.Start();
            stderr.Start();

            process.WaitForExit();

            stdout.Wait();
            stderr.Wait();

            return stdout.Output + stderr.Output;
        }

        /// <summary>
        /// Returns a usage string.
        /// </summary>
        /// <returns></returns>
        private string Usage()
        {
            string usage = RunCommand( this.SvnExePath, "help" );
            usage += 
@"In addition to the above, the following metacommands are available:
/?      This message
pwd     Print working directory
cd DIR  Change working directory
dir     List the contents of the working directory
";
            return usage;
        }
            
        private string WorkingDirectory()
        {
            return this.workingDirectory;
        }


        /// <summary>
        /// Change the public working directory.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>The new path.</returns>
        private string ChangeWorkingDirectory( string[] parameters )
        {
            if ( parameters.Length <= 1 )
                return "Invalid path";

            string path = string.Join( " ", parameters, 1, parameters.Length-1 );

            // absolute or relative?
            if ( !Path.IsPathRooted( path ) )
                path = Path.GetFullPath( Path.Combine( this.workingDirectory, path ) );

            if ( Directory.Exists( path ) )
            {
                this.workingDirectory = path;            
                return this.workingDirectory;
            }
            else
                return "Invalid path.";
        }

        /// <summary>
        /// List the current directory.
        /// </summary>
        /// <returns>A list of the directory.</returns>
        private string DirectoryListing()
        {
            return this.RunCommand( "cmd.exe", "/k dir \"" + 
                this.workingDirectory + "\"" );
        }

        /// <summary>
        /// Usage string for svn cd.
        /// </summary>
        /// <returns></returns>
        private string CDUsage()
        {
            return "svn cd DIR";
        }

        /// <summary>
        /// Splits the argument list into a string array, according to the 
        /// " and ' quotes.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private string[] ParseArguments( string args )
        {
            bool inSingleQuote = false, inQuote = false;
            StringBuilder builder = new StringBuilder();
            ArrayList arglist = new ArrayList();

            foreach( char c in args )
            {
                if ( c == '\'' )
                {
                    if ( inSingleQuote )
                    {
                        arglist.Add( builder.ToString() );
                        builder.Length = 0;
                        inSingleQuote = true;
                    }
                    else
                        inSingleQuote = true;
                }
                else if ( c== '\"' )
                {
                    if ( inQuote )
                    {
                        arglist.Add( builder.ToString() );
                        builder.Length = 0;
                        inQuote = true;
                    }
                    else
                        inQuote = true;
                }
                else if ( Char.IsWhiteSpace( c ) && !inQuote && !inSingleQuote )
                {
                    if ( builder.Length > 0 )
                    {
                        arglist.Add( builder.ToString() );
                        builder.Length = 0;
                    }
                }
                else
                    builder.Append( c );
            }
            if ( builder.Length > 0 )
                arglist.Add( builder.ToString() );

            return (string[])arglist.ToArray( typeof(String) );
        }

        private IContext context;
        private string workingDirectory;
        private readonly Regex INTRINSIC =  new Regex( @"\/\?|pwd|cd|dir" );
    }
}
