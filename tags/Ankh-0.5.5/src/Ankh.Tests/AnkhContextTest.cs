using System;
using System.IO;
using NUnit.Framework;
using EnvDTE;
using Ankh.UI;
using System.Windows.Forms;
using Utils;

namespace Ankh.Tests
{
	/// <summary>
	/// Tests for the Ankh.AnkhContext class.
	/// </summary>
    [TestFixture]
	public class AnkhContextTest : NSvn.Core.Tests.TestBase
	{
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            TestUtils.ToggleAnkh( false, "7.1" );
            this.dteFactory = DteFactory.Create2003();
            this.ExtractWorkingCopy();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown ();
            TestUtils.ToggleAnkh( true, "7.1" );
        }


        /// <summary>
        /// Test the SolutionIsOpen property.
        /// </summary>
        [Test]
        public void TestSolutionIsOpen()
        {
            IContext context = this.CreateContext();
            Assert.IsFalse( context.SolutionIsOpen );

            context.DTE.Solution.Open( this.Solution );
            
            Assert.IsTrue( context.SolutionIsOpen );

        }

        /// <summary>
        /// Test the AnkhLoadedForSolution property.
        /// </summary>
        [Test]
        public void TestAnkhLoadedForSolutionVersioned()
        {
            IContext context = this.CreateContext();
            Assert.IsFalse( context.AnkhLoadedForSolution );

            context.DTE.Solution.Open( this.Solution );

            Assert.IsTrue( context.AnkhLoadedForSolution );

            // Ankh.Load should be present
            Assert.IsTrue( File.Exists( Path.Combine( this.WcPath, "Ankh.Load" ) ) );
            Assert.IsFalse( File.Exists( Path.Combine( this.WcPath, "Ankh.NoLoad" ) ) );
        }

        /// <summary>
        /// Test the AnkhLoadedForSolution property.
        /// </summary>
        [Test]
        public void TestAnkhLoadedForSolutionUnversioned()
        {
            IContext context = this.CreateContext();
            Assert.IsFalse( context.AnkhLoadedForSolution );

            string solutionDir = this.GetTempFile();
            File.Delete( solutionDir );
            Zip.ExtractZipResource( solutionDir, this.GetType(), "Ankh.Tests.unversioned.zip" );

            context.DTE.Solution.Open( Path.Combine(solutionDir, "WindowsApplication.sln" ) );

            Assert.IsFalse( context.AnkhLoadedForSolution );

            // none of the files should be there
            Assert.IsFalse( File.Exists( Path.Combine( this.WcPath, "Ankh.Load" ) ) );
            Assert.IsFalse( File.Exists( Path.Combine( this.WcPath, "Ankh.NoLoad" ) ) );
        }

        /// <summary>
        /// Test denying an Ankh load
        /// </summary>
        [Test]
        public void TestAnkhLoadedForSolutionDenied()
        {
            IContext context = this.CreateContext();
            ((UIShell)context.UIShell).AllowLoad = DialogResult.No;

            context.DTE.Solution.Open( this.Solution );
            
            Assert.IsFalse( context.AnkhLoadedForSolution );

            // only the NoLoad file shoudl be there
            Assert.IsFalse( File.Exists( Path.Combine( this.WcPath, "Ankh.Load" ) ) );
            Assert.IsTrue( File.Exists( Path.Combine( this.WcPath, "Ankh.NoLoad" ) ) );
        }

        /// <summary>
        /// Tests cancelling an Ankh load
        /// </summary>
        [Test]
        public void TestAnkhLoadedForSolutionCancelled()
        {
            IContext context = this.CreateContext();
            ((UIShell)context.UIShell).AllowLoad = DialogResult.Cancel;

            context.DTE.Solution.Open( this.Solution );
            
            Assert.IsFalse( context.AnkhLoadedForSolution );

            // none of the files should exist
            Assert.IsFalse( File.Exists( Path.Combine( this.WcPath, "Ankh.Load" ) ) );
            Assert.IsFalse( File.Exists( Path.Combine( this.WcPath, "Ankh.NoLoad" ) ) );
        }

        /// <summary>
        /// Test the OperationRunning property.
        /// </summary>
        [Test]
        public void TestOperationRunning()
        {
            IContext context = this.CreateContext();
            Assert.IsFalse( context.OperationRunning );

            context.StartOperation( "Dummy operation" );
            Assert.IsTrue( context.OperationRunning );

            context.EndOperation();
            Assert.IsFalse( context.OperationRunning );
        }

        /// <summary>
        /// Test the unloading event.
        /// </summary>
        [Test]
        public void TestUnloading()
        {
            Assert.IsFalse( this.unloadingCalled );

            IContext context = this.CreateContext();
            context.Unloading +=new EventHandler(context_Unloading);
            context.DTE.Solution.Open( this.Solution );
            context.Shutdown();

            Assert.IsTrue( this.unloadingCalled );
        }




        private string Solution
        {
            get{ return Path.Combine( this.WcPath, "WindowsApplication.sln" ); }
        }

        /// <summary>
        /// Creates an AnkhContext object.
        /// </summary>
        /// <returns></returns>
        private AnkhContext CreateContext()
        {
            _DTE dte = this.dteFactory.Create();
            AddIn ankh = TestUtils.GetAddin( "Ankh", dte );
            return new AnkhContext( dte, ankh, new UIShell(), new ErrorHandler() );
        }

        private void context_Unloading(object sender, EventArgs e)
        {
            this.unloadingCalled = true;
        }

        #region UIShell class
        private class UIShell : IUIShell
        {
            public RepositoryExplorerControl RepositoryExplorer
            {
                get{ return new RepositoryExplorerControl(); }
            }

            public IContext Context
            {
                get{ return null; }
                set{ ; }
            }

            public DialogResult QueryWhetherAnkhShouldLoad()
            {
                return AllowLoad;
            }

            public void SetRepositoryExplorerSelection(object[] selection)
            {
                // TODO:  Add UIShell.SetRepositoryExplorerSelection implementation
            }

            public void ShowRepositoryExplorer(bool show)
            {
                // TODO:  Add UIShell.ShowRepositoryExplorer implementation
            }

            public DialogResult AllowLoad = DialogResult.Yes;
        }
        #endregion

        #region ErrorHandler class
        private class ErrorHandler : IErrorHandler
        {
            #region IErrorHandler Members

            public void Handle(Exception ex)
            {
                Assert.Fail( ex.Message + Environment.NewLine + ex.StackTrace );
            }

            #endregion

        }
        #endregion


        private DteFactory dteFactory;
        private bool unloadingCalled = false;

        
    }
}
