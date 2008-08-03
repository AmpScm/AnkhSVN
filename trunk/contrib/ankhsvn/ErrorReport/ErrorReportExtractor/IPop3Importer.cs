using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    public interface IPop3Importer : IReportContainer
    {
        void DeleteFromServer( int startIndex, int length );
    }
}
