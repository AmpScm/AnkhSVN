using System;
using EnvDTE;
using NUnit.Framework;
using System.Collections;
using Ankh.Solution;
using System.IO;

namespace Ankh.Tests
{
    /// <summary>
    /// Tests for the Ankh.Solution.Explorer class
    /// </summary>
    [TestFixture]
    public class SolutionExplorerTest : NSvn.Core.Tests.TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractWorkingCopy();
            this.dteFactory = DteFactory.Create2003();
            TestUtils.ToggleAnkh( false, "7.1" );

        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            TestUtils.ToggleAnkh( true, "7.1" );
        }

        [Test]
        public void TestGetSelectionResources()
        {
            Explorer expl = this.LoadExplorer();

            // select the solution node
            this.UIHierarchy.UIHierarchyItems.Item(1).Select( 
                vsUISelectionType.vsUISelectionTypeSelect );
            IList lst = expl.GetSelectionResources( false, this.Blanco );
            Assert.AreEqual( 2, lst.Count );

            SvnItem dir = this.GetFirstDir( lst );
            Assert.AreEqual( this.WcPath, dir.Path );

            SvnItem file = this.GetFirstFile( lst );
            Assert.AreEqual( Path.Combine( 
                this.WcPath, "WindowsApplication.sln" ), file.Path );

            // solution and project
            this.UIHierarchy.UIHierarchyItems.Item(1).UIHierarchyItems.Item(1).Select(
                vsUISelectionType.vsUISelectionTypeExtend );
            lst = expl.GetSelectionResources( false, this.Blanco );
            Assert.AreEqual( 4, lst.Count );

            // Form file
            this.UIHierarchy.UIHierarchyItems.Item(1).UIHierarchyItems.Item(1).UIHierarchyItems.Item(7).Select(
                vsUISelectionType.vsUISelectionTypeSelect );
            lst = expl.GetSelectionResources( false, this.Blanco );
            Assert.IsTrue( this.FindPathComponent( "Form1.cs", lst ) );
            Assert.IsTrue( this.FindPathComponent( "Form1.resx", lst ) );
            Assert.AreEqual( 2, lst.Count );
        }


        /// <summary>
        /// Test that the solution actually gets unloaded.
        /// </summary>
        [Test]
        public void TestUnload()
        {
            Explorer expl = this.LoadExplorer();
            expl.Unload();

            this.UIHierarchy.UIHierarchyItems.Item(1).Select(
                vsUISelectionType.vsUISelectionTypeSelect );

            IList lst = expl.GetSelectionResources( true, this.Blanco );
            Assert.AreEqual( 0, lst.Count );
        }


        [Test]
        public void TestRefreshSelection()
        {
            Explorer expl = this.LoadExplorer();

            using( StreamWriter w = new StreamWriter( Path.Combine(this.WcPath, "Class1.cs") ))
                w.WriteLine( "Foo" );

            this.UIHierarchy.UIHierarchyItems.Item(1).Select( 
                vsUISelectionType.vsUISelectionTypeSelect );

            IList lst = expl.GetSelectionResources( true, this.Modified );

            // without a refresh, there should be no modified items
            Assert.AreEqual( 0, lst.Count );

            expl.RefreshSelection();

            lst = expl.GetSelectionResources( true, this.Modified );
            Assert.AreEqual( 1, lst.Count );

        }

        

        /// <summary>
        /// Test the RefreshSelectionParents method.
        /// </summary>
        [Test]
        public void TestRefreshSelectionParents()
        {
            Explorer expl = this.LoadExplorer();
            File.CreateText( Path.Combine( this.WcPath, "uggabugga.txt" ) );

            this.Dte.Solution.Projects.Item(1).ProjectItems.AddFromFile( 
                Path.Combine( this.WcPath, "uggabugga.txt" ) );
            UIHierarchyItem item = 
                this.UIHierarchy.UIHierarchyItems.Item(1).UIHierarchyItems.Item(1).
                UIHierarchyItems.Item(10);
            Assert.AreEqual( "uggabugga.txt", item.Name );

            // verify that it isn't recognized by SVN now
            this.UIHierarchy.UIHierarchyItems.Item(1).Select( 
                vsUISelectionType.vsUISelectionTypeSelect );
            IList list = expl.GetSelectionResources( true, this.Blanco );
            int count = list.Count;
            Assert.IsFalse( this.FindPathComponent( "uggabugga.txt", list ) );
          
            // refresh the parent after selecting another item
            this.UIHierarchy.SelectUp( vsUISelectionType.vsUISelectionTypeSelect, 1 );
            expl.RefreshSelectionParents();

            // check if it's there now.
            this.UIHierarchy.UIHierarchyItems.Item(1).Select( 
                vsUISelectionType.vsUISelectionTypeSelect );
            list = expl.GetSelectionResources( true, this.Blanco );
            Assert.AreEqual( count + 1, list.Count );
            Assert.IsTrue( this.FindPathComponent( "uggabugga.txt", list ) );            
        }

        /// <summary>
        /// Test the Refresh(Project) method.
        /// </summary>
        [Test]
        public void TestRefreshProject()
        {
            Explorer expl = this.LoadExplorer();
            File.CreateText( Path.Combine( this.WcPath, "uggabugga.txt" ) );

            ProjectItem prjItem = this.Dte.Solution.Projects.Item(1).ProjectItems.AddFromFile( 
                Path.Combine( this.WcPath, "uggabugga.txt" ) );
            UIHierarchyItem item = 
                this.UIHierarchy.UIHierarchyItems.Item(1).UIHierarchyItems.Item(1).
                UIHierarchyItems.Item(10);
            Assert.AreEqual( "uggabugga.txt", item.Name );

            // verify that it isn't recognized by SVN now
            this.UIHierarchy.UIHierarchyItems.Item(1).Select( 
                vsUISelectionType.vsUISelectionTypeSelect );
            IList list = expl.GetSelectionResources( true, this.Blanco );
            int count = list.Count;
            Assert.IsFalse( this.FindPathComponent( "uggabugga.txt", list ) );

            // refresh the project
            expl.Refresh( prjItem.ContainingProject );

            // check if it's there now.
            this.UIHierarchy.UIHierarchyItems.Item(1).Select( 
                vsUISelectionType.vsUISelectionTypeSelect );
            list = expl.GetSelectionResources( true, this.Blanco );
            Assert.AreEqual( count + 1, list.Count );
            Assert.IsTrue( this.FindPathComponent( "uggabugga.txt", list ) );            
        }


        [Test]
        public void TestRefreshProjectItem()
        {
            Explorer expl = this.LoadExplorer();

            this.UIHierarchy.UIHierarchyItems.Item(1).UIHierarchyItems.Item(1).
                UIHierarchyItems.Item(6).Select( vsUISelectionType.vsUISelectionTypeSelect );

            using( StreamWriter writer = new StreamWriter( 
                       Path.Combine(this.WcPath, "Class3.cs") ) )
                writer.WriteLine( "Foo" );
            IList list = expl.GetSelectionResources( true, this.Modified );
            Assert.AreEqual( 0, list.Count );

            ProjectItem item = 
                this.Dte.Solution.Projects.Item(1).ProjectItems.Item( "Class3.cs" );
            expl.Refresh( item );

            list = expl.GetSelectionResources( true, this.Modified );
            Assert.AreEqual( 1, list.Count );
            Assert.IsTrue( this.FindPathComponent( "Class3.cs", list ) );
                
        }


        /// <summary>
        /// Test the SyncAll method.
        /// </summary>
        [Test]
        public void TestSyncAll()
        {
            Explorer expl = this.LoadExplorer();

            this.UIHierarchy.UIHierarchyItems.Item(1).Select( 
                vsUISelectionType.vsUISelectionTypeSelect );

            using( StreamWriter writer = new StreamWriter( 
                       Path.Combine(this.WcPath, "Class3.cs") ) )
                writer.WriteLine( "Foo" );
            IList list = expl.GetAllResources( this.Modified );
            Assert.AreEqual( 0, list.Count );

            expl.SyncAll();

            list = expl.GetAllResources( this.Modified );
            Assert.AreEqual( 1, list.Count );
            Assert.IsTrue( this.FindPathComponent( "Class3.cs", list ) );
        }

        [Test]
        public void TestGetItemResources()
        {
            Explorer expl = this.LoadExplorer();

            ProjectItem item = this.Dte.Solution.Projects.Item(1).ProjectItems.Item( "Form1.cs" );
            IList list = expl.GetItemResources( item, false );

            Assert.AreEqual( 2, list.Count );
            Assert.IsTrue( this.FindPathComponent( "Form1.cs", list ));
            Assert.IsTrue( this.FindPathComponent( "Form1.resx", list ));
        }


        /// <summary>
        /// Tests the GetSelectedProjectItem method.
        /// </summary>
        [Test]
        public void TestGetSelectedProjectItem()
        {
            Explorer expl = this.LoadExplorer();

            // no project item selected
            this.UIHierarchy.UIHierarchyItems.Item(1).Select(
                vsUISelectionType.vsUISelectionTypeSelect);
            ProjectItem prjItem = expl.GetSelectedProjectItem();
            Assert.IsNull( prjItem );

            // select one
            this.UIHierarchy.UIHierarchyItems.Item(1).
                UIHierarchyItems.Item(1).UIHierarchyItems.Item(2).Select(
                vsUISelectionType.vsUISelectionTypeSelect);
            prjItem = expl.GetSelectedProjectItem();
            
            ProjectItem prjItem2 = this.Dte.Solution.Projects.Item(1).ProjectItems.Item( 
                "App.ico" );
            Assert.AreEqual( prjItem.Name, prjItem2.Name );
        }

        #region private stuff
        /// <summary>
        /// TODO: Refactor
        /// </summary>
        private string Solution
        {
            get{ return Path.Combine( this.WcPath, "WindowsApplication.sln" ); }
        }

        private _DTE Dte
        {
            get
            { 
                if (this.dte == null) 
                    this.dte = this.dteFactory.Create();
                return this.dte;
            }
        }

        private UIHierarchy UIHierarchy
        {
            get
            {
                return (UIHierarchy)this.Dte.Windows.Item( 
                    Constants.vsWindowKindSolutionExplorer ).Object; 
            } 
        }


        private readonly ResourceFilterCallback Blanco = new ResourceFilterCallback(
            BlancoCallback );
        private static bool BlancoCallback( SvnItem item )
        {
            return true;
        }


        private readonly ResourceFilterCallback Modified = new ResourceFilterCallback(
            ModifiedCallback );
        private static bool ModifiedCallback( SvnItem item )
        {
            return item.IsModified;
        }
        private SvnItem GetFirstFile( IList items )
        {
            foreach( SvnItem item in items )
            {
                if ( item.IsFile )
                    return item;
            }

            return null;
        }

        private SvnItem GetFirstDir( IList items )
        {
            foreach( SvnItem item in items )
            {
                if ( item.IsDirectory )
                    return item;
            }

            return null;
        }

        private bool FindPathComponent( string component, IList items )
        {
            foreach( SvnItem item in items )
            {
                if ( item.Path.IndexOf( component ) >= 0 )
                    return true;
            }
            return false;
        }

        private Explorer LoadExplorer()
        {
            Explorer expl = new Explorer( this.Dte, new ContextBase() );
            this.Dte.Solution.Open( this.Solution );
            expl.Load();
            return expl;
        }
        #endregion


        private DteFactory dteFactory;
        private _DTE dte;

    }
}
