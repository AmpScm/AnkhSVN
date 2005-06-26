using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Threading;

namespace AnkhBot
{
    public class IssuezillaService : IService
    {
        public void Initialize( AnkhBot bot )
        {
            // empty
        }

        public Issue GetIssue( int number )
        {
            WebRequest request = WebRequest.Create( url );
            request.Method = "POST";

            string queryString = String.Format( 
                "id={0}&output_configured=true&include_attachments=false&download_type=browser" + 
                "&download_filename=issues.xml", number );

            byte[] queryBytes = Encoding.ASCII.GetBytes( queryString );
            request.ContentLength = queryBytes.Length;

            request.ContentType = "application/x-www-form-urlencoded";

            using( Stream s = request.GetRequestStream() )
                s.Write( queryBytes, 0, queryBytes.Length );

            // Get the response
            WebResponse response = null;
            AutoResetEvent evt = new AutoResetEvent(false);
            request.BeginGetResponse( delegate( IAsyncResult result )
            {
                response = request.EndGetResponse( result );
                evt.Set();
            }, null );

            if ( !WaitHandle.WaitAll( new WaitHandle[] { evt }, this.timeout, true ) )
            {
                throw new IssuezillaException( "Timed out" );
            }

            if ( response.ContentType.IndexOf( "text/xml" ) != 0 )
                throw new IssuezillaException( "Wrong content type returned: " + response.ContentType );

            // Deserialize to an Issue instance
            Issue issue = null;
            using ( Stream s = response.GetResponseStream() )
            {
                XmlSerializer serializer = new XmlSerializer( typeof( Issuezilla ) );
                issue = ((Issuezilla)serializer.Deserialize( s )).Issue;
            }


            return issue;
        }

        public void SetTimeout( int timeout )
        {
            this.timeout = timeout;
        }

        public string GetUrl( int issue )
        {
            return string.Format( "http://ankhsvn.tigris.org/issues/show_bug.cgi?id={0}", issue );
        }

        private const string url = @"http://ankhsvn.tigris.org/issues/xml.cgi";        
        private int timeout = 10000;
    }

    class IssuezillaException : Exception
    {
        public IssuezillaException( string msg )
            : base( msg )
        {
        }
    }

    
    /// <summary>
    /// Represents a single issue
    /// </summary>
    public class Issue
    {
        [XmlElement( "issue_id" )]
        public int Id;

        [XmlElement( "short_desc" )]
        public string ShortDescription;

        [XmlElement( "resolution" )]
        public string Resolution;

        [XmlElement( "issue_status" )]
        public string Status;

        [XmlElement( "priority" )]
        public string Priority;

    }

    /// <summary>
    /// Represents the returned document from IZ.
    /// </summary>
    [XmlRoot( "issuezilla" )]
    public class Issuezilla
    {
        [XmlElement( "issue" )]
        public Issue Issue;
    }
}
