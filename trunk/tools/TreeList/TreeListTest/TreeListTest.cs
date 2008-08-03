using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Ankh.Tools;
using System.Diagnostics;
using System.Windows.Forms;

namespace TreeListTest
{
    [TestFixture]
    public class TreeListTest
    {
        [SetUp]
        public void SetUp()
        {
            this.treeList = new TreeList();
            this.baseList = ( (ListView)this.treeList ).Items;
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void BasicHierarchy()
        {
            this.treeList.Items.Add( a );
            a.Children.Add(b);
            b.Children.Add(c);
            b.Children.Add(d);

            Assert.AreEqual( 1, this.baseList.Count );
            Assert.AreEqual( "A", this.baseList[0].Text);

            a.Expanded = true;
            Assert.AreEqual( 2, this.baseList.Count );
            Assert.AreEqual( "B", this.baseList[ 1 ].Text );
            

            b.Expanded = true;
            Assert.AreEqual( 4, this.baseList.Count );
            Assert.AreEqual( "C", this.baseList[ 2 ].Text );
            Assert.AreEqual( "D", this.baseList[ 3 ].Text );
        }

        [Test]
        public void Add()
        {
            this.treeList.Items.Add( a );
            a.Children.Add( b );

            a.Expanded = true;

            this.treeList.Items.Add( c );
            Assert.AreEqual( 3, this.baseList.Count );
            Assert.AreEqual( "B", this.baseList[ 1 ].Text );
            Assert.AreEqual( "C", this.baseList[ 2 ].Text );
        }

        [Test]
        public void Remove()
        {
            this.treeList.Items.Add( a );
            a.Children.Add( b ); a.Children.Add( c );

            a.Expanded = true;
            this.treeList.Items.Add( d );

            this.treeList.Items.Remove( a );

            Assert.AreEqual( 1, this.baseList.Count );
            Assert.AreEqual( "D", this.baseList[ 0 ].Text );
        }



        private TreeList treeList;
        private ListView.ListViewItemCollection baseList;
        private TreeListItem a = new TreeListItem( "A" );
        private TreeListItem b = new TreeListItem( "B" );
        private TreeListItem c = new TreeListItem( "C" );
        private TreeListItem d = new TreeListItem("D");


    }
}
