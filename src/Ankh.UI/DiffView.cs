using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace Ankh.UI
{
	/// <summary>
	/// This is a rich text box that displays unified diffs.
	/// </summary>
	public class DiffTextBox : System.Windows.Forms.RichTextBox
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DiffTextBox()
		{
            this.ReadOnly = true;
            this.Font = new Font( "Courier New", 10 );
		}

        public string Diff
        {
            get{ return this.Text; }
            set{ this.Colorize( value ); }
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

        /// <summary>
        /// Show the diff in the box.
        /// </summary>
        /// <param name="code"></param>
        private void Colorize( string code )
        {
            this.Text = "";
            using( StringReader reader = new StringReader( code ) )
            {
                string line;
                while( (line = reader.ReadLine()) != null )
                {
                    this.SetLineColor( line[0] );
                    this.AppendText( line + Environment.NewLine );
                }
            }
        }

        /// <summary>
        /// Choose the color of the line depending on the first char.
        /// </summary>
        /// <param name="firstChar"></param>
        private void SetLineColor( char firstChar )
        {
            switch( firstChar )
            {
                case '+':
                    this.SelectionColor = Color.Blue;
                    break;
                case '-':
                    this.SelectionColor = Color.Red;
                    break;
                default:
                    this.SelectionColor = Color.Green;
                    break;
            }
        }

		
    }
}
