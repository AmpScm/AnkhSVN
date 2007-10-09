using System;
using NUnit.Framework;
using EnvDTE;

namespace Ankh.Tests
{
	/// <summary>
	/// Test fixture for the OutputPaneWriter class.
	/// </summary>
	[TestFixture]
	public class OutputPaneWriterTest
	{
        /// <summary>
        /// 
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.factory = DteFactory.Create2003();
        }
        /// <summary>
        /// Test that the output pane is being created properly.
        /// </summary>
        [Test]
        public void TestCreation()
        {
            _DTE dte = this.factory.Create();
            OutputPaneWriter writer = new OutputPaneWriter( dte, "Hello world" );
            Assert.IsNotNull( this.FindPane( dte, "Hello world" ), "Output pane not found" );
        }

        [Test]
        public void TestOutput()
        {
            _DTE dte = this.factory.Create();
            OutputPaneWriter writer = new OutputPaneWriter( dte, "Test321" );
            writer.Write( "Hello world" );

            OutputWindowPane owp = this.FindPane( dte, "Test321" );
            TextSelection sel = owp.TextDocument.Selection;
            sel.SelectAll();
            Assert.AreEqual( "Hello world", sel.Text );
        }

        /// <summary>
        /// Test the Activate method.
        /// </summary>
        [Test]
        public void TestActivate()
        {
            _DTE dte = this.factory.Create();
            OutputPaneWriter writer = new OutputPaneWriter( dte, "Test123" );

            writer.Activate();
            
            Window win = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            OutputWindow ow = (OutputWindow)win.Object;
            Assert.AreEqual( "Test123", ow.ActivePane.Name );
        }

        [Test]
        public void TestClear()
        {
            _DTE dte = this.factory.Create();
            OutputPaneWriter writer = new OutputPaneWriter( dte, "Test243" );
            writer.Write( "Hello world" );
            writer.Clear();

            OutputWindowPane owp = this.FindPane( dte, "Test243" );
            TextSelection sel = owp.TextDocument.Selection;
            sel.SelectAll();
            Assert.AreEqual( "", sel.Text );
        }

        [Test]
        public void TestStartActionText()
        {
            _DTE dte = this.factory.Create();
            OutputPaneWriter writer = new OutputPaneWriter( dte, "Test243" );
            writer.StartActionText( "Foo" );

            OutputWindowPane owp = this.FindPane( dte, "Test243" );
            TextSelection sel = owp.TextDocument.Selection;
            sel.StartOfDocument( false );
            TextRanges tags = null;
            Assert.IsTrue( sel.FindPattern( @"\-+:Wh*Foo:Wh*\-+", 
                (int)vsFindOptions.vsFindOptionsRegularExpression, ref tags ) );

        }

        [Test]
        public void TestEndActionText()
        {
            _DTE dte = this.factory.Create();
            OutputPaneWriter writer = new OutputPaneWriter( dte, "Test243" );
            writer.EndActionText();

            OutputWindowPane owp = this.FindPane( dte, "Test243" );
            TextSelection sel = owp.TextDocument.Selection;
            sel.StartOfDocument( false );
            TextRanges tags = null;
            Assert.IsTrue( sel.FindPattern( @"\-+:Wh*Done:Wh*\-+", 
                (int)vsFindOptions.vsFindOptionsRegularExpression, ref tags ) );

        }


        private OutputWindowPane FindPane( _DTE dte, string caption )
        {
            Window win = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            OutputWindow ow = (OutputWindow)win.Object;

            foreach( OutputWindowPane owp in ow.OutputWindowPanes )
            {
                if ( owp.Name == caption )
                    return owp;
            }
            return null;
        }

        private DteFactory factory;
	}
}
