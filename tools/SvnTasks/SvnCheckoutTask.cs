using System;
using NAnt.Core;
using NAnt.Core.Attributes;
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
            try
            {
                Revision revision = NSvn.Core.Revision.Head;
                if ( this.Revision != -1 )
                    revision = NSvn.Core.Revision.FromNumber( this.Revision );

                RepositoryDirectory dir = new RepositoryDirectory( this.Url, revision );
				dir.Context.AddAuthenticationProvider( AuthenticationProvider.GetUsernameProvider() );
				dir.Context.AddAuthenticationProvider( AuthenticationProvider.GetSimpleProvider() );
				
                if( this.Verbose )
                    dir.Context = new Context( this.LogPrefix, this );

                if ( this.Username != null && this.Password != null )
                {
					dir.Context.AddAuthenticationProvider( AuthenticationProvider.GetSimplePromptProvider(
						new SimplePromptDelegate( this.SimplePrompt ), 1 ) );
				}

                dir.Checkout( this.LocalDir, true );
            }
            catch( AuthorizationFailedException )
            {
                throw new BuildException( "Unable to authorize against the repository." );
            }
            catch( SvnException ex )
            {
                throw new BuildException( "Unable to check out: " + ex.Message );
            }
            catch( Exception ex )
            {
                throw new BuildException( "Unexpected error: " + ex.Message );
            }
        }

		private SimpleCredential SimplePrompt( string realm, string password )
		{
			return new SimpleCredential( this.Username, this.Password );
		}

        #region class Context	
        private class Context : NSvnContext
        {
            public Context( string logPrefix, SvnCheckoutTask parent )
            {
                this.logPrefix = logPrefix;
				this.parent = parent;
            }

            protected override void NotifyCallback(NSvn.Core.Notification notification)
            {
                this.parent.Log( Level.Verbose, "{0}Checked out {1}", this.logPrefix, notification.Path + Environment.NewLine);
            }

            private string logPrefix;
			private SvnCheckoutTask parent;
        }
        #endregion

        private string username = null;
        private string password = null;
        private string localDir = null;
        private string url = null;
        private int revision = -1;
	}
}
