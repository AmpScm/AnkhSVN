// $Id$
using System;
using NUnit.Framework;
using NSvn.Core;
using System.IO;
using System.Collections;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests NSvn::Core::Client::Add
    /// </summary>
    [TestFixture]
    public class AddTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            this.ExtractWorkingCopy();
           

            this.notifications = new ArrayList();
        }

        /// <summary>
        /// Attempts to add a file, checking that the file actually was added
        /// </summary>
        [Test]
        public void TestBasic()
        {
            string testFile = this.CreateTextFile( "testfile.txt" );

            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ) );
            Client.Add( testFile, false, ctx );    
       
            Assertion.Assert( "No notification callbacks received", this.Notifications.Length > 0 );

            Assertion.AssertEquals( "svn st does not report the file as added", 
                'A', this.GetSvnStatus( testFile ) );
                        
        }

        /// <summary>
        /// Attempts to add a file with non-ansi characters in the filename
        /// </summary>
        [Test]
        public void TestAwkwardName()
        {
            string testFile = this.CreateTextFile( "månedslønn.tæxt");
            
            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ) );
            Client.Add( testFile, false, ctx );

            Assertion.AssertEquals( "svn st does not report the file as added", 
                'A', this.GetSvnStatus( testFile ) );
        }

        /// <summary>
        /// Creates a subdirectory with items in it, tries to add it non-recursively.
        /// Checks that none of the subitems are added
        /// </summary>
        [Test]
        public void TestAddDirectoryNonRecursively()
        {
            string dir1, dir2, testFile1, testFile2;
            this.CreateSubdirectories(out dir1, out dir2, out testFile1, out testFile2);

            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ) );
            // do a non-recursive add here
            Client.Add( dir1, false, ctx );

            Assertion.Assert( "Too many or no notifications received. Added recursively?", 
                this.Notifications.Length == 1 );
            Assertion.AssertEquals( "Subdirectory not added", 'A', this.GetSvnStatus( dir1 ) );
            try
            {
                Assertion.Assert( "Recursive add", this.GetSvnStatus( dir2 ) != 'A' );
                Assertion.Assert( "Recursive add", this.GetSvnStatus( testFile1 ) != 'A' );
                Assertion.Assert( "Recursive add", this.GetSvnStatus( testFile2 ) != 'A' );

                Assertion.Fail( "Files added recursively. Above assertions should have thrown" );
            }
            catch( ApplicationException )
            {
                // swallow
            }

        }

        /// <summary>
        /// Creates a subdirectory with some items in it. Attempts to add it recursively.
        /// </summary>
        [Test]
        public void TestAddDirectoryRecursively()
        {
            string dir1, dir2, testFile1, testFile2;
            this.CreateSubdirectories( out dir1, out dir2, out testFile1, out testFile2 );

            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ) );
            // now a recursive add
            Client.Add( dir1, true, ctx );

            // enough notifications?
            Assertion.AssertEquals( "Received wrong number of notifications", 
                4, this.Notifications.Length );
            Assertion.AssertEquals( "Subdirectory not added", 'A', this.GetSvnStatus( dir1 ) );
            Assertion.AssertEquals( "Subsubdirectory not added", 'A', this.GetSvnStatus( dir2 ) );
            Assertion.AssertEquals( "File in subdirectory not added", 'A', this.GetSvnStatus( testFile1 ) );
            Assertion.AssertEquals( "File in subsubdirectory not added", 'A', this.GetSvnStatus( testFile2 ) );
        }



        private void CreateSubdirectories(out string dir1, out string dir2, out string testFile1, out string testFile2)
        {
            dir1 = Path.Combine( this.WcPath, "subdir" );
            Directory.CreateDirectory( dir1 );

            dir2 = Path.Combine( dir1, "subsubdir" );
            Directory.CreateDirectory( dir2 );

            testFile1 = this.CreateTextFile( @"subdir\testfile.txt" );
            testFile2 = this.CreateTextFile( @"subdir\subsubdir\testfile2.txt" );
        }        
		
    }
}
