// $Id$
using System;
using NSvn.Core;
using Ankh.UI;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace Ankh.UI
{
    /// <summary>
    /// Represents a log message template.
    /// </summary>
    public class LogMessageTemplate
    {
        public LogMessageTemplate( string template )
        {
            this.template = template;
        }

        public LogMessageTemplate() : this( "" )
        {}

        /// <summary>
        /// The template to be used for log messages.
        /// </summary>
        public string Template
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.template; }

            [System.Diagnostics.DebuggerStepThrough]
            set
            { 
                this.template = value; 
            }
            
        }

        public virtual string PreProcess( CommitItem[] items )        
        {
            string text = this.Template;
            // substitute all the patterns.
            if ( LINETEMPLATE.IsMatch( this.template ) )
            {
                string linePattern = LINETEMPLATE.Match( this.template ).Groups[ "linepattern" ].Value.Trim();
                text = LINETEMPLATE.Replace( this.template, this.SubstituteLinePattern( linePattern, items ) );
                
            }
            return text;
        }

        public virtual string PostProcess( string message )
        {
            // strip out comments.
            using ( StringWriter writer = new StringWriter() )
            {
                using( StringReader reader = new StringReader( message ) )
                {
                    string line;
                    while( (line=reader.ReadLine()) != null )
                        if ( !COMMENTPATTERN.IsMatch( line ) )
                            writer.WriteLine( line );
                    return writer.ToString();
                }
            }
        }
 

        private string SubstituteLinePattern( string linePattern, CommitItem[] items )
        {
            StringBuilder builder = new StringBuilder();
            foreach( CommitItem item in items )
            { 
                string line = linePattern.Replace( "%path%", NSvn.SvnUtils.GetWorkingCopyRootedPath(
                    item.Path ) );
                line = line.Replace( "%basepath%", Path.GetFileName( item.Path ) );

                builder.Append( line + Environment.NewLine );
            }

            return builder.ToString();
        }

        private string template;
        private static readonly Regex LINETEMPLATE = new Regex(@"^\*\*\*(?'linepattern'.+)$", 
            RegexOptions.Multiline | RegexOptions.ExplicitCapture);
        private static readonly Regex COMMENTPATTERN = new Regex( @"^#.*?$", 
            RegexOptions.Multiline | RegexOptions.Singleline );
    }

}
