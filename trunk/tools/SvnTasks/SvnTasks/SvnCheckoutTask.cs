using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using NSvn;
using NSvn.Common;
using NSvn.Core;

namespace SvnTasks
{
	/// <summary>
	/// A Nant task to check out from a SVN repository.
	/// </summary>
	[TaskName( "svncheckout" )]
	public class SvnCheckoutTask : SvnBaseTask
	{
		protected bool m_recursive  = true;

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
			Log(Level.Info, "{0} {1}", this.Name, this.LocalDir);
			
			try
			{
				Revision revision = NSvn.Core.Revision.Head;
				if ( this.Revision != -1 )
					revision = NSvn.Core.Revision.FromNumber( this.Revision );
								
				this.client.Checkout(this.Url, this.LocalDir, revision, 
                    this.Recursive );
	
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
