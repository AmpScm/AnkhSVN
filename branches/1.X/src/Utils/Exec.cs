using System;
using System.Runtime.InteropServices;

namespace Utils
{
    public class Exec: IDisposable
    {
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			Win32.Win32.CloseHandle(this.processHandle);
			this.processHandle = IntPtr.Zero;
		}
		~Exec()
		{
			Dispose(false);
		}
		
		public void ExecPath ( string path )
        {
			Win32.PROCESS_INFORMATION pi;
			Win32.STARTUP_INFO si = new Win32.STARTUP_INFO();
			Win32.Win32.CreateProcess(null, path, IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref si, out pi);

			this.processHandle = pi.hProcess;
			Win32.Win32.CloseHandle( pi.hThread );
        }

		public void WaitForExit()
		{
			Win32.Win32.WaitForSingleObject( this.processHandle, UInt32.MaxValue );
		}

        private IntPtr processHandle;
    }
}
