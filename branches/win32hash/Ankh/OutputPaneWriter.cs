using System;
using System.IO;
using System.Text;
using EnvDTE;

namespace Ankh
{
    /// <summary>
    /// A TextWriter backed by the VS.NET output window.
    /// </summary>
    public class OutputPaneWriter : TextWriter
    {
        public OutputPaneWriter( _DTE dte, string caption )
        {
            this.outputWindow = dte.Windows.Item( EnvDTE.Constants.vsWindowKindOutput );

            OutputWindow window = (OutputWindow)this.outputWindow.Object;
            this.outputPane = window.OutputWindowPanes.Add( caption );		
        }

        public override Encoding Encoding
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return Encoding.Default; }
        }

        /// <summary>
        /// Activate the pane.
        /// </summary>
        public void Activate()
        {
            if ( !this.outputWindow.AutoHides )
            {
                this.outputWindow.Activate();
                this.outputPane.Activate();
            }
        }

        /// <summary>
        /// Clear the pane.
        /// </summary>
        public void Clear()
        {
            this.outputPane.Clear();
        }

        public override void Write( char c )
        {
            this.outputPane.OutputString( c.ToString() );
        }

        public override void Write( string s )
        {
            this.outputPane.OutputString( s );
        }

        public override void WriteLine( string s )
        {
            this.outputPane.OutputString( s + Environment.NewLine );
        }


        /// <summary>
        /// Writes Start text to outputpane.
        /// </summary>
        /// <param name="action">Action.</param>
        public void StartActionText( string action )
        {
            
            this.Activate();
            this.outputPane.OutputString( this.FormatMessage( action ) + Environment.NewLine + 
                Environment.NewLine );
        }

        /// <summary>
        /// Writes end text to outputpane.
        /// </summary>
        public void EndActionText()
        {
            this.outputPane.OutputString( this.FormatMessage( "Done" ) + Environment.NewLine + 
                Environment.NewLine );
        }
        

        /// <summary>
        /// Formats the text for output.
        /// </summary>
        /// <param name="action">action string.</param>
        /// <returns>Formated text string</returns>
        private string FormatMessage( string action )
        {
            int left = (LINELENGTH / 2) - (action.Length / 2);
            int right = LINELENGTH - ( left + action.Length );
            
            // Avoid those values to be negative
            left = Math.Max( left, 3 );
            right = Math.Max( right, 3 );

            return new string( '-', left ) + action + new string( '-', right );
        }

        private const int LINELENGTH = 70;
        private const char LINECHAR = '-';
        private OutputWindowPane outputPane;
        private Window outputWindow;
    }
}
