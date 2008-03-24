// $Id$
using System;
using Ankh.Commands;
using System.IO;
using NUnit.Framework;
using EnvDTE;
using AnkhSvn.Ids;

namespace Ankh.Tests
{
    /// <summary>
    /// Tests the BlameCommand class.
    /// </summary>
    [TestFixture]
    public class BlameCommandTest : NSvn.Core.Tests.TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp ();
            this.ExtractRepos();
            this.ExtractWorkingCopy();

            this.cmd = new BlameCommand();

            this.uiShell = new MyUIShellImpl();
            this.ctx = new ContextBase( );
            this.uiShell.Context = this.ctx;
            this.ctx.UIShell = this.uiShell;
        }

        [Test]
        public void TestQueryStatus()
        {
            /*
            // not enabled for an empty selection
            Assert.AreEqual( vsCommandStatus.vsCommandStatusSupported, 
                cmd.QueryStatus( this.ctx ) );

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

            // not enabled for multiple items
            string path2 = Path.Combine( this.WcPath, "Class2.cs" );
            SvnItem item2 = this.ctx.StatusCache[path2];
            this.explorer.Selection = new SvnItem[]{ item, item2 };
            Assert.AreEqual( vsCommandStatus.vsCommandStatusSupported, 
                cmd.QueryStatus( this.ctx ) );

            // not enabled for folders
            SvnItem folder = this.ctx.StatusCache[this.WcPath];
            this.explorer.Selection = new SvnItem[]{ folder };
            Assert.AreEqual( vsCommandStatus.vsCommandStatusSupported, 
                cmd.QueryStatus( this.ctx ) );
             */
        }

        [Test]
        public void TestSimpleBlame()
        {
            string path = Path.Combine( this.WcPath, "Class1.cs" );
            SvnItem item = this.ctx.StatusCache[path];

            this.cmd.OnExecute(new CommandEventArgs(AnkhCommand.Blame, this.ctx));
            Assert.IsTrue( this.uiShell.Called );
            Assert.IsFalse( this.uiShell.Html == String.Empty );
            Assert.IsFalse( this.uiShell.Reuse );
            Assert.IsTrue( this.uiShell.ProgressDialogCalled );
            Assert.IsTrue( this.uiShell.ShowPathSelectorCalled );
            Assert.AreEqual( this.uiShell.Caption, "Class1.cs" );

        }

        private class MyUIShellImpl : ContextBase.UIShellImpl
        {
            public override void DisplayHtml(string caption, string html, bool reuse)
            {
                this.Called = true;     
                this.Caption = caption;
                this.Html = html;
                this.Reuse = reuse;
            }

            public override bool RunWithProgressDialog(IProgressWorker worker, string caption)
            {
                this.ProgressDialogCalled = true;
                return base.RunWithProgressDialog (worker, caption);
            }

            public override PathSelectorInfo ShowPathSelector(PathSelectorInfo info)
            {
                this.ShowPathSelectorCalled = true;
                return info;
            }



            public bool ShowPathSelectorCalled = false;
            public bool ProgressDialogCalled = false;
            public bool Called = false;
            public string Caption ;
            public string Html;
            public bool Reuse;
        }

        private MyUIShellImpl uiShell;
        private ContextBase ctx;
    }
}
