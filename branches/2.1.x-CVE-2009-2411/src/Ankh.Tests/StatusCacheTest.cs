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

using System;
using NUnit.Framework;
using Ankh;
using System.IO;
using System.Collections;
using SharpSvn;


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
            StatusCache cache = new StatusCache();
            cache.Status(this.WcPath, SvnDepth.Infinity);

            string formPath = Path.Combine(this.WcPath, "Form1.cs");

            SvnItem item = cache[formPath];
            Assert.AreEqual( SvnStatus.Normal, item.Status.LocalContentStatus );

            item = cache[formPath.ToLower()];
			Assert.AreEqual(SvnStatus.Normal, item.Status.LocalContentStatus);

            item = cache[formPath.ToUpper()];
			Assert.AreEqual(SvnStatus.Normal, item.Status.LocalContentStatus);
        }

        /// <summary>
        /// Get the deletions from a directory.
        /// </summary>
        [Test]
        public void TestGetDeletions()
        {
            StatusCache cache = new StatusCache();
            this.Client.Delete(Path.Combine( this.WcPath, "Class1.cs" ));
            this.Client.Delete(Path.Combine( this.WcPath, "WindowsApplication.sln" ));
            
            // should be two deletions now
            cache.Status(this.WcPath, SvnDepth.Infinity);
            IList deletions = cache.GetDeletions( this.WcPath );
            Assert.AreEqual( 2, deletions.Count );

            // undelete one
            this.Client.Revert(Path.Combine( this.WcPath, "Class1.cs" ));
            deletions = cache.GetDeletions( this.WcPath );
            Assert.AreEqual( 1, deletions.Count );

            // this one should still be deleted
            Assert.AreEqual( Path.Combine( this.WcPath, "WindowsApplication.sln" ), 
                ((SvnItem)deletions[0]).Path);  

            // undelete all
			SvnRevertArgs a = new SvnRevertArgs();
			a.Depth = SvnDepth.Infinity;
            this.Client.Revert(this.WcPath, a);
            deletions = cache.GetDeletions( this.WcPath );
            Assert.AreEqual( 0, deletions.Count );
        }

        /// <summary>
        /// Test getting an item for a path not in the initial Status call
        /// </summary>
        [Test]
        public void TestGetPathNotInInitialStatus()
        {
            StatusCache cache = new StatusCache();
            cache.Status(Path.Combine(this.WcPath, "doc"), SvnDepth.Infinity);
            using( StreamWriter w = new StreamWriter(Path.Combine(this.WcPath, "Form1.cs")) )
                w.WriteLine( "Foo" );

            SvnItem item = cache[Path.Combine(this.WcPath, "Form1.cs")];
            Assert.AreEqual( SvnStatus.Modified, item.Status.LocalContentStatus );            
        }

        /// <summary>
        /// Test retrieving an unversioned item.
        /// </summary>
        [Test]
        public void TestGetUnversionedItem()
        {
            StatusCache cache = new StatusCache();

            string path = Path.GetTempFileName();
            SvnItem item = cache[path];
            Assert.IsFalse( item.IsVersionable );
        }
	}

}
