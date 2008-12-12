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
using EnvDTE;
using Ankh.Config;
using Ankh.EventSinks;
using System.IO;
using System.Windows.Forms;

namespace Ankh.Tests
{
    /// <summary>
    /// Tests the ProjectItemsEventSink class
    /// </summary>
    [TestFixture]
    public class ProjectItemsEventSinkTest : NSvn.Core.Tests.TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            this.ExtractWorkingCopy();
        }

        /// <summary>
        /// Tests the ItemAdded method.
        /// </summary>
        [Test]
        public void TestItemAutoAdded()
        {
            ContextBase ctx = this.CreateContextAndLoad();

            string path = Path.Combine( this.WcPath, "File.cs" );
            File.CreateText( path ).Close();
      
            ctx.DTE.Solution.Projects.Item( 1 
                ).ProjectItems.AddFromFile( path );
            ctx.CheckForException();

            // verify that the file is correctly added.
            Assert.AreEqual( StatusKind.Added, 
                ctx.Client.SingleStatus(path).TextStatus );           
        }

        /// <summary>
        /// Tests that ignored files aren't auto added.
        /// </summary>
        [Test]
        public void TestAutoAddIgnoredFile()
        {
            ContextBase ctx = this.CreateContextAndLoad();            

            string path = Path.Combine( this.WcPath, "File.ignored" );
            File.CreateText( path ).Close();
            ctx.Client.PropSet( new Property( "svn:ignore", "File.ignored" ), 
                this.WcPath, Recurse.Full );

            int youngest;
            ctx.Client.Status( out youngest, path, Revision.Unspecified, 
                new StatusCallback( this.IgnoredFileCallback ), false, true, false, true );
            Assert.AreEqual( StatusKind.Ignored, 
                this.ignoredFileStatus.TextStatus ); 
      
            ctx.DTE.Solution.Projects.Item( 1 
                ).ProjectItems.AddFromFile( path );
            ctx.CheckForException();

            // verify that the file is *NOT* added.
            ctx.Client.Status( out youngest, path, Revision.Unspecified, 
                new StatusCallback( this.IgnoredFileCallback ), false, true, false, true );
            Assert.AreEqual( StatusKind.Ignored, 
                this.ignoredFileStatus.TextStatus );
        }

        /// <summary>
        /// This tests for a specific bug in which the sink failed
        /// to refresh the sink before checking its text status
        /// </summary>
        [Test]
        public void TestItemAutoAddedTwice()
        {
            ContextBase ctx = this.CreateContextAndLoad();

            string path = Path.Combine( this.WcPath, "File.cs" );
            File.CreateText( path ).Close();  
          
            SvnItem item = ctx.StatusCache[path];
            ctx.Client.Add( path, Recurse.None );

            // unless the sink refreshes the item, it will be 
            // seen as unversioned and it will try to add it.
            ctx.DTE.Solution.Projects.Item( 1 
                ).ProjectItems.AddFromFile( path );

            // verify that no exception was thrown
            ctx.CheckForException();
            

            // verify that the file is correctly added.
            Assert.AreEqual( StatusKind.Added, 
                ctx.Client.SingleStatus(path).TextStatus );   
        }

        [Test]
        public void TestNonVersionedMiscellaneousFileAdded()
        {
            ContextBase ctx = this.CreateContextAndLoad();
            string dir = Path.Combine( this.WcPath, "Unversioned" );
            Directory.CreateDirectory(dir);
            string file = Path.Combine( dir, "unversioned.txt" );
            File.CreateText( file ).Close();

            ctx.DTE.ExecuteCommand( "File.OpenFile", file );

            ctx.CheckForException();

            Assert.AreEqual( Status.None, 
                ctx.Client.SingleStatus(file) );
        }

        

        [Ignore("No idea how to programmatically add an URI")]
        [Test]
        public void TestAddFileUri()
        {
            ContextBase ctx = this.CreateContextAndLoad();

            string path = Path.Combine( this.WcPath, "File.cs" );
            File.CreateText( path ).Close();  

            string uri = "file:///" + path.Replace( "\\", "/" );
            ctx.DTE.Solution.Projects.Item( 1 
                ).ProjectItems.AddFromFile( uri );
            ctx.CheckForException();

            Assert.AreEqual( StatusKind.Added, 
                ctx.Client.SingleStatus(path).TextStatus );
        }

        /// <summary>
        /// Verifies that a file gets deleted by Ankh when deleted in the IDE
        /// </summary>
        [Test]
        public void TestRemoveFile()
        {
            ContextBase ctx = this.CreateContextAndLoad();

            ctx.DTE.Solution.Projects.Item(1).
                ProjectItems.Item( "Form1.cs" ).Delete();

            ctx.CheckForException();

            string path = Path.Combine( this.WcPath, "Form1.cs" );
            Assert.AreEqual( StatusKind.Deleted, 
                ctx.Client.SingleStatus(path).TextStatus );
        }

        /// <summary>
        /// Tests that Ankh correctly prevents the user from renaming a 
        /// versioned file.
        /// </summary>
        [Test]
        public void TestAttemptRenameFile()
        {
            ContextBase ctx = this.CreateContextAndLoad();
            MyUIShell myShell = new MyUIShell();
            ctx.uiShell = myShell;

            ctx.DTE.Solution.Projects.Item(1).
                ProjectItems.Item( "Form1.cs" ).Name = "Form.cs";
            ctx.CheckForException();

            Assert.IsTrue( myShell.MessageBoxShown );
            Assert.IsTrue( File.Exists( 
                Path.Combine( this.WcPath, "Form1.cs" ) ) );
            Assert.AreEqual( "Form1.cs", ctx.DTE.Solution.Projects.Item(1).
                ProjectItems.Item( "Form1.cs" ).Name );
        }

        /// <summary>
        /// make sure the .svn dir won't get added automatically
        /// </summary>
        [Test]
        public void TestAttemptAddSvnDirectory()
        {
            ContextBase ctx = this.CreateContextAndLoad();
            string svnDir = Path.Combine( this.WcPath, Client.AdminDirectoryName );
            
            ctx.dte.Solution.Projects.Item(1).ProjectItems.
                AddFromDirectory( svnDir );

            Status status = ctx.Client.SingleStatus(svnDir);
            Assert.IsTrue( 
                status.TextStatus == StatusKind.Unversioned );
        }

        /// <summary>
        /// Helper method refactored out of the test methods.
        /// </summary>
        /// <returns></returns>
        private ContextBase CreateContextAndLoad()
        {
            TestUtils.ToggleAnkh( false, "7.1" );

            ContextBase ctx = new ContextBase();

            ctx.DTE.Solution.Open( 
                Path.Combine( this.WcPath, "WindowsApplication.sln" ) );

            ctx.Config.AutoAddNewFiles = true;
            return ctx;
        }

        private void IgnoredFileCallback( string path, Status status )
        {
            this.ignoredFileStatus = status;
        }


        private class MyUIShell : ContextBase.UIShellImpl
        {
            public bool MessageBoxShown = false;
            public override DialogResult ShowMessageBox(string text, string caption, 
                MessageBoxButtons buttons, MessageBoxIcon icon)
            {
                this.MessageBoxShown = true;
                return DialogResult.OK;
            }
        }

        private Status ignoredFileStatus;
        
    }
}
