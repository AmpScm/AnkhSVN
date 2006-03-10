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
            base.SetUp();
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

            this.Client.Notification += new NotificationDelegate(this.NotifyCallback);
            this.Client.Add( testFile, false );    
       
			Assert.IsTrue( this.Notifications.Length > 0, "No notification callbacks received" );

			Assert.AreEqual( 'A', this.GetSvnStatus( testFile ), "svn st does not report the file as added" );
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

            this.Client.Notification += new NotificationDelegate(this.NotifyCallback);
            // do a non-recursive add here
            this.Client.Add( dir1, false );

            Assert.IsTrue( this.Notifications.Length == 1, "Too many or no notifications received. Added recursively?" );
            Assert.AreEqual( 'A', this.GetSvnStatus( dir1 ), "Subdirectory not added");
            try
            {
                Assert.IsTrue(  this.GetSvnStatus( dir2 ) != 'A', "Recursive add" );
                Assert.IsTrue( this.GetSvnStatus( testFile1 ) != 'A', "Recursive add" );
                Assert.IsTrue( this.GetSvnStatus( testFile2 ) != 'A', "Recursive add" );

                Assert.Fail( "Files added recursively. Above assertions should have thrown" );
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

            this.Client.Notification += new NotificationDelegate(this.NotifyCallback);
            // now a recursive add
            this.Client.Add( dir1, true );

            // enough notifications?
            Assert.AreEqual( 4, this.Notifications.Length, "Received wrong number of notifications" );
            Assert.AreEqual( 'A', this.GetSvnStatus( dir1 ), "Subdirectory not added" );
            Assert.AreEqual( 'A', this.GetSvnStatus( dir2 ), "Subsubdirectory not added" );
            Assert.AreEqual( 'A', this.GetSvnStatus( testFile1 ), "File in subdirectory not added" );
            Assert.AreEqual( 'A', this.GetSvnStatus( testFile2 ), "File in subsubdirectory not added" );
        }

        [Test]
        public void TestWithForce()
        {
            string file = Path.Combine( this.WcPath, "AssemblyInfo.cs" );
            try
            {
                this.Client.Add( file, false, false );
                Assert.Fail( "Should have failed" );
            }
            catch( SvnClientException )
            {
                // swallow
            }

            // should not fail
            this.Client.Add( file, false, true );
        }

        [Test]
        public void TestAddFileWithNonAsciiFilename()
        {
            string newFile = Path.Combine( this.WcPath, "Æeiaæå.ø");
            File.Create(newFile).Close();
            this.Client.Add(newFile, true);
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
