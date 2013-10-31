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
using Microsoft.VisualStudio.Shell.Interop;
using System.Text.RegularExpressions;

namespace Ankh
{
    public static class VSVersion
    {
        static Version _vsVersion;
        static Version _osVersion;

        public static Version FullVersion
        {
            get
            {
                if (_vsVersion == null)
                {
                    _vsVersion = new Version(0, 0); // Assume not running inside Visual Studio!
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "msenv.dll");

                    if (File.Exists(path))
                    {
                        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(path);
                        string verName = null;

                        if (fvi != null)
                            verName = fvi.ProductVersion ?? fvi.FileVersion;

                        if (!string.IsNullOrEmpty(verName))
                        {
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
                    }
                }

                return _vsVersion;
            }
        }

        const int VSSPROPID_ReleaseVersion = -9068; // VS 12+

        internal static void Ensure(IAnkhServiceProvider context)
        {
            if (FullVersion.Major == 0)
            {
                // Use the old DTE api in an attempt to get the version
                Version ver;
                if (TryGetDteVersion(context, out ver))
                    _vsVersion = ver;
            }

            if (FullVersion.Major == 0)
            {
                string versionStr = null;

                // VS 2012 has a shell property
                IVsShell shell = context.GetService<IVsShell>(typeof(SVsShell));
                if (shell != null)
                {
                    object v;
                    if (VSErr.Succeeded(shell.GetProperty(VSSPROPID_ReleaseVersion, out v)))
                        versionStr = v as string;
                }

                if (!string.IsNullOrEmpty(versionStr))
                    ParseVersion(versionStr);
            }
        }

        private static bool TryGetDteVersion(IAnkhServiceProvider context, out Version version)
        {
            version = null;
            Type dteType = Type.GetType("EnvDTE._DTE, EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false);

            if (dteType == null)
                return false;

            Object dte = context.GetService(typeof(SDTE));

            if (dte == null)
                return false;

            System.Reflection.MethodInfo method = dteType.GetMethod("get_Version");

            if (method == null)
                return false;

            string verStr = method.Invoke(dte, null) as string;

            if (verStr != null)
            {
                version = new Version(verStr);
                return true;
            }

            return false;
        }

        static void ParseVersion(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            value = value.Trim();

            Regex re = new Regex("^[0-9]+(\\.[0-9]+){0,3}"); // Avoid suffix. No compilation necessary
            Match m = re.Match(value);
            if (m.Success)
                _vsVersion = new Version(m.Value);
        }

        public static Version OSVersion
        {
            get { return _osVersion ?? (_osVersion = Environment.OSVersion.Version); }
        }

        public static bool VS2013OrLater
        {
            get { return FullVersion.Major >= 12; }
        }

        public static bool VS2012OrLater
        {
            get { return FullVersion.Major >= 11; }
        }

        public static bool VS2010OrLater
        {
            get { return FullVersion.Major >= 10; }
        }

        public static bool VS2008OrOlder
        {
            get { return FullVersion.Major <= 9; }
        }

        public static bool VS2005
        {
            get { return FullVersion.Major == 8; }
        }

        public static bool VS2008
        {
            get { return FullVersion.Major == 9; }
        }

        public static bool VS2010
        {
            get { return FullVersion.Major == 10; }
        }

        public static bool VistaOrLater
        {
            get { return OSVersion.Major >= 6; }
        }

        public static bool VS2010OrVistaOrLater
        {
            get { return VS2010OrLater || VistaOrLater; }
        }

        static readonly Version _vS11RCVersion = new Version(11, 0, 50522);

        public static bool SupportsTheming
        {
            get { return FullVersion >= _vS11RCVersion; }
        }
    }
}
