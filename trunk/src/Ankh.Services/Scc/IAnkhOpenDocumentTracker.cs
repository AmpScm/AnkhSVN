using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    public interface IAnkhOpenDocumentTracker
    {
        bool IsDocumentOpen(string path);
        bool IsDocumentDirty(string path);

        bool SaveDocument(string path);
        bool SaveDocuments(IEnumerable<string> paths);
    }
}
