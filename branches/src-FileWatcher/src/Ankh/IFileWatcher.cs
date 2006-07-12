using System;

namespace Ankh
{
    public interface IFileWatcher
    {
        void AddFile( string path );
        void Clear();
        event FileModifiedDelegate FileModified;
    }
}
