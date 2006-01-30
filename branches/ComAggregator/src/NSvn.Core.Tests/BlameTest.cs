// $Id$
using System;
using System.IO;
using System.Collections;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests for the NSvn.Core.Client.Blame method.
    /// </summary>
    [TestFixture]
    public class BlameTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp ();
            this.ExtractRepos();
            this.ExtractWorkingCopy();
            this.blames = new ArrayList();
        }

        [Test]
        public void TestSimple()
        {
            string path = Path.Combine( this.WcPath, "Form.cs" );
            string blame = this.RunCommand( "svn", "blame -v " + path );
            Blame[] cmdline = this.ParseCommandLineBlame( blame );


            this.Client.Blame( path, Revision.FromNumber(0), Revision.Head, 
                new BlameReceiver( this.Receiver ) );
            
            Assert.AreEqual( cmdline.Length, this.blames.Count );
            for( int i = 0; i < cmdline.Length; i++ )
            {
                Blame.CheckEqual( cmdline[i], (Blame)this.blames[i] );
            }
        }

        [Test]
        public void TestWithEmptyEntries()
        {
            string path = Path.Combine( this.WcPath, "Form.cs" );

            // this won't give any results - verify that there are no exceptions
            this.Client.Blame( path, Revision.Head, Revision.Head,
                new BlameReceiver( this.Receiver ) );

            Blame[] b = (Blame[])this.blames.ToArray( typeof(Blame) );

            Assert.AreEqual( -1, b[0].Revision );
            Assert.AreEqual( "", b[0].Author );
            Assert.AreEqual( DateTime.MinValue, b[0].Date );

        }

        private void Receiver( long lineNumber, int revision, string author, 
            DateTime date, string line )
        {
            this.blames.Add( new Blame( lineNumber, revision, author, date, line ) );
        }

        private Blame[] ParseCommandLineBlame( string blame )
        {
            ArrayList blames = new ArrayList();
            long lineNumber = 0;
            foreach( Match m in BlameRegex.Matches( blame ) )
            {
                int revision = int.Parse( m.Groups["rev"].Value );
                string author = m.Groups["author"].Value;
                DateTime date = DateTime.ParseExact( m.Groups["date"].Value,
                    @"yyyy-MM-dd\ HH:mm:ss\ zzzz",
                    System.Globalization.CultureInfo.CurrentCulture );
                string line = m.Groups["line"].Value;
                blames.Add( new Blame( lineNumber++, revision, author, date, line ));
            }

            return (Blame[])blames.ToArray( typeof(Blame) );
        }

        private class Blame
        {
            public long LineNumber;
            public int Revision;
            public string Author;
            public DateTime Date;
            public string Line;

            public Blame( long lineNumber, int revision, string author, 
                DateTime date, string line )
            {
                this.LineNumber = lineNumber;
                this.Revision = revision;
                this.Author = author;
                this.Date = date;
                this.Line = line;
            }



            public static void CheckEqual( Blame a, Blame b )
            {
                Assert.AreEqual(a.LineNumber, b.LineNumber);
                Assert.AreEqual(a.Revision, b.Revision);
                Assert.AreEqual(a.Author, b.Author);
                long delta = Math.Abs( a.Date.Ticks - b.Date.Ticks );
                Assert.IsTrue( delta < Second.Ticks );
                Assert.AreEqual(a.Line, b.Line);
            }

            private static readonly TimeSpan Second = new TimeSpan( 0, 0, 0, 1 );
        }

        private readonly Regex BlameRegex = new Regex( 
            @"\s+(?<rev>\d+)\s+(?<author>\w+)\s+(?<date>\d\d\d\d-\d\d-\d\d \d\d:\d\d:\d\d [-+]\d\d\d\d) (\(\w{1,4}, \d\d \w{1,4} \d{4}\) )?(?<line>.*)" );

        private ArrayList blames;
    }
}
