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
            folderBrowser.OnlyFilesystem = true;

            if ( folderBrowser.ShowDialog() == DialogResult.Cancel )
                return;

            string reposPath = folderBrowser.DirectoryPath;
            Zip.ExtractZipResource( reposPath, this.GetType(), "ReposInstaller.repos.zip" );
            state["repospath"] = reposPath;

            if ( MessageBox.Show( "Do you want to check out a test working copy from the " + 
                "repository?", "Working copy", MessageBoxButtons.YesNo ) == DialogResult.No )
                return;

            if ( folderBrowser.ShowDialog() == DialogResult.Cancel )
                return;

            string wcPath = folderBrowser.DirectoryPath;

            string reposUrl = Path.Combine( reposPath, "trunk" );
            reposUrl = "file:///" + reposPath.Replace( "\\", "/" ).Replace(" ", "%20" );

            RepositoryDirectory dir = new RepositoryDirectory( reposUrl );           
            dir.Checkout( wcPath, true );

            state[ "wcpath" ] = wcPath;
        }
	}
}
