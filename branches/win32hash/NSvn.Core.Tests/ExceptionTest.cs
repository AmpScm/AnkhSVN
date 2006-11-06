using System;
using System.IO;
using Utils;
using NUnit.Framework;
using TestUtils;

using NSvn.Common;

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
            this.Client.Add( tempFile, Recurse.Full );
        }

        /// <summary>
        /// Attempt to update a locked wc.
        /// </summary>
        [Test]
        [ExpectedException(typeof(WorkingCopyLockedException))]
        public void TestCommitLockedWc()
        {
            string lockPath = Path.Combine(
                Path.Combine( this.WcPath, Client.AdminDirectoryName ), "lock" );
            File.Create( lockPath ).Close();

            this.Client.Update( this.WcPath, Revision.Head, Recurse.Full );
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
                this.RenameAdminDirs( wc2 );

                using (StreamWriter w = new StreamWriter( Path.Combine( this.WcPath, "Form.cs" ), true ) )
                    w.Write( "Moo" );

                this.RunCommand( "svn", "ci -m \"\" " + this.WcPath );

                using (StreamWriter w2 = new StreamWriter( Path.Combine( wc2, "Form.cs" ), true ) )
                    w2.Write( "Moo" );

                this.Client.Commit( new string[]{ wc2 }, Recurse.Full );
            }
            finally
            {
                if ( wc2 != null )
                    PathUtils.RecursiveDelete( wc2 );

            }
        }

    }
}
