using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using NSvn;
using NSvn.Common;
using NSvn.Core;

namespace SvnTasks
{
	/// <summary>
	/// Base NAnt task for SVN Task.
	/// </summary>
	public abstract class SvnBaseTask : Task
	{
		protected string username = null;
		protected string password = null;
		protected string localDir = null;
		protected string url = null;
		protected int revision = -1;

        /// <summary>
        /// The local path to check out to.
        /// </summary>
        [TaskAttribute("localDir", Required = false )]
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
        [TaskAttribute("url", Required=false)]
        public virtual string Url
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
		public void Test()
		{
			this.ExecuteTask();
		}

		protected SimpleCredential SimplePrompt(string realm, string password)
		{
			return new SimpleCredential(this.Username, this.Password);
		}
		#region class Context	
		protected class Context : NSvnContext
		{
			private string logPrefix;
			private Task parent;
			private string message;

			public Context(string logPrefix, Task parent, string message)
			{
				this.logPrefix = logPrefix;
				this.parent = parent;
				this.message = message;
			}
			protected override void NotifyCallback(NSvn.Core.Notification notification)
			{
				this.parent.Log(Level.Info, "{0} {1} {2}", 
					this.logPrefix, this.message, notification.Path + Environment.NewLine);
			}
		}
		#endregion

	}
}
