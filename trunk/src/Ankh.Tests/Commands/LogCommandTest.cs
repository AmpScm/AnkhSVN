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

// $Id$
using System;
using NSvn.Core;
using NUnit.Framework;
using EnvDTE;
using Ankh.Commands;
using System.IO;
using AnkhSvn.Ids;

namespace Ankh.Tests
{
    /// <summary>
    /// Tests for the LogCommand class.
    /// </summary>
    [TestFixture]
    public class LogCommandTest : NSvn.Core.Tests.TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractWorkingCopy();
            this.ExtractRepos();

            this.cmd = new LogCommand();

            this.uiShell = new MyUIShellImpl();
            this.ctx = new ContextBase( );
            this.uiShell.Context = this.ctx;
            this.ctx.UIShell = this.uiShell;
        }

        [Test]
        public void TestQueryStatus()
        {
            /*
            // should not be enabled for no selection at all
            Assert.AreEqual( vsCommandStatus.vsCommandStatusSupported, 
                this.cmd.QueryStatus( this.ctx ) );

            // 1 item selection - should be enabled
            string path = Path.Combine( this.WcPath, "Class1.cs" );
            SvnItem item = this.ctx.StatusCache[path];
            this.explorer.Selection = new SvnItem[]{ item };

            Assert.AreEqual( vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled,
                cmd.QueryStatus( this.ctx ) );

            // not enabled for an unversioned file
            string unversioned = Path.Combine( this.WcPath, "Unversioned.txt" );
            using( StreamWriter w = new StreamWriter( unversioned ) )
                w.WriteLine( "Hi" );

            SvnItem unversionedItem = this.ctx.StatusCache[unversioned];
            this.explorer.Selection = new SvnItem[]{ unversionedItem };

            Assert.AreEqual( vsCommandStatus.vsCommandStatusSupported, 
                cmd.QueryStatus( this.ctx ) );

            // enabled for multiple items
            string path2 = Path.Combine( this.WcPath, "Class2.cs" );
            SvnItem item2 = this.ctx.StatusCache[path2];
            this.explorer.Selection = new SvnItem[]{ item, item2 };
            Assert.AreEqual( vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled, 
                cmd.QueryStatus( this.ctx ) );

            // enabled for folders
            SvnItem folder = this.ctx.StatusCache[this.WcPath];
            this.explorer.Selection = new SvnItem[]{ folder };
            Assert.AreEqual( vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled, 
                cmd.QueryStatus( this.ctx ) );            
             */
        }

        [Test]
        public void TestLogSingleItem()
        {
            // single item
            string path = Path.Combine( this.WcPath, "Class1.cs" );
            SvnItem item = this.ctx.StatusCache[path];

            this.cmd.OnExecute(new CommandEventArgs(AnkhCommand.Log, this.ctx));

            Assert.IsTrue( this.uiShell.ProgressDialogCalled );

            Assert.IsNotNull( this.uiShell.Info );
            Assert.AreEqual( 1, this.uiShell.Info.Items.Count );
            Assert.AreSame( item, this.uiShell.Info.Items[0] );

            Assert.IsNotNull( this.uiShell.Html );
            
        }
         

        private class MyUIShellImpl : ContextBase.UIShellImpl
        {
            public override bool RunWithProgressDialog(IProgressWorker worker, string caption)
            {
                this.ProgressDialogCalled = true;
                return base.RunWithProgressDialog( worker, caption );
            }

            public override LogDialogInfo ShowLogDialog(LogDialogInfo info)
            {
                this.Info = info;
                return info;
            }

            public override void DisplayHtml(string caption, string html, bool reuse)
            {
                this.Html = html;
            }


            public string Html = null;
            public LogDialogInfo Info = null;
            public bool ProgressDialogCalled = false;

        }

        private MyUIShellImpl uiShell;
        private ContextBase ctx;
    }
}
