using System;
using System.Configuration.Install;
using System.Configuration;
using System.Collections;
using System.ComponentModel;
using Utils;
using NSvn;

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
            string reposPath = this.Context.Parameters[ "repospath" ];
            Zip.ExtractZipResource( reposPath, this.GetType(), "ReposInstaller.repos.zip" );

            string reposUrl = "file:///" + reposPath.Replace( "\\", "/" );
            string wcPath = this.Context.Parameters[ "wcpath" ];

            RepositoryDirectory dir = new RepositoryDirectory( reposUrl );           
            dir.Checkout( wcPath, true );
        }
	}
}
