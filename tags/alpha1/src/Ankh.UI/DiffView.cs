// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using SHDocVw;
using System.Diagnostics;

namespace Ankh.UI
{
	/// <summary>
	/// This is a control that displays unified diffs.
	/// </summary>
	public class DiffView : System.Windows.Forms.UserControl
	{
		public DiffView()
		{
            this.diffHtmlModel = new DiffHtmlModel();

            this.components = new System.ComponentModel.Container();
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
                using ( StreamWriter w = new StreamWriter( path ) )
                    w.Write( this.diffHtmlModel.GetHtml() );
                
                object url = "file://" + path;
                object nullObj = null;
                this.webBrowser.Navigate2( ref url, ref nullObj, ref nullObj,
                    ref nullObj, ref nullObj );
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

        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DiffView));
            this.webBrowser = new AxSHDocVw.AxWebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.webBrowser)).BeginInit();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Enabled = true;
            this.webBrowser.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("webBrowser.OcxState")));
            this.webBrowser.Size = new System.Drawing.Size(328, 184);
            this.webBrowser.TabIndex = 0;
            // 
            // DiffView
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.webBrowser});
            this.Name = "DiffView";
            this.Size = new System.Drawing.Size(328, 184);
            ((System.ComponentModel.ISupportInitialize)(this.webBrowser)).EndInit();
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private AxSHDocVw.AxWebBrowser webBrowser;

        private DiffHtmlModel diffHtmlModel;

		
    }
}
