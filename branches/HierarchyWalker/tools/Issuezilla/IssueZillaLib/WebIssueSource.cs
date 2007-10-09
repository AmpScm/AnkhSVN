using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Net;

namespace Fines.IssueZillaLib
{
    public class WebIssueSource : IssueSourceBase
    {
        public WebIssueSource( string baseUrl )
        {
            this.baseUrl = baseUrl;
        }

        public override issuezilla GetAllIssues()
        {
            WebClient client = new WebClient();
            client.QueryString[ "output_configured" ] = "true";
            client.QueryString[ "id" ] = "1-1000";
            client.QueryString[ "download_type" ] = "browse";
            client.QueryString[ "download_filename" ] = "issues.xml";

            using(StreamReader reader = new StreamReader(client.OpenRead(this.baseUrl)))
            {
                return this.Deserialize( reader );
            }
        }

       

        private string baseUrl;
    }
}
