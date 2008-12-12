// $Id$
//
// Copyright 2004-2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

// $Id$
using System;
using NUnit.Framework;
using System.IO;
using TestUtils;
using System.Collections;


namespace Ankh.Tests
{
    /// <summary>
    /// Tests the Ankh.SvnItem class.
    /// </summary>
    [TestFixture]
    public class SvnItemTest : NSvn.Core.Tests.TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractWorkingCopy();    
            this.ExtractRepos();
        }
        
        /// <summary>
        /// Tests the Changed event.
        /// </summary>
        [Test]
        public void TestChanged()
        {  
            SvnItem item = this.GetItem();

            item.Changed += new EventHandler(ItemChanged);

            using( StreamWriter writer = new StreamWriter(item.Path) )
                writer.WriteLine( "Foo" );

            Status status = this.Client.SingleStatus(item.Path);
            item.Refresh( status );

            Assert.IsTrue( this.itemChanged, "Changed not fired" );
        }

        /// <summary>
        /// Test the Path property.
        /// </summary>
        [Test]
        public void TestPath()
        {
            string path = Path.Combine( this.WcPath, "Form1.cs" );
            Status status = this.Client.SingleStatus( path );
            SvnItem item = new SvnItem( path, status );  

            Assert.AreEqual( path, item.Path, "Paths differ" );                       
        }


        /// <summary>
        /// Tests the IsVersioned property.
        /// </summary>
        [Test]
        public void TestIsVersioned()
        {
            // normal
            SvnItem item1 = this.GetItem();
            Assert.IsTrue( item1.IsVersioned );

            // missing
            File.Delete( item1.Path );
            item1.Refresh( this.Client );
            Assert.IsFalse( item1.IsVersioned );

            // revert it so we can play some more with it
            this.Client.Revert( new string[]{ item1.Path }, Recurse.None );

            // modified
            using( StreamWriter writer = new StreamWriter(item1.Path) )
                writer.WriteLine( "Foo" );
            item1.Refresh( this.Client );
            Assert.IsTrue( item1.IsVersioned );

            // added
            string addedFilePath = Path.Combine( this.WcPath, "added.txt" );
            File.CreateText(addedFilePath).Close();            
            this.Client.Add( addedFilePath, Recurse.Full );
            Status addedFileStatus = this.Client.SingleStatus( addedFilePath );
            SvnItem addedItem = new SvnItem( addedFilePath, addedFileStatus );
            Assert.IsTrue( addedItem.IsVersioned );

            

            // conflicted
            string otherWc = this.GetTempFile();
            Zip.ExtractZipResource( otherWc, this.GetType(), this.WC_FILE );
            try
            {
                using( StreamWriter w2 = new StreamWriter( Path.Combine(otherWc, "Form1.cs") ) )
                    w2.WriteLine( "Something else" );
                this.Client.Commit( new string[]{ otherWc }, Recurse.Full );
            }
            finally
            {
                Utils.PathUtils.RecursiveDelete( otherWc );
            }
            this.Client.Update( this.WcPath, Revision.Head, Recurse.Full );
            item1.Refresh( this.Client );
            Assert.AreEqual( StatusKind.Conflicted, item1.Status.TextStatus );
            Assert.IsTrue( item1.IsVersioned );

            // deleted
            this.Client.Resolved( item1.Path, Recurse.None );
            this.Client.Revert( new string[]{item1.Path}, Recurse.None );
            this.Client.Delete( new string[]{ item1.Path }, true );
            item1.Refresh( this.Client );
            Assert.AreEqual( StatusKind.Deleted, item1.Status.TextStatus );
            Assert.IsTrue( item1.IsVersioned );

            // unversioned
            string unversionedFile = Path.Combine( this.WcPath, "nope.txt" );
            File.CreateText(unversionedFile).Close();
            SvnItem unversioned = new SvnItem(unversionedFile, this.Client.SingleStatus(unversionedFile));
            Assert.AreEqual( StatusKind.Unversioned, unversioned.Status.TextStatus );
            Assert.IsFalse( unversioned.IsVersioned );

            // none
            string nonePath = Path.GetTempFileName();
            SvnItem none = new SvnItem(nonePath, this.Client.SingleStatus(nonePath));
            Assert.AreEqual( StatusKind.None, none.Status.TextStatus );
            Assert.IsFalse( none.IsVersioned );

        }

        /// <summary>
        /// Test that an incomplete working copy is also marked as versioned.
        /// </summary>
        [Test]
        public void TestIncompleteIsVersioned()
        {
            this.Client.Cancel += new CancelDelegate(this.Cancel);
            try
            {
                string incompletePath = this.GetTempFile();    
                try
                {
                     this.Client.Checkout( this.ReposUrl, incompletePath, Revision.Head, Recurse.Full );
                }
                catch( OperationCancelledException )
                {
                    // empty
                }

                SvnItem item = new SvnItem( incompletePath, 
                    this.Client.SingleStatus(incompletePath) );
                Assert.AreEqual( StatusKind.Incomplete, item.Status.TextStatus );
                Assert.IsTrue( item.IsVersioned );
            }
            finally
            {
                this.Client.Cancel -= new CancelDelegate(this.Cancel);
            }
        }

        /// <summary>
        /// Test the IsModified property.
        /// </summary>
        [Test]
        public void TestIsModified()
        {
            // normal
            SvnItem item1 = this.GetItem();
            Assert.IsFalse( item1.IsModified );

            // missing
            File.Delete(item1.Path);
            item1.Refresh(this.Client);
            Assert.IsFalse(item1.IsModified);

            this.Client.Revert(new string[]{item1.Path}, Recurse.Full);

            // modified
            using( StreamWriter writer = new StreamWriter(item1.Path) )
                writer.WriteLine( "Foo" );
            item1.Refresh( this.Client );
            Assert.IsTrue( item1.IsModified );

            // added
            string addedFilePath = Path.Combine( this.WcPath, "added.txt" );
            File.CreateText(addedFilePath).Close();            
            this.Client.Add( addedFilePath, Recurse.Full );
            Status addedFileStatus = this.Client.SingleStatus( addedFilePath );
            SvnItem addedItem = new SvnItem( addedFilePath, addedFileStatus );
            Assert.IsTrue( addedItem.IsModified );            

            // conflicted
            string otherWc = this.GetTempFile();
            Zip.ExtractZipResource( otherWc, this.GetType(), this.WC_FILE );
            try
            {
                using( StreamWriter w2 = new StreamWriter( Path.Combine(otherWc, "Form1.cs") ) )
                    w2.WriteLine( "Something else" );
                this.Client.Commit( new string[]{ otherWc }, Recurse.Full );
            }
            finally
            {
                Utils.PathUtils.RecursiveDelete( otherWc );
            }
            this.Client.Update( this.WcPath, Revision.Head, Recurse.Full );
            item1.Refresh( this.Client );
            Assert.AreEqual( StatusKind.Conflicted, item1.Status.TextStatus );
            Assert.IsTrue( item1.IsModified );

            // deleted
            this.Client.Resolved( item1.Path, Recurse.None );
            this.Client.Revert( new string[]{item1.Path}, Recurse.None );
            this.Client.Delete( new string[]{ item1.Path }, true );
            item1.Refresh( this.Client );
            Assert.AreEqual( StatusKind.Deleted, item1.Status.TextStatus );
            Assert.IsTrue( item1.IsModified );

            // unversioned
            string unversionedFile = Path.Combine( this.WcPath, "nope.txt" );
            File.CreateText(unversionedFile).Close();
            SvnItem unversioned = new SvnItem(unversionedFile, this.Client.SingleStatus(unversionedFile));
            Assert.AreEqual( StatusKind.Unversioned, unversioned.Status.TextStatus );
            Assert.IsFalse( unversioned.IsModified );
            
            // none
            string nonePath = Path.GetTempFileName();
            SvnItem none = new SvnItem(nonePath, this.Client.SingleStatus(nonePath));
            Assert.AreEqual( StatusKind.None, none.Status.TextStatus );
            Assert.IsFalse( none.IsModified );
        }

        /// <summary>
        /// Tests the IsDirectory property.
        /// </summary>
        [Test]
        public void TestIsDirectory()
        {
            Assert.IsTrue( this.GetItem(this.WcPath).IsDirectory );
            Assert.IsFalse( this.GetItem().IsDirectory );
        }
        
        /// <summary>
        /// Tests the IsFile property.
        /// </summary>
        [Test]
        public void TestIsFile()
        {
            Assert.IsFalse( this.GetItem(this.WcPath).IsFile );
            Assert.IsTrue( this.GetItem().IsFile );
        }

        /// <summary>
        /// Tests the IsVersionable property.
        /// </summary>
        [Test]
        public void TestIsVersionable()
        {
            // normal
            SvnItem item1 = this.GetItem();
            Assert.IsTrue( item1.IsVersionable );

            // missing
            File.Delete( item1.Path );
            item1.Refresh( this.Client );
            Assert.IsTrue( item1.IsVersionable );

            // revert it so we can play some more with it
            this.Client.Revert( new string[]{ item1.Path }, Recurse.None );

            // modified
            using( StreamWriter writer = new StreamWriter(item1.Path) )
                writer.WriteLine( "Foo" );
            item1.Refresh( this.Client );
            Assert.IsTrue( item1.IsVersionable );

            // added
            string addedFilePath = Path.Combine( this.WcPath, "added.txt" );
            File.CreateText(addedFilePath).Close();            
            this.Client.Add( addedFilePath, Recurse.Full );
            Status addedFileStatus = this.Client.SingleStatus( addedFilePath );
            SvnItem addedItem = new SvnItem( addedFilePath, addedFileStatus );
            Assert.IsTrue( addedItem.IsVersionable );            

            // conflicted
            string otherWc = this.GetTempFile();
            Zip.ExtractZipResource( otherWc, this.GetType(), this.WC_FILE );
            try
            {
                using( StreamWriter w2 = new StreamWriter( Path.Combine(otherWc, "Form1.cs") ) )
                    w2.WriteLine( "Something else" );
                this.Client.Commit( new string[]{ otherWc }, Recurse.Full );
            }
            finally
            {
                Utils.PathUtils.RecursiveDelete( otherWc );
            }
            this.Client.Update( this.WcPath, Revision.Head, Recurse.Full );
            item1.Refresh( this.Client );
            Assert.AreEqual( StatusKind.Conflicted, item1.Status.TextStatus );
            Assert.IsTrue( item1.IsVersionable );

            // deleted
            this.Client.Resolved( item1.Path, Recurse.None );
            this.Client.Revert( new string[]{item1.Path}, Recurse.None );
            this.Client.Delete( new string[]{ item1.Path }, true );
            item1.Refresh( this.Client );
            Assert.AreEqual( StatusKind.Deleted, item1.Status.TextStatus );
            Assert.IsTrue( item1.IsVersionable );

            // unversioned
            string unversionedFile = Path.Combine( this.WcPath, "nope.txt" );
            File.CreateText(unversionedFile).Close();
            SvnItem unversioned = new SvnItem(unversionedFile, this.Client.SingleStatus(unversionedFile));
            Assert.AreEqual( StatusKind.Unversioned, unversioned.Status.TextStatus );
            Assert.IsTrue( unversioned.IsVersionable );

            // none
            string nonePath = Path.GetTempFileName();
            SvnItem none = new SvnItem(nonePath, this.Client.SingleStatus(nonePath));
            Assert.AreEqual( StatusKind.None, none.Status.TextStatus );
            Assert.IsFalse( none.IsVersionable );

            // subdir of wc
            string newFolder = Path.Combine( this.WcPath, "NewFolder" );
            Directory.CreateDirectory( newFolder );
            SvnItem folder = this.GetItem( newFolder );
            Assert.IsTrue( folder.IsVersionable );

            // subdir of unversioned folder
            string anotherFolder = Path.Combine( newFolder, "AnotherFolder" );
            Directory.CreateDirectory( anotherFolder );
            SvnItem folder2 = this.GetItem( anotherFolder );
            Assert.IsFalse( folder2.IsVersionable );

            // file in unversioned folder
            string subFilePath = Path.Combine( newFolder, "NewFile.txt" );
            File.CreateText( subFilePath );
            SvnItem subFile = this.GetItem( subFilePath );
            Assert.IsFalse( subFile.IsVersionable );
        }


        /// <summary>
        /// Tests the static Filter method.
        /// </summary>
        [Test]
        public void TestFilter()
        {
            SvnItem item1 = this.GetItem( this.WcPath );
            SvnItem item2 = this.GetItem( Path.Combine(this.WcPath, "Form1.cs" ));
            SvnItem item3 = this.GetItem( Path.Combine(this.WcPath, "AssemblyInfo.cs" ));

            IList items = SvnItem.Filter( new SvnItem[]{item1, item2, item3}, 
                new ResourceFilterCallback(this.AllowAllFilter) );
            Assert.AreEqual( 3, items.Count );

            items = SvnItem.Filter( new SvnItem[]{item1, item2, item3},
                new ResourceFilterCallback(this.OnlyDirectoriesFilter) );
            Assert.AreEqual( 1, items.Count );
            Assert.AreEqual( ((SvnItem)items[0]).Path, this.WcPath );
        }


        /// <summary>
        /// Tests the static GetPaths method.
        /// </summary>
        [Test]
        public void TestGetPaths()
        {
            SvnItem item1 = this.GetItem( this.WcPath );
            SvnItem item2 = this.GetItem( Path.Combine(this.WcPath, "Form1.cs" ));
            SvnItem item3 = this.GetItem( Path.Combine(this.WcPath, "AssemblyInfo.cs" ));

            string[] paths = SvnItem.GetPaths(new SvnItem[]{item1, item2, item3});
            Assert.AreEqual( 3, paths.Length );
            Assert.IsTrue( Array.IndexOf( paths, this.WcPath ) >= 0 );
            Assert.IsTrue( Array.IndexOf( paths, Path.Combine(this.WcPath, "Form1.cs" ) ) >= 0 );
            Assert.IsTrue( Array.IndexOf( paths, Path.Combine(this.WcPath, "Form1.cs" ) ) >= 0 );
        }
        
        /// <summary>
        /// Test the ToString method.
        /// </summary>
        [Test]
        public void TestToString()
        {
            SvnItem item = this.GetItem();
            Assert.AreEqual( item.Path, item.ToString() );
        }

        private SvnItem GetItem()
        {
            string path = Path.Combine( this.WcPath, "Form1.cs" );
            return GetItem( path );            
        }

        private SvnItem GetItem( string path )
        {
            Status status = this.Client.SingleStatus( path );
            return new SvnItem( path, status );  
        }


        private void ItemChanged(object sender, EventArgs e)
        {
            this.itemChanged = true;
        }

        private bool AllowAllFilter( SvnItem item )
        {
            return true;
        }

        private bool OnlyDirectoriesFilter( SvnItem item )
        {
            return item.IsDirectory;
        }

        private void NullStatusCallback( string s, Status s2 )
        {
            // empty
        }


        private void Cancel(object sender, CancelEventArgs args)
        {
            // we'll cancel after 30 callbacks
            this.counter++;
            if ( this.counter > 30 )
                args.Cancel = true;
        }

        private int counter = 0;
        private bool itemChanged = false;

    }
}
