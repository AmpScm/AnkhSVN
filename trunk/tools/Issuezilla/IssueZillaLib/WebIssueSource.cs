using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace Fines.IssueZillaLib
{
    public class WebIssueSource : IssueSourceBase, IIssuePoster, IMetadataSource
    {
        public WebIssueSource( string baseUrl, ICredentials credentials )
        {
            this.baseUrl = baseUrl;
            this.credentials = credentials;
        }

        public override issuezilla GetAllIssues()
        {
            this.EnsureLoggedIn();

            WebClient client = this.CreateWebClient();

            client.QueryString[ "output_configured" ] = "true";
            client.QueryString[ "id" ] = "1-1000";
            client.QueryString[ "download_type" ] = "browse";
            client.QueryString[ "download_filename" ] = "issues.xml";

            issuezilla zilla = null;

            using(StreamReader reader = new StreamReader(client.OpenRead(this.baseUrl + "issues/xml.cgi")))
            {
                zilla = this.Deserialize( reader );
            }

            this.ExtractSessionId( client );

            foreach ( issue issue in zilla.issue )
            {
                issue.State = IssueState.Unmodified;
            }

            return zilla;
        }

        private void ExtractSessionId( WebClient client)
        {
            string[] cookies = client.ResponseHeaders.GetValues( "Set-Cookie" );
            if ( cookies == null )
            {
                return;
            }

            foreach ( string cookie in cookies )
            {
                if ( cookie.StartsWith( "JSessionID=", StringComparison.InvariantCultureIgnoreCase ) )
                {
                    this.sessionIdCookie = cookie;
                    break;
                }
            }
        }

        public void PostNewIssue( issue issue, string comment )
        {
            this.EnsureLoggedIn();

            WebClient client = this.CreateWebClient();
            NameValueCollection coll = issue.GetNameValues();

            coll.Add( "comment", comment );

            using ( StreamReader reader = new StreamReader(
                new MemoryStream(
                    client.UploadValues(
                        this.baseUrl + "issues/post_bug.cgi",
                        coll) 
                        )
                    )
                )
            {
                string message = reader.ReadToEnd();
               

                Form form = new Form();
                WebBrowser browser = new WebBrowser();
                form.Controls.Add( browser );
                browser.Dock = DockStyle.Fill;

                browser.DocumentText = message;

                form.ShowDialog();

            }
        }

        public void UpdateIssue( issue issue, string comment )
        {
            this.EnsureLoggedIn();

            WebClient client = this.CreateWebClient();
            NameValueCollection coll = issue.GetNameValues();

            coll.Add( "comment", comment );
            coll.Add( "id", issue.issue_id );
            coll.Add( "knob", "none" );
            coll.Add( "longdesclength", "0" );

            using ( StreamReader reader = new StreamReader(
                new MemoryStream(
                    client.UploadValues(
                        this.baseUrl + "issues/process_bug.cgi",
                        coll )
                        )
                    )
                )
            {
                string message = reader.ReadToEnd();

                
            }
        }

        public void LoadMetaData()
        {
            this.LoadMetaDataItems();
        }

        public IList<MetadataItem<string, string>> Versions
        {
            get 
            {
                return this.GetMetaData( "version" );
            }
        }

        public IList<MetadataItem<string, string>> SubComponents
        {
            get
            {
                return this.GetMetaData( "subcomponent" );
            }
        }

        public IList<MetadataItem<string, string>> Components
        {
            get
            {
                return this.GetMetaData( "component" );
            }
        }

        public IList<MetadataItem<string, string>> Platforms
        {
            get
            {
                return this.GetMetaData( "rep_platform" );
            }
        }


        public IList<MetadataItem<string, string>> OperatingSystems
        {
            get
            {
                return this.GetMetaData( "op_sys" );
            }
        }

        public IList<MetadataItem<string, string>> Priorities
        {
            get
            {
                return this.GetMetaData( "priority" );
            }
        }

        public IList<MetadataItem<string, string>> IssueTypes
        {
            get
            {
                return this.GetMetaData( "issue_type" );
            }
        }

        public IList<MetadataItem<string, string>> Resolutions
        {
            get
            {
                return this.GetMetaData( "resolution" );
            }
        }

        private IList<MetadataItem<string, string>> GetMetaData( string name )
        {
            if ( metaDataItems == null )
            {
                this.LoadMetaDataItems();
            }

            IList<MetadataItem<string, string>> items = null;
            if ( metaDataItems.TryGetValue(name, out items) )
            {
                return items;  
            }
            else
            {
                return null;
            }
        }

        private void LoadMetaDataItems()
        {
            this.EnsureLoggedIn();

            WebClient client = this.CreateWebClient();

            string body = null;
            using ( StreamReader reader = new StreamReader(client.OpenRead(this.baseUrl + "issues/show_bug.cgi?id=1")) )
            {
                body = reader.ReadToEnd();
            }

            this.metaDataItems = new Dictionary<string, IList<MetadataItem<string, string>>>();


            foreach ( Match select in SelectRegex.Matches(body) )
            {
                List<MetadataItem<string, string>> list = new List<MetadataItem<string, string>>();
                string name = select.Groups[ "name" ].Value;

                foreach ( Match option in OptionRegex.Matches( select.Groups[ "body" ].Value ) )
                {
                    list.Add( new MetadataItem<string, string>( option.Groups[ "value" ].Value,
                        option.Groups[ "text" ].Value ) );
                }

                this.metaDataItems.Add( name, list );
            }
            
        }

        private WebClient CreateWebClient()
        {
            WebClient client = new WebClient();
            if ( this.sessionIdCookie != null )
            {
                client.Headers[ "Cookie" ] = this.sessionIdCookie; 
            }
            return client;
        }

        private void EnsureLoggedIn()
        {
            GetSessionIdCookie();

            WebClient client = this.CreateWebClient();

            NetworkCredential cred = this.credentials.GetCredential( new Uri( this.baseUrl ), "" );

            if ( cred != null )
            {
                client.QueryString[ "loginID" ] = cred.UserName;
                client.QueryString[ "password" ] = cred.Password;
                client.QueryString[ "Login" ] = "Login";

                using ( StreamReader reader = new StreamReader( client.OpenRead( this.baseUrl +
                    "servlets/TLogin" ) ) )
                {
                    reader.ReadToEnd();
                } 
            }
        }

        private void GetSessionIdCookie()
        {
            if ( this.sessionIdCookie != null )
            {
                return;
            }

            WebClient client = new WebClient();
            using ( StreamReader reader = new StreamReader( client.OpenRead( this.baseUrl ) ) )
            {
                reader.ReadToEnd();
            }

            ExtractSessionId( client );
        }

        private static readonly Regex SelectRegex = new Regex( @"\<select\s+name=[""'](?<name>.*?)[""'].*?\>(?<body>.*?)</select",
                ( RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace ) );
        private static readonly Regex OptionRegex = new Regex( @"\<option.*?value=[""'](?<value>.*?)[""'].*?\>(?<text>.*?)\</option\>",
            RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase );



        private string sessionIdCookie;
        private string baseUrl;
        private ICredentials credentials;
        private IDictionary<string, IList<MetadataItem<string, string>>> metaDataItems;


    }
}
