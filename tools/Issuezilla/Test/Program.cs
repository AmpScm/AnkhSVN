using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Fines.IssueZillaLib;
using System.Collections.Specialized;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DoStuff();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);

            }
            
        }

        private static void DoStuff()
        {
            WebClient client = new WebClient();

            using (StreamReader reader = new StreamReader(client.OpenRead("http://ankhsvn.tigris.org")))
            {
                reader.ReadToEnd();
            }

            string[] cookies = client.ResponseHeaders.GetValues("Set-Cookie");
            string sessionId = GetSessionId(cookies);

            if (sessionId == null)
            {
                throw new Exception("Cannot find session ID");
            }

            client = new WebClient();

            client.QueryString["loginID"] = "arild_fines";
            client.QueryString["password"] = "grynte";
            client.QueryString["Login"] = "Login";
            client.Headers["Cookie"] = sessionId;

            DumpResponse(client);

            client = new WebClient();
            issue issue = new issue();
            issue.reporter = "arild_fines";
            issue.component = "ankhsvn";
            issue.version = "current";
            issue.subcomponent = "Ankh";
            issue.rep_platform = "All";
            issue.op_sys = "All";
            issue.priority = "P3";
            issue.issue_type = "DEFECT";
            issue.assigned_to = "";
            issue.cc = new string[] { "" };
            issue.issue_file_loc = "http://";
            issue.short_desc = "Summa summarum";

            client.Headers["Cookie" ] = sessionId;

            NameValueCollection nameValues = issue.GetNameValues();
            nameValues.Add( "comment", "No comment" );
            using ( MemoryStream stream = new MemoryStream( client.UploadValues( "http://ankhsvn.tigris.org/issues/post_bug.cgi", nameValues ) ) )
            {
                using ( StreamReader reader = new StreamReader( stream ) )
                {
                    Console.WriteLine(reader.ReadToEnd());
                }
            }

            
        }

        private static void DumpResponse(WebClient client)
        {
            using (StreamReader reader = new StreamReader(client.OpenRead("http://www.tigris.org/servlets/TLogin")))
            {
                Console.WriteLine(reader.ReadToEnd());
            }
        }

        private static string GetSessionId(string[] cookies)
        {
            foreach (string cookie in cookies)
            {
                if (cookie.StartsWith("JSessionID=", StringComparison.InvariantCultureIgnoreCase))
                {
                    return cookie;
                }
            }
            return null;
        }
    }
}
