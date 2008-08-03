using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    public interface IService
    {
        void SetProgressCallback( IProgressCallback callback );
    }
}
