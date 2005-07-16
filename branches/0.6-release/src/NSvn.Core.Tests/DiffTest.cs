// $Id$
using System;
using NUnit.Framework;
using System.IO;
using System.Text;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests Client::Diff
    /// </summary>
    [TestFixture]
    public class DiffTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.ExtractRepos();
            this.ExtractWorkingCopy();
        }

        [Test]
        public void TestLocalDiff()
        {  
            string form = Path.Combine( this.WcPath, "Form.cs" );

            
            using( StreamWriter w = new StreamWriter( form, false ) )
                w.Write( "Moo moo moo moo moo\r\nmon\r\nmooo moo moo \r\nssdgo" );

            //Necessary for some weird reason
            Directory.SetCurrentDirectory( this.WcPath );
            string clientDiff = this.RunCommand( "svn", "diff Form.cs"  );

            MemoryStream outstream = new MemoryStream();
            MemoryStream errstream = new MemoryStream();
            this.Client.Diff( new string[]{}, "Form.cs", Revision.Base, "Form.cs", 
                Revision.Working, false, true, false, outstream, errstream );


            string err = Encoding.Default.GetString( errstream.ToArray() );
            Assertion.AssertEquals( "Error in diff: " + err, string.Empty, 
                err );
            string apiDiff = Encoding.Default.GetString( outstream.ToArray() );
            Assertion.AssertEquals( "Client diff differs", clientDiff, apiDiff );
        }

        [Test]
        public void TestReposDiff()
        {
            string clientDiff = this.RunCommand( "svn", "diff -r 1:5 " + this.ReposUrl );
            
            MemoryStream outstream = new MemoryStream();
            MemoryStream errstream = new MemoryStream();

            this.Client.Diff( new string[]{}, this.ReposUrl, Revision.FromNumber(1), 
                this.ReposUrl, Revision.FromNumber(5), true, true, false, outstream,
                errstream );

            string err = Encoding.Default.GetString( errstream.ToArray() );
            Assertion.AssertEquals( "Error in diff: " + err, string.Empty, err );

            string apiDiff = Encoding.Default.GetString( outstream.ToArray() );
            Assertion.AssertEquals( "Diffs differ", clientDiff, apiDiff );
        }

        [Test]
        public void TestDiffBinary()
        {
            string path = Path.Combine( this.WcPath, "Form.cs" );
            this.RunCommand( "svn", "propset svn:mime-type application/octet-stream " + 
                path );
            this.RunCommand( "svn", "ci -m '' " + path );

            using( StreamWriter w = new StreamWriter(path) )
                w.WriteLine( "Hi there" );
            

            MemoryStream outstream = new MemoryStream();
            MemoryStream errstream = new MemoryStream();

            // this should not diff a binary file
            this.Client.Diff( new string[]{}, path, Revision.Base, 
                path, Revision.Working, true, true, false, outstream,
                errstream );
            string diff = Encoding.ASCII.GetString( outstream.ToArray() );
            Assert.IsTrue( diff.IndexOf( "application/octet-stream" ) >= 0 );
            

            outstream = new MemoryStream();
            errstream = new MemoryStream();

            this.Client.Diff( new string[]{}, path, Revision.Base, 
                path, Revision.Working, true, true, false, true, outstream,
                errstream );

            Assert.IsTrue( outstream.Length > 0 );
            diff = Encoding.ASCII.GetString( outstream.ToArray() );
            Assert.IsTrue( diff.IndexOf( "application/octet-stream" ) < 0 );

        }

        		
    }
}
