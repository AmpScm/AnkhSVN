using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    class ProgressCallback : IProgressCallback
    {
        private bool verboseMode;

        public bool VerboseMode
        {
            get { return verboseMode; }
            set { verboseMode = value; }
        }
	

        #region IProgressCallback Members



        public void Info(string message, params object[] args)
        {
            this.Output(ConsoleColor.Green, message, args);
        }

        public void Warning(string message, params object[] args)
        {
            this.Output(ConsoleColor.Yellow, message, args);
        }

        public void Error(string message, params object[] args)
        {
            this.Output(ConsoleColor.Red, message, args);
        }

         public void Verbose(string message, params object[] args)
        {
            if (this.VerboseMode)
            {
                this.Output(ConsoleColor.Cyan, message, args);
            }
        }



        #endregion

        private void Output(ConsoleColor color, string message, params object[] args)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message, args);
            Console.ForegroundColor = originalColor;
        }


        #region IProgressCallback Members


        public void Progress()
        {
            Console.Write(".");
        }

        #endregion

        #region IProgressCallback Members


        public void Exception(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
        }

        #endregion
    }
}
