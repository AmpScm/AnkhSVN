using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using NSvn;
using NSvn.Common;
using NSvn.Core;

namespace SvnTasks
{
	/// <summary>
	/// A Nant task to get the latest revision from a SVN repository.
	/// </summary>
	[TaskName( "svnrevision" )]
	public class SvnRevisionTask : SvnBaseTask
	{
		
		private string nodeToGetRevisionOn;
		/// <summary>
		/// The funky stuff happens here.
		/// </summary>
		protected override void ExecuteTask()
		{
			this.Log(Level.Info, "{0} {1}",this.Name, this.Url);
			
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
				DirectoryEntry[] dirs = Client.List(this.Url, revision, false, clientContext);
				
				foreach (DirectoryEntry de in dirs)
				{
					if (de.Path == nodeToGetRevisionOn)
					{
						this.Revision = de.CreatedRevision;
						Project.Properties[ "svn.revision" ] = String.Format("{0:D4}", de.CreatedRevision);
					}
				}
				
			}
			catch( AuthorizationFailedException )
			{
				throw new BuildException( "Unable to authorize against the repository." );
			}
		
			catch( SvnException ex )
			{	
				throw new BuildException( "Unable to get revision.\n" + ex );
			}
			catch( Exception ex )
			{
				throw new BuildException( "Unexpected error: " + ex );
			}
		}
		
		/// <summary>
		/// This overide parses the url passed in into 2 parts. 
		/// The base url and the node to get the revision on. 
		/// </summary>
		/// <remarks>Assumes the last portion of the url is a node and not the repository</remarks>
		public override string  Url
		{
			get
			{
				return url;
			}
			set
			{
				url = value;
				if (url.LastIndexOf("/") == url.Length - 1)
				{
					url = url.Remove(url.Length - 1, 1);
				}
				int lastSlash = url.LastIndexOf("/");
				nodeToGetRevisionOn = url.Substring(lastSlash + 1);
				url = url.Substring(0,lastSlash);
			}
		}
	
	}
}
