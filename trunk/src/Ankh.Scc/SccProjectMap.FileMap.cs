using System;
using System.Collections.Generic;
using Ankh.Scc.ProjectMap;

namespace Ankh.Scc
{
    partial class SccProjectMap : AnkhService
    {
        readonly Dictionary<string, SccProjectFile> _fileMap = new Dictionary<string, SccProjectFile>(StringComparer.OrdinalIgnoreCase);

        public bool TryGetValue(string fileName, out SccProjectFile file)
        {
            return _fileMap.TryGetValue(fileName, out file);
        }

        public bool ContainsFile(string path)
        {
            return _fileMap.ContainsKey(path);
        }

        public void AddFile(string path, SccProjectFile sccProjectFile)
        {
            _fileMap.Add(path, sccProjectFile);
        }

        public int UniqueFileCount
        {
            get { return _fileMap.Count; }
        }

        public void RemoveFile(string fileName)
        {
            _fileMap.Remove(fileName);
        }

        public IEnumerable<string> AllFiles
        {
            get { return _fileMap.Keys; }
        }
    }
}
