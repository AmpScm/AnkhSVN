using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    public class NullProgressCallback : IProgressCallback
    {
        #region IProgressCallback Members

        public void Verbose( string message, params object[] args )
        {
        }

        public void Info( string message, params object[] args )
        {
        }

        public void Warning( string message, params object[] args )
        {
        }

        public void Error( string message, params object[] args )
        {
        }

        public void Progress()
        {
        }

        public bool VerboseMode
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public void Exception( Exception ex )
        {
            
        }

        #endregion
    }
}
