using System;
using System.Collections.Generic;
using System.Text;

namespace AnkhBot
{
    class ArgSplitter : IEnumerable<string>
    {
        public ArgSplitter( string line )
        {
            this.line = line;
        }

        public IEnumerator<string> GetEnumerator()
        {
            bool inQuotes = false;
            bool prevWasSpace = false;
            StringBuilder builder = new StringBuilder();

            foreach (char c in this.line)
            {
                switch (c)
                {
                    case ' ':
                        if (inQuotes)
                            goto default;
                        else
                        {
                            prevWasSpace = true;
                        }
                        break;
                    case '"':
                        if (inQuotes)
                        {
                            yield return builder.ToString();
                            builder.Length = 0;
                        }
                        inQuotes = !inQuotes;
                        break;
                    default:
                        if (prevWasSpace)
                        {
                            if (builder.Length > 0)
                                yield return builder.ToString().Trim();
                            builder.Length = 0;
                            prevWasSpace = false;
                        }
                        builder.Append( c );

                        break;
                }

            }
            if (builder.Length > 0)
                yield return builder.ToString();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private string line;

    }
}
