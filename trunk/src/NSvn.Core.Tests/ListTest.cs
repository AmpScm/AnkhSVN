using System;
using System.Collections;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace NSvn.Core.Tests
{
	/// <summary>
	/// Tests Client::List
	/// </summary>
	[TestFixture]
	public class ListTest : TestBase
	{
        [SetUp]
        public override void SetUp()
        {
            this.ExtractRepos();
        }

        /// <summary>
        /// Compares the list from the command line client with that obtained
        /// from Client::List
        /// </summary>
        [Test]
        public void TestList()
        {
            string list = this.RunCommand( "svn", "list -v " + this.ReposUrl );           

            // clean out whitespace
            string[] entries = list.Trim().Split( '\n' );
            Hashtable ht = new Hashtable();
            foreach( string e in entries)
            {
                Entry ent = new Entry( e );
                ht[ ent.Path ] = ent;
            }

            DirectoryEntry[] dirents = Client.List( this.ReposUrl, Revision.Head, false, 
                new ClientContext() );

            Assertion.AssertEquals( "Wrong number of entries returned", entries.Length, 
                dirents.Length );

            foreach( DirectoryEntry ent in dirents )
            {
                string path = ent.Path;
                if ( ent.NodeKind == NodeKind.Directory )
                    path += "/";

                Entry entry = (Entry)ht[ path ];
                Assertion.AssertNotNull( "No entry found for " + path, entry );

                entry.Match( ent );               
            }
        }

        private class Entry
        {
            
            
            public Entry( string line )
            {
                if ( !Reg.IsMatch( line ) )
                    throw new Exception( "Commandline client bad output" );

                Match match = Reg.Match( line );
                
                this.hasProperties = match.Groups[1].ToString() == "P";
                this.createdRevision = int.Parse( match.Groups[2].ToString() );
                this.author = match.Groups[3].ToString();
                this.size = long.Parse( match.Groups[4].ToString() );
                System.IFormatProvider format =
                    new System.Globalization.CultureInfo("en-US", true);

                this.time = DateTime.ParseExact( match.Groups[5].ToString(), "MMM' 'dd' 'HH':'mm", 
                    format );
                this.path = match.Groups[6].ToString();                
            }

            public void Match( DirectoryEntry ent )
            {                
                Assertion.AssertEquals( "HasProperties differs", this.hasProperties, 
                    ent.HasProperties );
                Assertion.AssertEquals( "CreatedRevision differs", this.createdRevision, 
                    ent.CreatedRevision );
                Assertion.AssertEquals( "Size differs", this.size, 
                    ent.Size );

                long delta =  Math.Abs( this.time.Ticks - ent.Time.ToLocalTime().Ticks );
                Assertion.Assert( "Time differs: " + this.time + " vs " + 
                    ent.Time.ToLocalTime() + " Delta is " + delta, 
                    delta < TICKS_PER_MINUTE );
                Assertion.AssertEquals( "Last author differs", this.author, ent.LastAuthor );
            }

            public string Path
            { 
                get{ return this.path; }
            }

            private const long TICKS_PER_MINUTE = 600000000;

            private bool hasProperties;
            private int createdRevision;
            private string author;
            private long size;
            private DateTime time;
            private string path;
            private static readonly Regex Reg = new Regex(
                @"(\w)\s+(\d+)\s+(\w+)\s+(\d+)\s+(\w+\s\d+\s\d\d:\d\d)\s+(\S+)" );
        }
    }
}
