using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AnkhSvn.Ids;
using EnvDTE;
using Utils;

namespace Ankh.Commands
{
    /// <summary>
    /// A command used to run svn.exe directly from the VS.NET command window.
    /// This command should be aliased as "svn".
    /// </summary>
	[Command(AnkhCommand.RunSvnCommand)]
    public class RunSvnCommand : CommandBase
    {
        public RunSvnCommand()
        {
            this.workingDirectory = Environment.CurrentDirectory;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.GetService<IContext>();
            EnvDTE._DTE dte = e.GetService<EnvDTE._DTE>(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));

            this.context = context;
            this.window = (CommandWindow)((Window)dte.Windows.Item( 
                EnvDTE.Constants.vsWindowKindCommandWindow )).Object;

            // is it one of the intrinsic commands?
            if ( !ParseIntrinsicCommand( e.Argument as string ) )
                this.RunCommand( this.SvnExePath, e.Argument as string );
        }

        private string SvnExePath
        {
            get
            {
                // use the path in the config file if it's specified
                // if not, just try to find it in PATH
                string path = this.context.Configuration.Instance.Subversion.SvnExePath;
                return path != null ? path : "svn.exe";
            }
        }

        private bool ParseIntrinsicCommand( string parameters )
        {
            string[] args = this.ParseArguments( parameters );

            if ( args.Length < 1 )
                return false;

            bool retval = true;
            switch( args[0] )
            {
                case "/?":
                    this.Usage();
                    break;
                case "pwd":
                    this.WorkingDirectory();
                    break;
                case "cd":
                    this.ChangeWorkingDirectory( args );
                    break;
                case "dir":
                    this.DirectoryListing();
                    break;
                default:
                    retval = false;
                    break;
            }

            return retval;         
        }

        /// <summary>
        /// Runs a command.
        /// </summary>
        /// <param name="command">The command to run.</param>
        /// <param name="parameters">The parameters to use.</param>
        /// <returns>The output from the command.</returns>
        private void RunCommand(string command, string parameters)
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
                this.window.OutputString( String.Format( 
                    "Unable to launch {0}.{1}{1}",
                    command, Environment.NewLine ) );
                return;
            }
            ProcessReader stdout = new ProcessReader( process.StandardOutput );
            ProcessReader stderr = new ProcessReader( process.StandardError );

            stdout.Start();
            stderr.Start();

            while( !(process.HasExited && stdout.HasExited && stderr.HasExited && stdout.Empty &&
                stderr.Empty ) )
            {
                int idx = WaitHandle.WaitAny( new WaitHandle[]{ 
                                                                  stdout.WaitHandle,
                                                                  stderr.WaitHandle 
                                                              }, 20, false );
                if ( idx == 0 )
                {
                   window.OutputString( stdout.ReadLine() + "\r\n" );
                } 
                else if ( idx == 1 )                
                {
                    window.OutputString( stderr.ReadLine() + "\r\n" );
                }
                
            }
        }

        /// <summary>
        /// Returns a usage string.
        /// </summary>
        /// <returns></returns>
        private void Usage()
        {
            this.RunCommand( this.SvnExePath, "help" );
            this.window.OutputString( 
@"In addition to the above, the following metacommands are available:
/?      This message
pwd     Print working directory
cd DIR  Change working directory
dir     List the contents of the working directory
" );
        }
            
        private void WorkingDirectory()
        {
            this.window.OutputString( this.workingDirectory + "\r\n" );
        }


        /// <summary>
        /// Change the public working directory.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>The new path.</returns>
        private void ChangeWorkingDirectory( string[] parameters )
        {
            if ( parameters.Length <= 1 )
                this.window.OutputString( "Invalid path\r\n" );

            string path = string.Join( " ", parameters, 1, parameters.Length-1 );

            // absolute or relative?
            if ( !PathUtils.IsPathAbsolute( path ) )
                path = Path.GetFullPath( Path.Combine( this.workingDirectory, path ) );

            if ( Directory.Exists( path ) )
            {
                this.workingDirectory = path;            
                this.window.OutputString( this.workingDirectory + "\r\n" );
            }
            else
                this.window.OutputString( "Invalid path.\r\n" );
        }

        /// <summary>
        /// List the current directory.
        /// </summary>
        /// <returns>A list of the directory.</returns>
        private void DirectoryListing()
        {
            this.RunCommand( "cmd.exe", "/k dir \"" + 
                this.workingDirectory + "\"" );
        }

        /// <summary>
        /// Usage string for svn cd.
        /// </summary>
        /// <returns></returns>
        private void CDUsage()
        {
            this.window.OutputString( "svn cd DIR\r\n" );
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

        private CommandWindow window;
        private IContext context;
        private string workingDirectory;
        private readonly Regex INTRINSIC =  new Regex( @"\/\?|pwd|cd|dir" );
    }
}
