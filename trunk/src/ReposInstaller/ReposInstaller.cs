using System;
using System.Configuration.Install;
using System.Configuration;
using System.Collections;
using System.ComponentModel;
using Utils;
using NSvn;
using System.IO;
using System.Windows.Forms;
using System.Web;
using EnvDTE;

namespace ReposInstaller
{
	/// <summary>
	/// Installs a repos and extracts a working copy from it.
	/// </summary>
	[RunInstaller(true)]
	public class ReposInstaller : Installer
	{
		public ReposInstaller()
		{			
		}

		public override void Install( IDictionary state )
		{

			if ( MessageBox.Show( "Do you want to extract a test repository?", "Test repository",
				MessageBoxButtons.YesNo ) == DialogResult.No )
				return;

			FolderBrowser folderBrowser = new FolderBrowser();
			folderBrowser.StartLocation = FolderBrowser.FolderID.MyComputer;
			folderBrowser.OnlyFilesystem = true;

			if ( folderBrowser.ShowDialog() == DialogResult.Cancel )
				return;

			string reposPath = Path.Combine( folderBrowser.DirectoryPath, "repos" );
            
			if ( Directory.Exists( reposPath  ) )
				this.RecursiveDelete( reposPath );

			Zip.ExtractZipResource( reposPath, this.GetType(), "ReposInstaller.repos.zip" );
			state["repospath"] = reposPath;

			if ( MessageBox.Show( "Do you want to check out a test working copy from the " + 
				"repository?", "Working copy", MessageBoxButtons.YesNo ) == DialogResult.No )
				return;

			if ( folderBrowser.ShowDialog() == DialogResult.Cancel )
				return;

			string wcPath = Path.Combine( folderBrowser.DirectoryPath, "wc" );
			if ( Directory.Exists( wcPath ) )
				this.RecursiveDelete( wcPath );

			string reposUrl = Path.Combine( reposPath, "trunk" );
			reposUrl = "file:///" + reposUrl.Replace( "\\", "/" ).Replace(" ", "%20" );

			RepositoryDirectory dir = new RepositoryDirectory( reposUrl );           
			dir.Checkout( wcPath, true );

			state[ "wcpath" ] = wcPath;
		}

		public override void Rollback(System.Collections.IDictionary savedState)
		{
			base.Rollback( savedState );

			string reposPath = (string)savedState[ "repospath" ];
			string wcPath = (string)savedState[ "wcpath" ];

			if ( reposPath != null && Directory.Exists( reposPath ) )
				this.RecursiveDelete( reposPath );
			if ( wcPath != null && Directory.Exists( wcPath ) )
				this.RecursiveDelete( wcPath );        
		}   

		public override void Uninstall(IDictionary savedState)
		{
			try
			{
			
				//base.Uninstall( savedState );

				// make sure VS.NET is closed down
				while( this.VSIsRunningRunning() )
				{
					MessageBox.Show( "One or more instances of VS.NET are running. " + 
						"Please close these before continuing", "VS.NET is running", MessageBoxButtons.OK,
						MessageBoxIcon.Warning );
				} 
          
				MessageBox.Show( "Hello 2" );

				// delete the commands
				this.DeleteAnkhCommands( "VisualStudio.DTE.7" );
				this.DeleteAnkhCommands( "VisualStudio.DTE.7.1" );
			}
			catch( Exception )
			{
				MessageBox.Show( "An error occurred during uninstallation of " + 
					"Ankh VS.NET commands. \r\nThey might still be present. " + 
					"Run devenv /setup from the command line to reset your VS.NET installation",
					"Error", MessageBoxButtons.OK,
					MessageBoxIcon.Error );

				// swallow
			}
            
		}

		/// <summary>
		/// Deletes the Ankh commands associated with a specific VS.NET version
		/// </summary>
		/// <param name="progid"></param>
		private void DeleteAnkhCommands( string progid )
		{
			Type t = Type.GetTypeFromProgID( progid );
			if ( t == null )
				return;

			_DTE dte = (_DTE)Activator.CreateInstance( t );

			// find our commands and delete them           
			foreach( Command cmd in dte.Commands )
			{
				try
				{
					if ( cmd.Name.StartsWith( PROGID ) )
						cmd.Delete();
				}
				catch( Exception )
				{
					// HACK: swallow
				}
			}
		}

		private bool VSIsRunningRunning()
		{ 
			System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
			foreach( System.Diagnostics.Process process in processes )
			{
				try
				{
					if ( process != null && process.MainModule != null )
					{
						if ( process.MainModule.FileName.EndsWith( "devenv.exe" ) )
							return true;
					}
				}
				catch( Exception )
				{
					// HACK: swallow
				}
			}
			return false;
		}

        
		private void RecursiveDelete( string path )
		{
			foreach( string dir in Directory.GetDirectories( path ) )
			{
				this.RecursiveDelete( dir );
			}

			foreach( string file in Directory.GetFiles( path ) )
				File.SetAttributes( file, FileAttributes.Normal );

			File.SetAttributes( path, FileAttributes.Normal );
			Directory.Delete( path, true );
		}

		private string PROGID="Ankh";
	}
}
