using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Ankh.Tools;

namespace TreeListTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Load += new EventHandler( Form1_Load );

            //this.treeList1.CheckBoxes = true;

            //this.treeList1.TreeListItems.Add( new TreeListItem( "A" ) ).Level = 1;
            //this.treeList1.TreeListItems.Add( new TreeListItem( "A" ) ).Level = 2;
            //this.treeList1.TreeListItems.Add( new TreeListItem( "A" ) ).Level = 3;
        }

        void Form1_Load( object sender, EventArgs e )
        {
            item1 = new TreeListItem( "A" );
            this.treeList1.Items.Add( item1 );
            item1.SubItems.Add( "Blah" );

            item2 = new TreeListItem( "B" );
            item2.SubItems.Add( "Blah" );

            item3 = new TreeListItem( "C" );
            item3.SubItems.Add( "Blah" );

            item4 = new TreeListItem( "D" );
            item4.SubItems.Add( "Blah" );

            item1.Children.Add( item2 );
            item1.Children.Add( item3 );
            

            item2.Children.Add( item4 );
        }



        private TreeListItem item1, item2, item3, item4;

        private void treeList1_SelectedIndexChanged( object sender, EventArgs e )
        {

        }

        private void treeList1_Click( object sender, EventArgs e )
        {
            
        }

        private void button1_Click( object sender, EventArgs e )
        {
            //this.treeList1.Items.Clear();
            TreeListItem item = new TreeListItem(this.textBox1.Text);
            this.FillRecursively( item, this.textBox1.Text );

            this.treeList1.Items.Add( item );
        }

        private void FillRecursively( TreeListItem item, string path )
        {
            DirectoryInfo info = new DirectoryInfo( path );
            foreach ( FileSystemInfo childInfo in info.GetFileSystemInfos() )
            {
                TreeListItem child = new TreeListItem( childInfo.Name );
                child.SubItems.Add( childInfo.LastWriteTime.ToString() );

                item.Children.Add( child );

                if ( (childInfo.Attributes & FileAttributes.Directory) != 0 )
                {
                    FillRecursively( child, childInfo.FullName );
                }
            }
        }

        private void button2_Click( object sender, EventArgs e )
        {
            if ( this.treeList1.SelectedItems.Count == 1 )
            {
                TreeListItem item = this.treeList1.SelectedItems[ 0 ] as TreeListItem;
                this.treeList1.Items.Remove(item);
            }
        }
    }
}