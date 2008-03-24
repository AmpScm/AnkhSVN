// $Id$
using System;
using System.Xml;
using SharpSvn;

namespace Ankh
{
    /// <summary>
    /// Summary description for LogResult.
    /// </summary>
    public class LogResult : XmlResultBase
    {
        public LogResult()
            : base("LogResult")
        {
        }

            public void Receive(object sender, SvnLogEventArgs args)
        {
            this.Writer.WriteStartElement( "LogItem" );

            this.Writer.WriteElementString( "Author", args.Author );
            this.Writer.WriteElementString("Revision", args.Revision.ToString());
            this.Writer.WriteElementString( "Date", args.Time.ToLocalTime().ToString( "s" ) );
            this.Writer.WriteElementString( "Message", args.LogMessage );

            this.Writer.WriteStartElement( "ChangedPaths" );
            foreach( SvnChangeItem cp in args.ChangedPaths )
            {
                this.Writer.WriteStartElement( "ChangedPath" );
                this.Writer.WriteElementString( "Path", cp.Path );
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
