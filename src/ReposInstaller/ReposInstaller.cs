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
    }
}
