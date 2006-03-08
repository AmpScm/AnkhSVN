using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ErrorReportExtractor
{
    public class StackTrace : List<IStackTraceItem>, IStackTrace 
    {
        public StackTrace(string body)
        {
            this.ParseBody(body);
        }

       
        public string Text
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        private void ParseBody(string body)
        {
            MatchCollection matches = StackTraceItemRegex.Matches(body);
            int sequenceNumber = 0;
            foreach( Match match in matches)
            {
                string methodName = null;
                string parameters = null;
                string filename = null;
                int? linenumber = null;
 
                Group group= match.Groups["MethodName"];
                if (group.Success)
                {
                    methodName = group.Value;
                }
                group = match.Groups["Parameters"];
                if (group.Success)
                {
                    parameters = group.Value;
                }
                group = match.Groups["Filename"];
                if (group.Success)
                {
                    filename = group.Value;
                }
                group = match.Groups["LineNumber"];
                if (group.Success)
                {
                    linenumber = int.Parse(group.Value);
                }

                this.Add(new StackTraceItem(methodName, parameters, filename, linenumber, sequenceNumber));

                sequenceNumber++;
            }
        }

        private static readonly Regex StackTraceItemRegex = new Regex(
            @"\s*at (?<MethodName>(\w+\.)*\w+)?\((?<Parameters>.*?)?\)(\s+in\s+(?<Filename>.+?):line)?(\s+(?<LineNumber>\d+))?",
                RegexOptions.IgnoreCase);
    }
}
