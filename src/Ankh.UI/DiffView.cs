// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Ankh.UI
{
    /// <summary>
    /// This is a control that displays unified diffs.
    /// </summary>
    public partial class DiffView : System.Windows.Forms.UserControl
    {
        public DiffView()
        {
            this.diffHtmlModel = new DiffHtmlModel();
            
            this.InitializeComponent();
        }

        /// <summary>
        /// The diff itself.
        /// </summary>
        public string Diff
        {
            get{ return this.diffHtmlModel.Diff; }
            set
            { 
                this.diffHtmlModel.Diff = value;
                
                string path = Path.GetTempFileName();
                Trace.WriteLine( path );
                using ( StreamWriter w = new StreamWriter( path, true, System.Text.Encoding.Default ) )
                    w.Write( this.diffHtmlModel.GetHtml() );
                
                string url = "file://" + path;
                this.webBrowser.Navigate(url);
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }            


        private DiffHtmlModel diffHtmlModel;


    }
}
