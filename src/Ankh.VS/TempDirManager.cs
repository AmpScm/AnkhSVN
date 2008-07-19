using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Ankh.VS
{
    class TempDirManager : IAnkhTempDirManager
    {
        readonly TempDirCollection _tempDirs = new TempDirCollection();

        public TempDirManager(IServiceProvider context)
        {
        }

        public string GetTempDir()
        {
            string name = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(name);
            _tempDirs.AddDirectory(name, false);
            return name;
        }

        class TempDirCollection : IDisposable
        {
            readonly Dictionary<string, bool> _directories = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            public TempDirCollection()
            {
            }

            ~TempDirCollection()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                Dispose(true);
            }

            void Dispose(bool disposing)
            {
                Delete();
            }

            void Delete()
            {
                foreach (string dir in _directories.Keys)
                {
                    if (!_directories[dir])
                    {
                        try
                        {
                            RecursiveDelete(new DirectoryInfo(dir));
                        }
                        catch 
                        {
                            // This code potentially runs in the finalizer thread, never throw from here
                        }
                    }
                }
            }

            void RecursiveDelete(DirectoryInfo dir)
            {
                if (dir == null)
                    throw new ArgumentNullException("dir");

                if (!dir.Exists)
                    return;

                foreach (DirectoryInfo sd in dir.GetDirectories())
                {
                    RecursiveDelete(sd);
                }

                foreach (FileInfo file in dir.GetFiles())
                {
                    file.Attributes = FileAttributes.Normal;
                    file.Delete();
                }

                dir.Attributes = FileAttributes.Normal; // .Net fixes up FileAttributes.Directory
                dir.Delete();
            }

            public void AddDirectory(string name, bool keepDir)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name");

                _directories.Add(name, keepDir);
            }
        }
    }
}
