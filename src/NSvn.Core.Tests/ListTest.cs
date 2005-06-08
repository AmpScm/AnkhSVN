// $Id$
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
            base.SetUp();

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
            string[] entries = Regex.Split( list, @"\r\n" );
            //string[] entries = list.Trim().Split( '\n' );
            Hashtable ht = new Hashtable();
            foreach( string e in entries)
            { 
                if ( e != String.Empty )
                {
                    Entry ent = new Entry( e );
                    ht[ ent.Path ] = ent;
                }
            }

            DirectoryEntry[] dirents = this.Client.List( this.ReposUrl, Revision.Head, 
                false);

            Assertion.AssertEquals( "Wrong number of entries returned", ht.Count, 
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

                this.createdRevision = int.Parse( match.Groups[1].ToString() );
                this.author = match.Groups[2].ToString();
                
                if ( match.Groups[3].Success )
                    this.size = long.Parse( match.Groups[3].ToString() );
                else 
                    this.size = 0;

                System.IFormatProvider format = 
                    System.Globalization.CultureInfo.CurrentCulture;

                // get the month and day
                string date = match.Groups[4].ToString();
                Console.WriteLine(date);
                this.time = DateTime.ParseExact( date, "MMM' 'dd", 
                    format );                

                // the year
                if ( match.Groups[5].Success )
                {
                    this.time = this.time.AddYears( -this.time.Year + 
                        int.Parse(match.Groups[5].ToString()) );
                }

                // or the time of day?
                DateTime timeOfDay = DateTime.Today;
                if ( match.Groups[6].Success )
                {
                    timeOfDay = DateTime.ParseExact( match.Groups[6].ToString(), 
                        "HH':'mm", format );
                }
                this.time = this.time.AddHours( timeOfDay.Hour );
                this.time = this.time.AddMinutes( timeOfDay.Minute );
 
                this.path = match.Groups[7].ToString();                
            }

            public void Match( DirectoryEntry ent )
            {                
                Assertion.AssertEquals( "CreatedRevision differs", this.createdRevision, 
                    ent.CreatedRevision );
                Assertion.AssertEquals( "Size differs", this.size, 
                    ent.Size );

                // strip off time portion
                DateTime entryTime = ent.Time.ToLocalTime();
                entryTime = entryTime - entryTime.TimeOfDay;

                long delta =  Math.Abs( this.time.Ticks - entryTime.Ticks );
                Assertion.Assert( "Time differs: " + this.time + " vs " + 
                    entryTime + " Delta is " + delta, 
                    delta < TICKS_PER_MINUTE );
                Assertion.AssertEquals( "Last author differs", this.author, ent.LastAuthor );
            }

            public string Path
            { 
                get{ return this.path; }
            }

            private const long TICKS_PER_MINUTE = 600000000;

            private int createdRevision;
            private string author;
            private long size;
            private DateTime time;
            private string path;
            private static readonly Regex Reg = new Regex(
                @"\s+(\d+)\s+(\w+)\s+(\d+)?\s+(\w+\s\d+)\s+(?:(\d{4})|(\d\d:\d\d))\s+(\S+)" );
        }
    }
}
