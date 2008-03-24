using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.ContextServices
{
    public interface IAnkhOperationLogger
    {
        IDisposable BeginOperation(string message);

        void Write(string message);
        void WriteLine(string message);
    }
}
