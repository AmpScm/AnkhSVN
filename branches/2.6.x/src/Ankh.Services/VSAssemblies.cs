using System;
using System.Reflection;

namespace Ankh
{
    public static class VSAssemblies
    {

        static Assembly _vsShellInterop11;
        public static Assembly VSShellInterop11
        {
            get
            {
                if (_vsShellInterop11 == null && VSVersion.VS2012OrLater)
                    _vsShellInterop11 = Assembly.Load("Microsoft.VisualStudio.Shell.Interop.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                return _vsShellInterop11;
            }

        }

        static Assembly _vsShellInterop12;
        public static Assembly VSShellInterop12
        {
            get
            {
                if (_vsShellInterop12 == null && VSVersion.VS2013OrLater)
                    _vsShellInterop12 = Assembly.Load("Microsoft.VisualStudio.Shell.Interop.12.0, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                return _vsShellInterop12;
            }

        }
    }
}
