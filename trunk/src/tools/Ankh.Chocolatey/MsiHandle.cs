using Microsoft.Win32.SafeHandles;

namespace Ankh.Chocolatey
{
    class MsiHandle : SafeHandleMinusOneIsInvalid
    {
        public MsiHandle()
            : base(true)
        { }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.MsiCloseHandle(handle) == 0;
        }
    }
}
