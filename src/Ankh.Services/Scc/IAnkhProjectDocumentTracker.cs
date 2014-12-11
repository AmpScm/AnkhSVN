using System;
using System.Collections.Generic;

namespace Ankh.Scc
{
    public interface IAnkhProjectDocumentTracker
    {
        IEnumerable<string> GetAllDocumentFiles(string documentName);
    }
}
