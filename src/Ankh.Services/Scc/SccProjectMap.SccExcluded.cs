using System;
using System.Collections.Generic;
using System.IO;
using SharpSvn;

namespace Ankh.Scc
{
    partial class SccProjectMap
    {
        readonly HybridCollection<string> _sccExcluded = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

        public void SerializeSccExcludeData(System.IO.Stream store, bool writeData)
        {
            ClearSolutionInfo(); // Force the right data
            string baseDir = SolutionDirectory;

            if (writeData)
            {
                if (_sccExcluded.Count == 0)
                    return;

                using (BinaryWriter bw = new BinaryWriter(store))
                {
                    bw.Write(_sccExcluded.Count);

                    foreach (string path in _sccExcluded)
                    {
                        if (baseDir != null)
                            bw.Write(SvnItem.MakeRelative(baseDir, path));
                        else
                            bw.Write(path);
                    }
                }
            }
            else
            {
                if (store.Length == 0)
                    return;

                using (BinaryReader br = new BinaryReader(store))
                {
                    int count = br.ReadInt32();

                    while (count-- > 0)
                    {
                        string path = br.ReadString();

                        try
                        {
                            if (baseDir != null)
                                path = SvnTools.GetNormalizedFullPath(Path.Combine(baseDir, path));
                            else
                                path = SvnTools.GetNormalizedFullPath(path);

                            if (!_sccExcluded.Contains(path))
                                _sccExcluded.Add(path);
                        }
                        catch { }
                    }
                }
            }
        }

        public bool IsSccExcluded(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            return _sccExcluded.Contains(path);
        }

        public void SccExclude(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            _sccExcluded.Add(path);
        }

        public bool SccRemoveExcluded(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            return _sccExcluded.Remove(path);
        }
    }
}
