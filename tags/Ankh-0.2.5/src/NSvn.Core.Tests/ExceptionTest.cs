using System;
using System.IO;
using Utils;
using NUnit.Framework;

namespace NSvn.Core.Tests
{
	/// <summary>
	/// Summary description for ExceptionTest.
	/// </summary>
	public class ExceptionTest : TestBase
	{
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.ExtractWorkingCopy();
            this.ExtractRepos();
        }

        /// <summary>
        /// Attempt to add a file that is not in a vc dir.
        /// </summary>
        [Test]
        [ExpectedException(typeof(NotVersionControlledException))]
        public void TestAddFileInNonVersionedDir()
        {
            string tempFile = Path.GetTempFileName();
            Client.Add( tempFile, true, new ClientContext() );
        }

        /// <summary>
        /// Attempt to update a locked wc.
        /// </summary>
        [Test]
        [ExpectedException(typeof(WorkingCopyLockedException))]
        public void TestCommitLockedWc()
        {
            string lockPath = Path.Combine( Path.Combine( this.WcPath, ".svn" ), "lock" );
            File.Create( lockPath ).Close();

            Client.Update( this.WcPath, Revision.Head, true, new ClientContext() );
        }

        /// <summary>
        /// Attempt to commit an out of date resource.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ResourceOutOfDateException))]
        public void TestResourceOutOfDate()
        {
            string wc2 = null;

            try
            {
                wc2 = this.FindDirName( Path.Combine( TestBase.BASEPATH, TestBase.WC_NAME ) );
                Zip.ExtractZipResource( wc2, this.GetType(), this.WC_FILE );

                using (StreamWriter w = new StreamWriter( Path.Combine( this.WcPath, "Form.cs" ), true ) )
                    w.Write( "Moo" );

                this.RunCommand( "svn", "ci -m \"\" " + this.WcPath );

                using (StreamWriter w2 = new StreamWriter( Path.Combine( wc2, "Form.cs" ), true ) )
                    w2.Write( "Moo" );

                Client.Commit( new string[]{ wc2 }, false, new ClientContext() );
            }
            finally
            {
                if ( wc2 != null )
                    this.RecursiveDelete( wc2 );

            }
        }

	}
}
