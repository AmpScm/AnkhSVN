using System.Text;

using EnvDTE;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using NSvn.Core;
using System.Windows.Forms;

//using Microsoft.VisualStudio.Shell.Interop;
//using Microsoft.VisualStudio;
//using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;


namespace Ankh
{
    public class VSConstants
    {
        public const int S_OK = 0;
        public const int E_FAIL = -2147467259;
    }

    enum VSITEMID
    {
        VSITEMID_NIL = -1,
        VSITEMID_ROOT = -2,
        VSITEMID_SELECTION = -3
    }
}