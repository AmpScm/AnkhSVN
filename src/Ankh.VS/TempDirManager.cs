// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using SharpSvn;

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
            string name = "";
            for (int i = 4; i < 32; i += 2)
            {
                name = Path.Combine(Path.GetTempPath(), "AnkhSVN\\" + Guid.NewGuid().ToString("N").Substring(0, i));

                if (!SvnItem.PathExists(name))
                    break;
            }
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
                try
                {
                    Dispose(true);
                }
                finally
                {
                    GC.SuppressFinalize(this);
                }
            }

            void Dispose(bool disposing)
            {
                Delete();
            }

            void Delete()
            {
                foreach (KeyValuePair<string, bool> kv in _directories)
                {
                    if (!kv.Value)
                        SvnItem.DeleteNode(kv.Key);
                }
            }

            public void AddDirectory(string name, bool keepDir)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name");
                else if (!SvnTools.IsNormalizedFullPath(name))
                    throw new ArgumentException("Not a normalized full path", "name");

                _directories.Add(name, keepDir);
            }
        }
    }
}
