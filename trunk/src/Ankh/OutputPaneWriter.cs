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
            OutputWindow window = (OutputWindow)dte.Windows.Item( 
                EnvDTE.Constants.vsWindowKindOutput).Object;
            this.outputPane = window.OutputWindowPanes.Add( caption );			
		}

        public override Encoding Encoding
        {
            get{ return Encoding.Default; }
        }

        /// <summary>
        /// Activate the pane.
        /// </summary>
        public void Activate()
        {
            this.outputPane.Activate();
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

        /// <summary>
        /// Writes Start text to outputpane.
        /// </summary>
        /// <param name="action">Action.</param>
        public void StartActionText( string action )
        {
            
            this.Activate();
            this.outputPane.OutputString( FormatMessage( action ) );
        }

        /// <summary>
        /// Writes end text to outputpane.
        /// </summary>
        public void EndActionText()
        {
            this.outputPane.OutputString( FormatMessage( "Done" ));
        }
        

        /// <summary>
        /// Formats the text for output.
        /// </summary>
        /// <param name="action">action string.</param>
        /// <returns>Formatet text string</returns>
        private string FormatMessage( string action )
        {
            string message = line + action + " ";
            return Environment.NewLine 
                + message.PadRight(50, '-') + Environment.NewLine + Environment.NewLine;
        }

        private const string line = "---------------------- ";
        private OutputWindowPane outputPane;
	}
}
