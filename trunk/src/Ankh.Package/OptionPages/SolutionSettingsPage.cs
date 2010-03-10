using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using System.Windows.Forms;

namespace Ankh.VSPackage.OptionPages
{
	class SolutionSettingsPage : IPropertyPage, IPropertyPage2
	{
		IPropertyPageSite _site;
		IAnkhServiceProvider _context;

		public IPropertyPageSite Site
		{
			get { return _site; }
		}

		public IAnkhServiceProvider Context
		{
			get
			{
				return _context ?? (_context = Package.GetGlobalService(typeof(IAnkhServiceProvider)) as IAnkhServiceProvider);
			}
		}
		#region IPropertyPage Members

		public void Activate(IntPtr hWndParent, RECT[] pRect, int bModal)
		{
			//throw new NotImplementedException();
		}

		public int Apply()
		{
			throw new NotImplementedException();
		}

		public void Deactivate()
		{
			//throw new NotImplementedException();
		}

		public void GetPageInfo(PROPPAGEINFO[] pPageInfo)
		{
			if (pPageInfo == null)
				return;

			pPageInfo[0].pszTitle = "Issue Tracker";
			pPageInfo[0].SIZE.cx = 100;
			pPageInfo[0].SIZE.cy = 100;
		}

		public void Help(string pszHelpDir)
		{
			//throw new NotImplementedException();
		}

		public int IsPageDirty()
		{
			return VSConstants.S_FALSE; // Not dirty (S_OK = Dirty)
		}

		public void Move(RECT[] pRect)
		{
			throw new NotImplementedException();
		}

		public void SetObjects(uint cObjects, object[] ppunk)
		{
			//throw new NotImplementedException();
		}

		public void SetPageSite(IPropertyPageSite pPageSite)
		{
			_site = pPageSite;
		}

		public void Show(uint nCmdShow)
		{
			//throw new NotImplementedException();
		}

		public int TranslateAccelerator(MSG[] pMsg)
		{
			if (pMsg == null)
				return VSConstants.E_POINTER;

			Message message = Message.Create(pMsg[0].hwnd, (int)pMsg[0].message, pMsg[0].wParam, pMsg[0].lParam);

			Control control = Control.FromChildHandle(message.HWnd);

			if (control != null && control.PreProcessMessage(ref message))
				return VSConstants.S_OK;
			
			if (Site != null)
				return Site.TranslateAccelerator(pMsg);
			
			return VSConstants.S_OK;
		}

		public void EditProperty(int DISPID)
		{
			throw new NotImplementedException();
		}

		#endregion


		#region IPropertyPage2 Members


		void IPropertyPage2.Apply()
		{
			Apply();
		}

		#endregion
	}
}
