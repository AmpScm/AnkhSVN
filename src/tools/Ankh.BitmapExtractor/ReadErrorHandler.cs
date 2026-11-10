using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.CommandTable;

namespace Ankh.BitmapExtractor
{
    class ReadErrorHandler : IMessageProcessor
    {
        public void Dependency(string file)
        {
            Console.WriteLine(" Dependency {0}", file);
        }

        public void Error(int error, string file, int line, int pos, string message)
        {
            Console.Error.WriteLine("{1}({2},{3}): Error {0}: {4}", error, file, line, pos, message);
        }

        public bool VerboseOutput()
        {
            return true;
        }

        public void Warning(int error, string file, int line, int pos, string message)
        {
            Console.Error.WriteLine("{1}({2},{3}): Warning {0}: {4}", error, file, line, pos, message);
        }

        public void WriteLine(string format, params object[] arg)
        {
            Console.WriteLine(" " + format, arg);
        }
    }
}
