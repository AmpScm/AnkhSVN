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
            string reposPath = Context.Parameters[ "repospath" ];
            string wcPath = Context.Parameters[ "wcpath" ];
            


//            if (Directory.Exists( reposPath) )
//                System.Windows.Forms.MessageBox.Show(" Deleting: " + reposPath);
//
//                Directory.Delete(reposPath, true);
//
//            if (Directory.Exists( wcPath ) )
//                System.Windows.Forms.MessageBox.Show(" Deleting: " + wcPath );
//                Directory.Delete( wcPath, true );


            Zip.ExtractZipResource( reposPath, this.GetType(), "ReposInstaller.repos.zip" );
            System.Windows.Forms.MessageBox.Show( "Directory exisists:" 
                + Directory.Exists( reposPath ).ToString() );
            reposPath = Path.Combine( reposPath, "trunk" );
            string reposUrl = "file:///" + reposPath.Replace( "\\", "/" ).Replace(" ", "%20" );
            
            System.Windows.Forms.MessageBox.Show( "reposPath: " + reposPath + Environment.NewLine
                + "wcPath: " + wcPath + Environment.NewLine + "reposUrl:" + reposUrl);
            
            RepositoryDirectory dir = new RepositoryDirectory( reposUrl );           
            dir.Checkout( wcPath, true );
        }
	}
}
