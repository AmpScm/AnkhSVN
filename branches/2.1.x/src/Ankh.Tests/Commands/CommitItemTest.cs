// $Id$
//
// Copyright 2005-2008 The AnkhSVN Project
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
using System.IO;
using NUnit.Framework;
using NSvn.Core;
using NSvn.Common;
using EnvDTE;
using Ankh.Commands;
using System.Collections;

namespace Ankh.Tests
{
    /// <summary>
    /// Test for the CommitItemCommand class.
    /// </summary>
    [TestFixture]
    public class CommitItemTest : NSvn.Core.Tests.TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractWorkingCopy();
            this.ExtractRepos();   
            TestUtils.ToggleAnkh( false, "7.1" );

            this.uiShell = new MyUIShellImpl();
            this.ctx = new MyContext( );
            this.uiShell.Context = this.ctx;
            this.explorer = new ContextBase.ExplorerImpl( this.ctx );            
            this.ctx.UIShell = this.uiShell;
            this.ctx.SolutionExplorer = this.explorer;

            this.cmd = new CommitItemCommand();

        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            TestUtils.ToggleAnkh( true, "7.1" );
        }
        
        [Test]
        public void TestQueryStatus()
        {            
            string path = Path.Combine( this.WcPath, "Class1.cs" );
            SvnItem item = this.ctx.StatusCache[path];
            Assert.AreEqual( StatusKind.Normal, item.Status.TextStatus );

            // modify the item
            this.Modify( path );
            item.Refresh( this.ctx.Client );
            Assert.AreEqual( StatusKind.Modified, item.Status.TextStatus );      

            // commit should not be enabled for no selection
            Assert.AreEqual( vsCommandStatus.vsCommandStatusSupported, 
                this.cmd.QueryStatus( this.ctx ) );

            this.explorer.Selection = new SvnItem[]{ item };

            // check that Commit is enabled on this modified item
            Assert.AreEqual( vsCommandStatus.vsCommandStatusEnabled |
            vsCommandStatus.vsCommandStatusSupported, this.cmd.QueryStatus( this.ctx ) );      
        }

        [Test]
        public void TestCommitSingleFile()
        {
            string path = Path.Combine( this.WcPath, "Class1.cs" );
            this.Modify( path );
            SvnItem item = this.ctx.StatusCache[path];
            int oldRev = item.Status.Entry.Revision;

            this.uiShell.CommitItems = this.explorer.Selection = new object[]{ item };
            this.uiShell.LogMessage = "42";

            this.cmd.Execute( this.ctx, "" );

            Assert.IsTrue( this.uiShell.ShowCommitDialogModalCalled );

            item.Refresh( this.ctx.Client );
            Assert.AreEqual( StatusKind.Normal, item.Status.TextStatus );

            ctx.Client.Log( new string[]{ item.Path }, Revision.Head, Revision.Head, false, false,
                new LogMessageReceiver( this.Receiver ) );

            Assert.AreEqual( "42", this.message.Message );
        }

        [Test]
        public void TestCommitAddedFile()
        {
            string path = Path.Combine( this.WcPath, "NewFile.cs" );
            this.Modify( path );
            this.ctx.Client.Add( path, Recurse.None );

            SvnItem item = this.ctx.StatusCache[path];
            Assert.AreEqual( StatusKind.Added, item.Status.TextStatus );

            this.uiShell.CommitItems = this.explorer.Selection = new object[]{ item };
            this.uiShell.LogMessage = "42";

            this.cmd.Execute( this.ctx, "" );

            Assert.IsTrue( this.uiShell.ShowCommitDialogModalCalled );

            item.Refresh( this.ctx.Client );
            Assert.AreEqual( StatusKind.Normal, item.Status.TextStatus );

            ctx.Client.Log( new string[]{ item.Path }, Revision.Head, Revision.Head, false, false,
                new LogMessageReceiver( this.Receiver ) );

            Assert.AreEqual( "42", this.message.Message );
        }

        /// <summary>
        /// Commits items from two different repositories
        /// </summary>
        [Test]
        public void CommitMultipleRepositories()
        {
            // we need a second repos
            string repos2Path = Path.Combine( @"\tmp", "repos2" );
            string repos2Url = ExtractRepos( "Ankh.Tests.repos2.zip", repos2Path, this.GetType() );
            string wc2Path = this.FindDirName( Path.Combine(@"\tmp", "wc2") );
            this.RunCommand( "svn", String.Format( "co {0} {1}", repos2Url, wc2Path ) );
            string path1 = Path.Combine( wc2Path, "AssemblyInfo.cs" );
            this.Modify( path1 );
            SvnItem item1 = this.ctx.StatusCache[path1];
            
            string path2 = Path.Combine( this.WcPath, "Class1.cs" );
            this.Modify( path2 );
            SvnItem item2 = this.ctx.StatusCache[path2];

            Assert.AreEqual( item1.Status.TextStatus, StatusKind.Modified );
            Assert.AreEqual( item2.Status.TextStatus, StatusKind.Modified );

            this.uiShell.CommitItems = new SvnItem[]{ item1, item2 };
            this.cmd.Execute( this.ctx, "" );

            Assert.AreEqual( item1.Status.TextStatus, StatusKind.Normal );
            Assert.AreEqual( item2.Status.TextStatus, StatusKind.Normal );

            Assert.IsTrue( this.ctx.Description.IndexOf( item1.Status.Entry.Uuid ) >= 0 );
            Assert.IsTrue( this.ctx.Description.IndexOf( item2.Status.Entry.Uuid ) >= 0 );
            
        }

        private void Modify( string path )
        {
            using( StreamWriter w = new StreamWriter( path ) )
                w.WriteLine( "Hi" );
        }

        private void Receiver( LogMessage message )
        {
            this.message = message;
        }


        /// <summary>
        /// Helper method refactored out of the test methods.
        /// </summary>
        /// <returns></returns>
        private ContextBase CreateContextAndLoad()
        {
            
            return ctx;
        }

        private UIHierarchy UIHierarchy
        {
            get
            {
                return (UIHierarchy)this.ctx.DTE.Windows.Item( 
                    Constants.vsWindowKindSolutionExplorer ).Object; 
            } 
        }

        private class MyContext : ContextBase
        {
            public override void StartOperation(string description)
            {
                this.Description += description;
            }

            public string Description = "";

        }

        private class MyUIShellImpl : ContextBase.UIShellImpl
        {
            public override CommitContext ShowCommitDialogModal(CommitContext ctx)
            {
                ctx.LogMessage = this.LogMessage;
                ctx.Cancelled = this.Cancelled;
                ctx.CommitItems = this.CommitItems;

                this.ShowCommitDialogModalCalled = true;

                return ctx;
            }

            public bool ShowCommitDialogModalCalled = false;
            public string LogMessage = "";
            public bool Cancelled = false;
            public IList CommitItems = new object[]{};
        }        

        private MyContext ctx;
        private ContextBase.ExplorerImpl explorer;
        private MyUIShellImpl uiShell;
        private CommitItemCommand cmd;
        private LogMessage message;
    }
}
