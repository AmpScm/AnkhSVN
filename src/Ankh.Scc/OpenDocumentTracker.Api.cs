using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc.ProjectMap;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

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

        public bool PromptSaveDocument(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (!_docMap.TryGetValue(path, out data))
                return false;

            if (!data.IsDirty || (data.Cookie == 0))
                return true; // Not/never modified, no need to save

            // Save the document if it is dirty
            return ErrorHandler.Succeeded(RunningDocumentTable.SaveDocuments((uint)__VSRDTSAVEOPTIONS.RDTSAVEOPT_PromptSave, 
                data.Hierarchy, data.ItemId, data.Cookie));
        }

        public bool SaveDocument(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (!_docMap.TryGetValue(path, out data))
                return false;

            if (!data.IsDirty || (data.Cookie == 0))
                return true; // Not/never modified, no need to save

            // Save the document if it is dirty
            return ErrorHandler.Succeeded(RunningDocumentTable.SaveDocuments(0, data.Hierarchy, data.ItemId, data.Cookie));
        }

        public bool SaveDocuments(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            foreach (string path in paths)
            {
                if (!SaveDocument(path))
                    return false;
            }

            return true;
        }

        public DocumentLock LockDocuments(IEnumerable<string> paths)
        {
            return new SccDocumentLock(Context);
        }

        class SccDocumentLock : DocumentLock
        {
            readonly IAnkhServiceProvider _context;

            public SccDocumentLock(IAnkhServiceProvider context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                _context = context;
            }

            public override void Reload(IEnumerable<string> paths)
            {
                if (paths == null)
                    throw new ArgumentNullException("paths");

                OpenDocumentTracker dt = (OpenDocumentTracker)_context.GetService<IAnkhOpenDocumentTracker>();

                if (dt == null)
                    throw new InvalidOperationException();

                foreach (string path in paths)
                {
                    SccDocumentData dd;
                    if (dt._docMap.TryGetValue(path, out dd))
                    {
                        if (!dd.GetIsDirty())
                            dd.Reload(true);
                    }
                }
            }

            public override void Dispose()
            {
                //
            }
        }
    }
}
