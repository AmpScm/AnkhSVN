using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ErrorReportExtractor;

namespace ErrorReport.GUI
{
    public partial class ProgressCallbackControl : UserControl, IProgressCallback
    {
        public ProgressCallbackControl()
        {
            InitializeComponent();
        }

        #region IProgressCallback Members

        public void Verbose( string message, params object[] args )
        {
            WriteMessage( message, args );
        }

        public void Info( string message, params object[] args )
        {
            WriteMessage( message, args );
        }

        public void Warning( string message, params object[] args )
        {
            WriteMessage( message, args );
        }

        public void Error( string message, params object[] args )
        {
            WriteMessage( message, args );
        }

        public void Progress()
        {
            this.progressBar.PerformStep();
        }

        public bool VerboseMode
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public void Exception( Exception ex )
        {
            WriteMessage( ex.ToString() );
        }

        #endregion

        private void WriteMessage( string message, params object[] args )
        {
            this.richTextBox.AppendText( String.Format( message, args ) + Environment.NewLine );
        }
    }
}
