using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ErrorReportExtractor;
using System.Runtime.InteropServices;

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
            //this.progressBar.PerformStep();
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
            MethodInvoker invoker = delegate
            {
                MessageBox.Show(ex.ToString());
            };
            this.PerformInvoke( invoker );
        }

        private void PerformInvoke( MethodInvoker invoker )
        {
            if ( this.InvokeRequired )
            {
                this.Invoke( invoker );
            }
            else
            {
                invoker();
            }
        }

        #endregion

        private void WriteMessage( string message, params object[] args )
        {
            MethodInvoker invoker = delegate
            {
                this.richTextBox.AppendText( String.Format( message, args ) + Environment.NewLine );
                this.ScrollToBottom();
            };
            this.PerformInvoke( invoker );
            
        }

        private void ScrollToBottom()
        {
            SendMessage( this.richTextBox.Handle, WM_VSCROLL, SB_BOTTOM, 0 );
        }

        [DllImport( "User32.dll" )]
        private static extern int SendMessage( IntPtr hWnd, uint msg, int wparam, int lparam );

        private const int WM_VSCROLL = 0x0115;
        private const int SB_BOTTOM = 7;
    }
}
