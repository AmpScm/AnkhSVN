// $Id$
using System;
using NUnit.Framework;
using System.IO;

namespace NSvn.Tests
{
	/// <summary>
	/// Tests the WorkingCopyDirectory class.
	/// </summary>
	[TestFixture]
	public class WorkingCopyDirectoryTest : TestBase
	{
        /// <summary>
        /// 
        /// </summary>
        [SetUp]
        public override void SetUp()
        {
            this.ExtractWorkingCopy();
        }

        [Test]
        public void TestChildren()
        {
            string[] fsEntries = Directory.GetFileSystemEntries( this.WcPath );

            WorkingCopyDirectory dir = new WorkingCopyDirectory( this.WcPath );
            
            // the -1 is to account for the .svn dir
            Assertion.AssertEquals( "Wrong number of children", fsEntries.Length - 1, 
                dir.Children.Count );

            // verify that they are all there
            foreach( string fsEntry in fsEntries )
            {
                string filename = Path.GetFileName( fsEntry );
                // dont bother with this one
                if ( filename == ".svn" )
                    continue;

                Assertion.AssertNotNull( "Item " + filename + " not found", 
                    dir.Children[filename] );

                Assertion.Assert( filename + " should have been versioned",
                    dir.Children[filename].IsVersioned );

                // dir or file?
                if ( File.Exists( fsEntry ) )
                    Assertion.Assert( filename + " should have been a directory", 
                        !(dir.Children[filename].IsDirectory) );
                else if ( Directory.Exists( fsEntry ) )
                    Assertion.Assert( filename + " should have been a file",
                        dir.Children[filename].IsDirectory );
                else 
                    Assertion.Fail( "Huh? Not a file or a directory?" );
            }

            // make sure its updated properly
            using( StreamWriter w = File.CreateText( Path.Combine( this.WcPath, "moo" ) ) )
                w.Write( "MOO" );

            Assertion.AssertEquals( "Wrong number of children", fsEntries.Length,
                dir.Children.Count );
            Assertion.AssertNotNull( "moo not found", dir.Children["moo"] );
            Assertion.Assert( "moo should not be versioned", !(dir.Children["moo"].IsVersioned) );

        }
	}
}
