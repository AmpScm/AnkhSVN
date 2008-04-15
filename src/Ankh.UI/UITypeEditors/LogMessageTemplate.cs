// $Id$
using System;

using Ankh.UI;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Collections;
using Ankh.Scc;

namespace Ankh.UI
{
    /// <summary>
    /// Represents a log message template.
    /// </summary>
    public class LogMessageTemplate
    {
        IAnkhServiceProvider _context;

        public LogMessageTemplate(IAnkhServiceProvider context, string template)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            this.template = template;
        }

        public LogMessageTemplate(IAnkhServiceProvider context)
            : this(context, "")
        { }

        /// <summary>
        /// The template to be used for log messages.
        /// </summary>
        public string Template
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.template; }

            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                this.template = value;
            }

        }

        public bool UrlPaths
        {
            get { return this.urlPaths; }
            set { this.urlPaths = value; }
        }

        public virtual string PreProcess(IList paths)
        {
            if (this.Template.Trim() == String.Empty)
            {
                return this.Template;
            }

            string text = this.Template;
            // substitute all the patterns.
            if (LINETEMPLATE.IsMatch(this.template))
            {
                string linePattern = LINETEMPLATE.Match(this.template).Groups["linepattern"].Value.Trim();
                text = LINETEMPLATE.Replace(this.template, this.SubstituteLinePattern(linePattern, paths));

            }
            return text.Trim() + Environment.NewLine;
        }

        public virtual string PostProcess(string message)
        {
            // strip out comments.
            using (StringWriter writer = new StringWriter())
            {
                using (StringReader reader = new StringReader(message))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                        if (!COMMENTPATTERN.IsMatch(line))
                            writer.WriteLine(line);
                    return writer.ToString().Trim() + Environment.NewLine;
                }
            }
        }

        /// <summary>
        /// Removes an item from the log message, if unchanged. 
        /// </summary>
        /// <param name="currentLog">The current log message.</param>
        /// <param name="path">The path to be removed.</param>
        /// <returns>The new log message</returns>
        public virtual string RemoveItem(string currentLog, string path)
        {
            string line = Regex.Escape(this.LineForPath(path));
            string regex = "^" + line + @"\s*";

            return Regex.Replace(currentLog, regex, "", RegexOptions.Multiline);
        }

        /// <summary>
        /// Adds an item to the log message, if its not already there.
        /// </summary>
        /// <param name="currentLog">The current log message.</param>
        /// <param name="path">The new path.</param>
        /// <returns>The altered log message</returns>
        public virtual string AddItem(string currentLog, string path)
        {
            string line = this.LineForPath(path);

            // dont replace if the line is already there
            if (currentLog.IndexOf(line) >= 0)
                return currentLog;
            else
                return currentLog + line + Environment.NewLine;
        }

        private string LineForPath(string path)
        {
            IWorkingCopyOperations wcOp = _context.GetService<IWorkingCopyOperations>();

            string rootedPath = (this.urlPaths || (wcOp == null))
                ? path : wcOp.GetWorkingCopyRootedPath(path);
            string linePattern = LINETEMPLATE.Match(this.template).Groups[
                "linepattern"].Value.Trim();
            return linePattern.Replace("%path%", rootedPath);
        }


        private string SubstituteLinePattern(string linePattern, IList paths)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string path in paths)
            {
                string line = LineForPath(path);
                builder.Append(line + Environment.NewLine);
            }

            return builder.ToString();
        }

        private string template;
        private bool urlPaths;
        private static readonly Regex LINETEMPLATE = new Regex(@"^\*\*\*(?'linepattern'.+)$",
            RegexOptions.Multiline | RegexOptions.ExplicitCapture);
        private static readonly Regex COMMENTPATTERN = new Regex(@"^#.*?$",
            RegexOptions.Multiline | RegexOptions.Singleline);
    }

}
