using System;
using NUnit.Framework;
using Ankh;
using System.IO;
using NSvn.Core;
using System.Collections;


namespace Ankh.Tests
{
	/// <summary>
	/// A test fixture for the Ankh.StatusCache class.
	/// </summary>
    [TestFixture]
    public class StatusCacheTest : NSvn.Core.Tests.TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractWorkingCopy();
        }

        /// <summary>
        /// Test extracting the same path using various casings.
        /// </summary>
        [Test]
        public void TestVariousPathVariations()
        {
            StatusCache cache = new StatusCache( this.Client );
            cache.Status( this.WcPath );

            string formPath = Path.Combine(this.WcPath, "Form.cs");

            SvnItem item = cache[formPath];
            Assert.AreEqual( StatusKind.Normal, item.Status.TextStatus );

            item = cache[formPath.ToLower()];
            Assert.AreEqual( StatusKind.Normal, item.Status.TextStatus );

            item = cache[formPath.ToUpper()];
            Assert.AreEqual( StatusKind.Normal, item.Status.TextStatus );
        }

        /// <summary>
        /// Get the deletions from a directory.
        /// </summary>
        [Test]
        public void TestGetDeletions()
        {
            StatusCache cache = new StatusCache( this.Client );
            this.Client.Delete( new string[]{Path.Combine( this.WcPath, "Form.resx" )}, 
                true);
            this.Client.Delete( new string[]{Path.Combine( this.WcPath, "GoogleOne.suo" )}, 
                true);
            
            // should be two deletions now
            cache.Status( this.WcPath );
            IList deletions = cache.GetDeletions( this.WcPath );
            Assert.AreEqual( 2, deletions.Count );

            // undelete one
            this.Client.Revert(  new string[]{Path.Combine( this.WcPath, "Form.resx" )}, false );
            deletions = cache.GetDeletions( this.WcPath );
            Assert.AreEqual( 1, deletions.Count );

            // this one should still be deleted
            Assert.AreEqual( Path.Combine( this.WcPath, "GoogleOne.suo" ), 
                ((SvnItem)deletions[0]).Path);  

            // undelete all
            this.Client.Revert( new string[]{this.WcPath}, true );
            deletions = cache.GetDeletions( this.WcPath );
            Assert.AreEqual( 0, deletions.Count );
        }

        /// <summary>
        /// Test getting an item for a path not in the initial Status call
        /// </summary>
        [Test]
        public void TestGetPathNotInInitialStatus()
        {
            StatusCache cache = new StatusCache(this.Client);
            cache.Status( Path.Combine(this.WcPath, "doc") );
            using( StreamWriter w = new StreamWriter(Path.Combine(this.WcPath, "Form.cs")) )
                w.WriteLine( "Foo" );

            SvnItem item = cache[Path.Combine(this.WcPath, "Form.cs")];
            Assert.AreEqual( StatusKind.Modified, item.Status.TextStatus );            
        }

        /// <summary>
        /// Test retrieving an unversioned item.
        /// </summary>
        [Test]
        public void TestGetUnversionedItem()
        {
            StatusCache cache = new StatusCache(this.Client);

            string path = Path.GetTempFileName();
            SvnItem item = cache[path];
            Assert.IsFalse( item.IsVersionable );
        }
	}

}
