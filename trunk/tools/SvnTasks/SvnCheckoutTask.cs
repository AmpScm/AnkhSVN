using System;
using SourceForge.NAnt;
using SourceForge.NAnt.Attributes;
using NSvn;
using NSvn.Common;
using NSvn.Core;

namespace Rogue.SvnTasks
{
	/// <summary>
	/// A Nant task to check out from a SVN repository.
	/// </summary>
	[TaskName( "svncheckout" )]
	public class SvnCheckoutTask : Task
	{
        /// <summary>
        /// The local path to check out to.
        /// </summary>
        [TaskAttribute("localDir", Required = true )]
        public string LocalDir
        {
            get{ return this.localDir; }
            set{ this.localDir = value; }
        }

        /// <summary>
        /// The username to authenticate with.
        /// </summary>
        [TaskAttribute("username", Required=false)]
        public string Username
        {
            get{ return this.username; }
            set{ this.username = value; }
        }

        /// <summary>
        /// The password to authenticate with.
        /// </summary>
        [TaskAttribute("password", Required=false)]
        public string Password
        {
            get{ return this.password; }
            set{ this.password = value; }
        }

        /// <summary>
        /// The URL to check out from.
        /// </summary>
        [TaskAttribute("url", Required=true)]
        public string Url
        {
            get{ return this.url; }
            set{ this.url = value; }
        }

        /// <summary>
        /// The revision to check out - defaults to HEAD.
        /// </summary>
        [TaskAttribute("revision", Required=false)]
        public int Revision
        {
            get{ return this.revision; }
            set{ this.revision = value; }
        }

        /// <summary>
        /// The funky stuff happens here.
        /// </summary>
        protected override void ExecuteTask()
        {
            Revision revision = NSvn.Core.Revision.Head;
            if ( this.Revision != -1 )
                revision = NSvn.Core.Revision.FromNumber( this.Revision );

            RepositoryDirectory dir = new RepositoryDirectory( this.Url, revision );

            dir.Context.AddAuthenticationProvider( 
                new AuthenticationProvider( this.Username, this.Password ) );

            dir.Checkout( this.LocalDir, true );
        }

        #region class AuthenticationProvider	
        private class AuthenticationProvider : IAuthenticationProvider
        {
            public AuthenticationProvider( string username, string password )
            {
                this.username = username;
                this.password = password;
            }

            private string username = null;
            private string password = null;

            #region Implementation of IAuthenticationProvider
            public bool SaveCredentials(System.Collections.ICollection parameters)
            {
                return false;
            }

            public NSvn.Common.ICredential NextCredentials(System.Collections.ICollection parameters)
            {
                return null;
            }

            public NSvn.Common.ICredential FirstCredentials(System.Collections.ICollection parameters)
            {
                return new SimpleCredential( this.username, this.password );;
            }

            public string Kind
            {
                get
                {
                    return SimpleCredential.AuthKind;
                }
            }
            #endregion            
        }
        #endregion

        private string username = null;
        private string password = null;
        private string localDir = null;
        private string url = null;
        private int revision = -1;
	}
}
