using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.ContextServices
{
    interface IAnkhOperationLogger
    {
        IDisposable BeginOperation(string message);

        void Write(string message);
        void WriteLine(string message);
    }
}
