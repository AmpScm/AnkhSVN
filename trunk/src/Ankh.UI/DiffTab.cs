// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Ankh.UI
{
    /// <summary>
    /// A TabControl that displays unified diffs.
    /// </summary>
    public class DiffTab : System.Windows.Forms.TabControl
    {
        public event DiffWantedDelegate DiffWanted;

        public DiffTab()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        /// <summary>
        /// Add a new tab page.
        /// </summary>
        /// <param name="path"></param>
        public void AddPage( string path )
        {
            string filename = Path.GetFileName( path );

            DiffTabPage newPage = new DiffTabPage( filename );
            newPage.Tag = path;  
            this.Controls.Add( newPage );
            newPage.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                    components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion		

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged( e );

            InsertDiffView();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged( e );

            if ( this.Visible )
                InsertDiffView();
        }


        private void InsertDiffView()
        {
            // is there already a diffview there?
            if ( this.SelectedTab != null && !((DiffTabPage)this.SelectedTab).Initialized )
            {
                string path = (string)this.SelectedTab.Tag;
                DiffWantedEventArgs args = new DiffWantedEventArgs(path);

                // now see if anyone has a diff for us
                if ( this.DiffWanted != null )
                    this.DiffWanted( this, args );

                if ( args.Diff != null )
                    ((DiffTabPage)this.SelectedTab).Diff = args.Diff;

                if ( args.Source != null )
                    ((DiffTabPage)this.SelectedTab).Source = args.Source;
            }
        }

        /// <summary>
        /// One tab on the difftab - representing one file, with subtabs for the 
        /// code and the diff.
        /// </summary>
        private class DiffTabPage : TabPage
        {
            public DiffTabPage( string path ) : base( path )
            {
                // create subtabs
                this.diffPage = new TabPage( "Diff" );
                this.sourcePage = new TabPage( "Source" );  

                // create an inner tab control to hold the subtabs
                TabControl innerControl = new TabControl();
                innerControl.Controls.AddRange( 
                    new Control[]{ this.diffPage, this.sourcePage } );
                this.Controls.Add( innerControl );
                innerControl.Dock = DockStyle.Fill;
            }

            /// <summary>
            /// Has this page been initialized with a diff/source?
            /// </summary>
            public bool Initialized
            {
                get
                {
                    return this.diffPage.Controls.Count > 0 ||
                        this.sourcePage.Controls.Count > 0;
                }
            }

            /// <summary>
            /// Set the diff.
            /// </summary>
            public string Diff
            {
                set
                {
                    DiffView diff = new DiffView();
                    diff.Diff = value;
                    this.diffPage.Controls.Add( diff );
                    diff.Dock = DockStyle.Fill;
                }
            }

            /// <summary>
            /// The source.
            /// </summary>
            public string Source
            {
                set
                {
                    RichTextBox box = new RichTextBox();
                    box.Multiline = true;
                    box.ScrollBars = RichTextBoxScrollBars.Both;
                    box.Text = value;
                    this.sourcePage.Controls.Add( box );
                    box.Dock = DockStyle.Fill;
                }
            }  

            private TabPage diffPage;
            private TabPage sourcePage;
        }

        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

    }

    /// <summary>
    /// The arguments passed to the DiffWanted event.
    /// </summary>
    public class DiffWantedEventArgs : EventArgs
    {
        internal DiffWantedEventArgs( string path )
        {
            this.path = path;
        }

        public string Path
        {
            get{ return this.path; }
        }

        public string Diff
        {
            get{ return this.diff; }
            set{ this.diff = value; }
        }

        public string Source
        { 
            get{ return this.source; }
            set{ this.source = value; }
        }

        private string source;
        private string path; 
        private string diff;
    }

    public delegate void DiffWantedDelegate ( object sender, DiffWantedEventArgs args );
}
