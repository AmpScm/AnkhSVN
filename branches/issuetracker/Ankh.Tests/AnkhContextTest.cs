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
using System.IO;
using NUnit.Framework;
using EnvDTE;
using Ankh.UI;
using System.Windows.Forms;
using TestUtils;
using System.ComponentModel.Design;

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

            ((IDTEContext)context).DTE.Solution.Open( this.Solution );
            
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

            ((IDTEContext)context).Solution.Open(this.Solution);

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

            ((IDTEContext)context).Solution.Open(Path.Combine(solutionDir, "WindowsApplication.sln"));

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

            ((IDTEContext)context).Solution.Open(this.Solution);
            
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

            ((IDTEContext)context).Solution.Open(this.Solution);
            
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
            ((IDTEContext)context).Solution.Open(this.Solution);
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
            return new AnkhContext( dte, ankh, new UIShell() );
        }

        private void context_Unloading(object sender, EventArgs e)
        {
            this.unloadingCalled = true;
        }

        #region UIShell class
        private class UIShell : ContextBase.UIShellImpl
        {
            public override RepositoryExplorerControl RepositoryExplorer
            {
                get{ return new RepositoryExplorerControl(); }
            }           

            public override DialogResult QueryWhetherAnkhShouldLoad()
            {
                return AllowLoad;
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

            public void SendReport()
            {
                Assert.Fail( "Should not be called, I guess" );
            }

            public void Write( string message, Exception ex, TextWriter writer )
            {
                // empty
            }

            #endregion


            #region IErrorHandler Members

            public void LogException( Exception exception, string message, params object[] args )
            {
                throw new Exception( "The method or operation is not implemented." );
            }

            #endregion
}
        #endregion


        private DteFactory dteFactory;
        private bool unloadingCalled = false;

        
    }
}
