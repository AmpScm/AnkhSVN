using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using NSvn;
using NSvn.Common;
using NSvn.Core;

namespace SvnTasks
{
	/// <summary>
	/// A Nant task to export from a SVN repository.
	/// </summary>
	[TaskName( "svnexport" )]
	public class SvnExportTask : SvnBaseTask
	{
		private bool m_force = false;
		
		[TaskAttribute("force", Required=false)]
		public bool Force
		{
			get
			{
				return m_force;
			}
			set
			{
				m_force = value;
			}
		}
      
		/// <summary>
		/// The funky stuff happens here.
		/// </summary>
		protected override void ExecuteTask()
		{
			Log(Level.Info, "{0} {1} to {2}", this.Name, this.Url, this.LocalDir);
			try
			{
				Revision revision = NSvn.Core.Revision.Head;
				if ( this.Revision != -1 )
					revision = NSvn.Core.Revision.FromNumber( this.Revision );
				
				this.client.Export(this.Url, this.LocalDir, revision, 
                    this.Force);

			}
			catch( AuthorizationFailedException )
			{
				throw new BuildException( "Unable to authorize against the repository." );
			}
			catch( WorkingCopyLockedException )
			{
				throw new BuildException( "The working copy is Locked." );
			}
			catch( NotVersionControlledException )
			{
				throw new BuildException( "The directory specified is not under version control." );
			}
			catch( ResourceOutOfDateException )
			{
				throw new BuildException( "The resource is of of date." );
			}
			catch( IllegalTargetException )
			{
				throw new BuildException( "Illegal target." );
			}
			catch( SvnException ex )
			{	
				throw new BuildException( "Unable to export. Does the local directory already exist? Use force option to overwrite.\n" + ex );
			}
			catch( Exception ex )
			{
				throw new BuildException( "Unexpected error: " + ex );
			}
		}

		
	}
}
