using System;
using EnvDTE;
using System.Windows.Forms;


namespace Ankh
{
	[Service(typeof(IHostWindowService))]
	class HostWindowService : IHostWindowService
	{
		public HostWindowService(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}


		public IWin32Window HostWindow
		{
			get 
			{
				if (this.hostWindow == null)
				{
					_DTE dte = (_DTE)serviceProvider.GetService(typeof(_DTE));
					this.hostWindow = new Win32Window(new IntPtr(dte.MainWindow.HWnd));
				}
				return this.hostWindow; 
			}
		}

		

		private class Win32Window : IWin32Window
		{
			public Win32Window(IntPtr handle)
			{
				this.handle = handle;
			}

			public System.IntPtr Handle
			{
				get { return this.handle; }
			}

			private IntPtr handle;
		}

		private IWin32Window hostWindow;
		private IServiceProvider serviceProvider;
	}
}