#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Provider;
using NSvn.Core;
using System.IO;

using DriveInfo = System.Management.Automation.DriveInfo;

#endregion

namespace Rogue.Monad
{
	[CmdletProvider( "Svn", ProviderCapabilities.None )]
	public class SvnProvider : NavigationCmdletProvider, IContentCmdletProvider
	{
		public SvnProvider()
		{

		}


		protected override ProviderInfo Start( ProviderInfo providerInfo )
		{
			if ( providerInfo == null )
				throw new ArgumentNullException( "providerInfo" );

			return new SvnProviderInfo( providerInfo );
		}


		protected override bool IsValidPath( string path )
		{
			return true;
		}

		/// <summary>
		/// Lists a subversion directory, returning the child items.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="recurse"></param>
		protected override void GetChildItems( string path, bool recurse )
		{
			if ( string.IsNullOrEmpty( path ) )
			{
				throw new ArgumentException( "path" );
			}
			
			this.DoList( path, Revision.Head, recurse, delegate( DirectoryEntry entry )
			{
				this.WriteItemObject( entry,
					this.MakePath( path, entry.Path ), entry.NodeKind == NodeKind.Directory );
				return true;
			} );
		}


		protected override void GetChildNames( string path, bool returnAllContainers )
		{
			if ( string.IsNullOrEmpty( path ) )
			{
				throw new ArgumentException( "path" );
			}

			this.DoList( path, Revision.Head, false, delegate( DirectoryEntry entry )
			{
				if ( returnAllContainers || entry.NodeKind != NodeKind.Directory )
				{
					this.WriteItemObject( entry.Path, this.MakePath( path, entry.Path ),
						entry.NodeKind == NodeKind.Directory );
				}
				return true;
			} );
		}

		protected override void GetItem( string path )
		{
			string parent = this.GetParentPath( path, null );
			string child = this.GetChildName( path );
			
			// we can't get at a single item without listing
			this.DoList( path, Revision.Head, false, delegate( DirectoryEntry entry )
			{
				if ( entry.Path == child )
				{
					this.WriteItemObject( entry, this.MakePath( parent, entry.Path ),
						entry.NodeKind == NodeKind.Directory );
					return false;
				}
				return true;
			} );
		}


		protected override string GetParentPath( string path, string root )
		{
			return this.Urlify( base.GetParentPath( path, root ) );
		}


		protected override bool HasChildItems( string path )
		{
			return true;
		}

		protected override bool IsItemContainer( string path )
		{
			return true;
		}

		protected override bool ItemExists( string path )
		{
			return true;
		}




		/// <summary>
		/// Concatenates two paths.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		protected override string MakePath( string parent, string child )
		{
			if ( parent.EndsWith( ":" ) )
				return parent + PathSeparator + PathSeparator + child;
			else if ( parent.EndsWith( PathSeparator ) ^ child.StartsWith( PathSeparator ) )
				return parent + child;
			else if ( parent.EndsWith( PathSeparator ) && child.StartsWith( PathSeparator ) )
				return parent + child.Substring( 1 );
			else
				return parent + PathSeparator + child;
		}

		#region IContentCmdletProvider Members

		public void ClearContent( string path )
		{
			throw new NotImplementedException();
		}

		public object ClearContentDynamicParameters( string path )
		{
			throw new NotImplementedException();
		}

		public IContentReader GetContentReader( string path )
		{
			if ( string.IsNullOrEmpty( path ))
			{
				throw new ArgumentException( "path" );
			}
			string url = this.Urlify( path );
			MemoryStream stream = new MemoryStream();
			this.Client.Cat( stream, url, Revision.Head );
			return new ContentReader( stream );

		}

		public object GetContentReaderDynamicParameters( string path )
		{
			throw new NotImplementedException();
		}

		public IContentWriter GetContentWriter( string path )
		{
			throw new NotImplementedException();
		}

		public object GetContentWriterDynamicParameters( string path )
		{
			throw new NotImplementedException();
		}

		#endregion

		private SvnProviderInfo SvnProvInfo
		{
			get { return (SvnProviderInfo)this.ProviderInfo; }
		}

		private Client Client
		{
			get { return this.SvnProvInfo.Client; }
		}

		private string Urlify( string path )
		{
			return path.Replace( "\\", "/" );
		}

		private delegate bool ListCallback( DirectoryEntry entry );

		private void DoList( string path, Revision revision, bool recurse, ListCallback callback )
		{
			string url = this.Urlify( path );
			DirectoryEntry[] entries = this.Client.List( url, revision, recurse );

			foreach ( DirectoryEntry entry in entries )
			{
				if ( !callback( entry ) )
					break;
			}
		}

		/// <summary>
		/// Maintains our internal state.
		/// </summary>
		private class SvnProviderInfo : ProviderInfo
		{
			public SvnProviderInfo( ProviderInfo info ) : base( info )
			{
				this.client = new Client();
			}

			public Client Client
			{
				get { return this.client; }
			}

			private Client client;
		}

		/// <summary>
		/// Implemented to read from a repository file.
		/// </summary>
		private class ContentReader : IContentReader
		{
			public ContentReader( Stream stream )
			{
				stream.Seek( 0L, SeekOrigin.Begin );
				this.reader = new StreamReader( stream );
			}

			#region IContentReader Members

			public void Close()
			{
				this.reader.Close();
			}

			public System.Collections.IList Read( long readCount )
			{
				List<string> list = new List<string>();
				for ( long i = 0; i < readCount; i++ )
				{
					string line;
					if ( (line = this.reader.ReadLine()) == null )
						break;
					list.Add( line );
				}

				return list.ToArray();
			}

			public void Seek( long offset, System.IO.SeekOrigin origin )
			{
				this.reader.BaseStream.Seek( offset, origin );
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				this.Close();
			}

			#endregion

			private StreamReader reader;
		}
		private const string DriveSeparator = "@";
		private const string PathSeparator = "/";
		private const string ProtocolSeparator = ":";


	}
}
