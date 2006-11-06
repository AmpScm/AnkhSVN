// $Id$
using System;
using System.Xml;
using NSvn.Core;

namespace Ankh
{
	/// <summary>
	/// Summary description for LogResult.
	/// </summary>
	public class LogResult : XmlResultBase
	{
		public LogResult() : base( "LogResult" )
		{
		}

        public void Receive( LogMessage msg )
        {
            this.Writer.WriteStartElement( "LogItem" );

            this.Writer.WriteElementString( "Author", msg.Author );
            this.Writer.WriteElementString( "Revision", msg.Revision.ToString() );
            this.Writer.WriteElementString( "Date", msg.Date.ToString( "s" ) );
            this.Writer.WriteElementString( "Message", msg.Message );

            this.Writer.WriteStartElement( "ChangedPaths" );
            foreach( string path in msg.ChangedPaths.Keys )
            {
                this.Writer.WriteStartElement( "ChangedPath" );
                this.Writer.WriteElementString( "Path", path );

                ChangedPath cp = msg.ChangedPaths.Get( path );
                this.Writer.WriteElementString( "Action", cp.Action.ToString() );

                if ( cp.CopyFromPath != null )
                    this.Writer.WriteElementString( "CopyFromPath", cp.CopyFromPath );

                if ( cp.CopyFromPath != null )
                    this.Writer.WriteElementString( "CopyFromRevision", 
                        cp.CopyFromRevision.ToString() );

                this.Writer.WriteEndElement();
            }
            this.Writer.WriteEndElement();

            this.Writer.WriteEndElement();
        }
	}
}
