// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Ankh
{
    public static class VSVersion
    {
        static readonly object _lock = new object();
        static Version _vsVersion;

        public static Version FullVersion
        {
            get
            {
                lock (_lock)
                {
                    if (_vsVersion == null)
                    {
                        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "msenv.dll");

                        if (File.Exists(path))
                        {
                            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(path);

                            string verName = fvi.ProductVersion;

                            for (int i = 0; i < verName.Length; i++)
                            {
                                if (!char.IsDigit(verName, i) && verName[i] != '.')
                                {
                                    verName = verName.Substring(0, i);
                                    break;
                                }
                            }
                            _vsVersion = new Version(verName);
                        }
                        else
                            _vsVersion = new Version(0, 0); // Not running inside Visual Studio!
                    }
                }

                return _vsVersion;
            }
        }

        public static bool VS2010OrLater
        {
            get { return FullVersion >= new Version(10, 0); }
        }

        public static bool VS2008OrOlder
        {
            get { return FullVersion < new Version(10, 0); }
        }

        public static bool VS2005
        {
            get { return FullVersion.Minor == 8; }
        }

        public static bool VS2008
        {
            get { return FullVersion.Minor == 9; }
        }

        public static bool VS2010
        {
            get { return FullVersion.Minor == 10; }
        }
    }
}
