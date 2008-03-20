using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc.ProjectMap;

namespace Ankh.Scc
{
    partial class OpenDocumentTracker 
    {
        public bool IsDocumentOpen(string path)
        {
            if(path == null)
                throw new ArgumentNullException("path");

            SccDocumentData data;
            
            if(!_docMap.TryGetValue(path, out data))
                return false;
            else
                return true;
        }

        public bool IsDocumentDirty(string path)
        {
            if(path == null)
                throw new ArgumentNullException("path");

            SccDocumentData data;
            
            if(!_docMap.TryGetValue(path, out data))
                return false;
            
            return data.IsDirty;
        }

        public bool SaveDocument(string path)
        {
            return false;
        }

        public bool SaveDocuments(IEnumerable<string> paths)
        {
            return false;
        }
    }
}
