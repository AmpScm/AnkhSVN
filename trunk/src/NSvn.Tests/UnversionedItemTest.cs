using System;
using NSvn.Common;
using NSvn.Core;
using NSvn.Core.Tests;
using NUnit.Framework;
using System.IO;

namespace NSvn.Tests
{
	/// <summary>
	/// Tests the UnversionedItem class.
	/// </summary>
	[TestFixture]
	public class UnversionedItemTest : TestBase
	{

        public override void SetUp()
        {
            
            this.ExtractWorkingCopy();
            this.newFile = Path.Combine( this.WcPath, "newfile.txt" );
            this.newDir = Path.Combine( this.WcPath, "newdir" );
            this.fileInNewDir = Path.Combine( newDir, "file.txt" );
        }

        [Test]
        public void TestInstanceAddFile()
        {           
            using( StreamWriter w = File.CreateText( newFile ) )
                w.Write( "MOO" );

            UnversionedItem item = new UnversionedFile( this.newFile );
            WorkingCopyItem wcItem = item.Add( false );
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

            WorkingCopyItem dir = UnversionedItem.Add( this.newDir, true );

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
