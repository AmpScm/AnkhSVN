﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Ankh.VS
{
    [GlobalService(typeof(IAnkhTempDirManager))]
    class TempDirManager : AnkhService, IAnkhTempDirManager
    {
        readonly TempDirCollection _tempDirs = new TempDirCollection();

        public TempDirManager(IAnkhServiceProvider context)
            : base(context)
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

                DirectoryInfo[] directories = null;
                try
                {
                    directories = dir.GetDirectories();
                }
                catch
                { }

                if (directories != null)
                {
                    foreach (DirectoryInfo sd in directories)
                    {
                        RecursiveDelete(sd);
                    }
                }

                FileInfo[] files = null;
                try
                {
                    files = dir.GetFiles();
                }
                catch { }

                if (files != null)
                {
                    foreach (FileInfo file in files)
                    {
                        try
                        {
                            file.Attributes = FileAttributes.Normal;
                            file.Delete();
                        }
                        catch { }
                    }
                }

                try
                {
                    dir.Attributes = FileAttributes.Normal; // .Net fixes up FileAttributes.Directory
                    dir.Delete();
                }
                catch { }
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
