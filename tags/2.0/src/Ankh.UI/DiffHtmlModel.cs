// $Id$
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Ankh.UI
{
    /// <summary>
    /// Represents a diff and its conversion to a colored HTML representation.
    /// </summary>
    public class DiffHtmlModel
    {
        public DiffHtmlModel(string diff)
        {
            this.diff = diff;
        }

        public DiffHtmlModel()
            : this("")
        {
        }

        public string Diff
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.diff; }

            [System.Diagnostics.DebuggerStepThrough]
            set { this.diff = value; }
        }

        /// <summary>
        /// The color to use for added lines.
        /// </summary>
        public Color AddedLine
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.addedLine; }

            [System.Diagnostics.DebuggerStepThrough]
            set { this.addedLine = value; }
        }

        /// <summary>
        /// The color to use for removed lines.
        /// </summary>
        public Color RemovedLine
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.removedLine; }

            [System.Diagnostics.DebuggerStepThrough]
            set { this.removedLine = value; }
        }

        /// <summary>
        /// The color to use for normal lines.
        /// </summary>
        public Color NormalLine
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.normalLine; }

            [System.Diagnostics.DebuggerStepThrough]
            set { this.normalLine = value; }
        }

        /// <summary>
        /// The color to use for @@ lines.
        /// </summary>
        public Color AtLine
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.atLine; }

            [System.Diagnostics.DebuggerStepThrough]
            set { this.AtLine = value; }
        }

        static string XHtmlEncode(string line)
        {
            StringBuilder sb = new StringBuilder(line.Length + line.Length / 4);

            for (int i = 0; i < line.Length; i++)
            {
                if (!char.IsControl(line, i) && (int)line[i] < 128)
                {
                    switch (line[i])
                    {
                        case '\"':
                            sb.Append("&quot;");
                            break;
                        case '<':
                            sb.Append("&lt;");
                            break;
                        case '>':
                            sb.Append("&gt;");
                            break;
                        case '&':
                            sb.Append("&amp;");
                            break;
                        default:
                            sb.Append(line[i]);
                            continue;
                    }
                }
                else
                {
                    sb.Append("&#");
                    sb.Append((int)line[i]);
                    sb.Append(";");
                }
            }

            return sb.ToString();
        }

        public string GetHtml()
        {
            using (StringWriter writer = new StringWriter())
            {
                this.WriteProlog(writer);

                using (StringReader reader = new StringReader(this.diff))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // get the css class for this line
                        string cssClass = this.GetClass(line);
                        writer.Write("<span class=\"{0}\">", cssClass);
                        writer.Write(XHtmlEncode(line));
                        writer.WriteLine("</span>");
                    }
                }
                this.WriteEpilog(writer);

                return writer.ToString();
            }
        }

        /// <summary>
        /// Retrieve the CSS class for a line, based on its first chars.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string GetClass(string line)
        {
            if (line.Length == 0)
                return "default";

            switch (line[0])
            {
                case '+':
                    return "plus";
                case '-':
                    return "minus";
                case '@':
                    return "at";
                default:
                    return "default";
            }
        }

        /// <summary>
        /// Write out the standard housekeeping stuff, and the inline stylesheet
        /// </summary>
        private void WriteProlog(TextWriter writer)
        {

            writer.WriteLine(
                @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
<html>
    <head> 
        <title>Diff</title>
        <style type='text/css'>
        <!--
            .plus {{  color: {0};  }}
            .minus {{ color: {1}; }}
            .at {{ color: {2}; }}
            .default {{ color: {3};}}
        -->
        </style>
    </head>
    <body>
<pre>", this.Convert(this.AddedLine),
                this.Convert(this.RemovedLine),
                this.Convert(this.AtLine),
                this.Convert(this.NormalLine));
        }

        /// <summary>
        /// Standard housekeeping stuff to round off the html doc.
        /// </summary>
        /// <param name="writer"></param>
        private void WriteEpilog(TextWriter writer)
        {
            writer.WriteLine("</pre>");
            writer.WriteLine("    </body>");
            writer.WriteLine("</html>");
        }

        private string Convert(Color color)
        {
            return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G,
                color.B);
        }

        private string diff;
        private Color removedLine = Color.Red;
        private Color addedLine = Color.Blue;
        private Color atLine = Color.Brown;
        private Color normalLine = Color.Green;
    }
}
