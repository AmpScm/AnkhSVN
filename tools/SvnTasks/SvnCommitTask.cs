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

				ClientContext clientContext = new ClientContext();
				clientContext.AuthBaton = new AuthenticationBaton();
				clientContext.AuthBaton.Add(AuthenticationProvider.GetUsernameProvider());
				clientContext.AuthBaton.Add(AuthenticationProvider.GetSimpleProvider());
				if ( this.Username != null && this.Password != null )
				{
					clientContext.AuthBaton.Add(
						AuthenticationProvider.GetSimplePromptProvider(
						new SimplePromptDelegate(this.SimplePrompt),1));
				}
				string[] targets = new string[1];
				targets[0] = this.LocalDir;
				Client.Commit(targets, !this.Recursive, clientContext);
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
	}
}
