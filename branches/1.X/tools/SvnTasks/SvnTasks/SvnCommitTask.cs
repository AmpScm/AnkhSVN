using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using NSvn;
using NSvn.Common;
using NSvn.Core;

namespace SvnTasks
{
	/// <summary>
	/// A Nant task to commit a file to a SVN repository.
	/// </summary>
	[TaskName( "svncommit" )]
	public class SvnCommitTask : SvnBaseTask
	{
		protected bool m_recursive  = false;
		protected string m_logMessage = String.Empty;

		[TaskAttribute("recursive", Required=false)]
		public bool Recursive
		{
			get
			{
				return m_recursive;
			}
			set
			{
				m_recursive = value;
			}
		}
		[TaskAttribute("logmessage", Required=false)]
		public string LogMessage
		{
			get
			{
				return m_logMessage;
			}
			set
			{
				m_logMessage = value;
			}
		}

		/// <summary>
		/// The funky stuff happens here.
		/// </summary>
		protected override void ExecuteTask()
		{
			this.Log(Level.Info, "{0} {1}",this.Name, this.LocalDir);
			
			try
			{
				Revision revision = NSvn.Core.Revision.Head;
				if ( this.Revision != -1 )
					revision = NSvn.Core.Revision.FromNumber( this.Revision );

				string[] targets = new string[1];
				targets[0] = this.LocalDir;
				this.client.LogMessage += new LogMessageDelegate(client_LogMessage);
				this.client.Commit(targets, !this.Recursive);
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

		private void client_LogMessage(object sender, LogMessageEventArgs args)
		{
			args.Message = m_logMessage;
		}
	}
}
