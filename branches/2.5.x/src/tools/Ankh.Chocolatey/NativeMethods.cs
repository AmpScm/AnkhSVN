using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Ankh.Chocolatey
{
    static class NativeMethods
    {
        const string MsiDll = "Msi.dll";

        [DllImport(MsiDll, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public extern static uint MsiOpenPackageW(string szPackagePath, out MsiHandle product);

        [DllImport(MsiDll, ExactSpelling=true)]
        public extern static uint MsiCloseHandle(IntPtr hAny);

        [DllImport(MsiDll, CharSet = CharSet.Unicode, ExactSpelling = true)]
        static extern uint MsiGetProductPropertyW(MsiHandle hProduct, string szProperty, StringBuilder value, ref int length);


        [DllImport(MsiDll, ExactSpelling = true)]
        public static extern int MsiSetInternalUI(int value, IntPtr hwnd);

        public static uint MsiGetProductProperty(MsiHandle hProduct, string szProperty, out string value)
        {
            StringBuilder sb = new StringBuilder(1024);
            int length = sb.Capacity;
            uint err;
            value = null;
            if(0 == (err = MsiGetProductPropertyW(hProduct, szProperty, sb, ref length)))
            {
                sb.Length = length;
                value = sb.ToString();
                return 0;
            }

            return err;
        }

    }
}
