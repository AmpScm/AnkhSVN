// $Id$
using System;
using NSvn.Common;
using NSvn.Core;
using NUnit.Framework;
using System.IO;

namespace NSvn.Tests
{
	/// <summary>
	/// Tests the UnversionedItem class.
	/// </summary>
	[TestFixture]
	public class UnversionedResourceTest : TestBase
	{

        public override void SetUp()
        {            
            this.ExtractWorkingCopy();
            this.newFile = Path.Combine( this.WcPath, "newfile.txt" );
            this.newDir = Path.Combine( this.WcPath, "newdir" );
            this.fileInNewDir = Path.Combine( newDir, "file.txt" );

            this.ResetNotificationCount();
        }

        [Test]
        public void TestInstanceAddFile()
        {           
            using( StreamWriter w = File.CreateText( newFile ) )
                w.Write( "MOO" );

            UnversionedResource item = new UnversionedFile( this.newFile );
            
            //TODO: fix this
            // this.SetupEventHandlers( item.Notifications );

            //TODO: fix this
//            Assertion.AssertEquals( "Expected only one notification", 1,
//                this.NotificationCount );
            WorkingCopyResource wcItem = item.Add( false );
            Assertion.AssertEquals( "Wrong type returned. Should be working copy file",
                typeof( WorkingCopyFile ), wcItem.GetType() );
            Assertion.AssertEquals( "File not added", StatusKind.Added, 
                wcItem.Status.TextStatus );
        }

        [Test]
        public void TestStaticAddDir()
        {
            Directory.CreateDirectory( this.newDir );
            using ( StreamWriter w = File.CreateText(this.fileInNewDir) )
                w.Write( "MOO" );

            WorkingCopyResource dir = UnversionedResource.Add( this.newDir, true );

            Assertion.AssertEquals( "Wrong type returned. Should be working copy dir",
                typeof( WorkingCopyDirectory ), dir.GetType() );
            Assertion.AssertEquals( "Directory not added", StatusKind.Added,
                dir.Status.TextStatus );

            WorkingCopyFile file = new WorkingCopyFile( this.fileInNewDir );
            Assertion.AssertEquals( "File in subdirectory not added", 
                StatusKind.Added, file.Status.TextStatus );
        }

       
        private string newFile;
        private string newDir;
        private string fileInNewDir;
	}
}
