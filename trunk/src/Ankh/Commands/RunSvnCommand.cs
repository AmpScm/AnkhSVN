using EnvDTE;
using System.Diagnostics;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Utils;

namespace Ankh.Commands
{
    /// <summary>
    /// A command used to run svn.exe directly from the VS.NET command window.
    /// This command should be aliased as "svn".
    /// </summary>
    [VSNetCommand("RunSvn")]
    internal class RunSvnCommand : CommandBase
    {
        public RunSvnCommand()
        {
            this.workingDirectory = Environment.CurrentDirectory;
        }

        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            return Enabled;
        }
       
        public override void Execute(AnkhContext context, string parameters)
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
            if ( !INTRINSIC.IsMatch( parameters ) )
                return null;

            string output = "";
            string intrinsic = INTRINSIC.Match( parameters ).ToString();
            switch( intrinsic )
            {
                case "/?":
                    output = Usage();
                    break;
                case "pwd":
                    output = WorkingDirectory();
                    break;
                case "cd":
                    output = ChangeWorkingDirectory( parameters );
                    break;
                case "dir":
                    output = DirectoryListing();
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
        /// Change the internal working directory.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>The new path.</returns>
        private string ChangeWorkingDirectory( string parameters )
        {
            // get the path portion
            int firstBlank = parameters.IndexOf( ' ' );
            if ( firstBlank < 0 )
                return CDUsage();

            string path = parameters.Substring( firstBlank + 1 );

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
            return this.RunCommand( "cmd.exe", "/k dir " + this.workingDirectory );
        }

        /// <summary>
        /// Usage string for svn cd.
        /// </summary>
        /// <returns></returns>
        private string CDUsage()
        {
            return "svn cd DIR";
        }

        private AnkhContext context;
        private string workingDirectory;
        private readonly Regex INTRINSIC =  new Regex( @"\/\?|pwd|cd|dir" );
    }
}
