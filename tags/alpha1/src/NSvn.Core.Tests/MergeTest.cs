// $Id$
using NUnit.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;

//TODO: Not possible to do the testing yet as the repository
//      contains only one revision.
namespace NSvn.Core.Tests 
{
    /// <summary>
    /// Tests the NSvn.Client.MoveFile method
    /// </summary>
    [TestFixture]
    public class MergeTest : TestBase 
    {
        [SetUp]
        public override void SetUp() 
        {
            base.SetUp();

            this.ExtractRepos();
            this.ExtractWorkingCopy(); 
        }

        /// <summary>
        /// Tests rolling back a change with merge
        /// </summary>
        //TODO: Not possible to do the testing yet as the repository
        //      contains only one revision.
        [Test]
        public void TestMergeRollBack() 
        {
            string srcPath = this.ReposUrl;
            string dstPath = this.WcPath;
            string comparePath = Path.Combine (this.WcPath, @"doc/text_r5.txt" );            

            ClientContext ctx = new ClientContext() ;
            Client.Merge( srcPath, Revision.FromNumber(5) , srcPath, Revision.FromNumber(4) ,
                 dstPath, true, true, false, false, ctx ); 

            Assertion.AssertEquals( "Wrong status", 'D', this.GetSvnStatus( comparePath ) );
       
        }

        /// <summary>
        /// Tests moving changes with merge
        /// </summary>
        [Ignore( "Not implemented yet" )]
        [Test]
        public void TestMergeChanges() 
        {
                  
        }   

        
    }
}
