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
