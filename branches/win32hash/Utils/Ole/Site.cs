// $Id$
using System;
using System.Windows.Forms;
using stdole;

namespace Utils.Ole
{
	/// <summary>
	/// Represents the "site" that hosts a webbrowser control.
	/// </summary>
	public class Site :
        IOleClientSite, IDocHostUIHandler,
        IOleDocumentSite, IDocHostShowUI
	{
		public Site()
		{
        }        

        public void HostOleObject( IOleObject obj )
        {
            if ( obj == null )
                throw new ArgumentNullException( "browser" );            

            obj.SetClientSite( this );
        }

        /// <summary>
        /// The IDispatch object that will be used to provide scripting services.
        /// </summary>
        public virtual IDispatch External
        {
            get{ return this.external; }
            set{ this.external = value; }
        }

        #region IOleClientSite Members

        public virtual void SaveObject()
        {
        }

        public virtual void GetMoniker(uint dwAssign, uint dwWhichMoniker, ref object ppmk)
        {
        }

        public virtual void GetContainer(ref object ppContainer)
        {
        }

        public virtual void ShowObject()
        {
        }

        public virtual void OnShowWindow(bool fShow)
        {
        }

        public virtual void RequestNewObjectLayout()
        {
        }

        #endregion

        #region IDocHostUIHandler2 Members

        public virtual void GetOverrideKeyPath(ref string pchKey, uint dw)
        {
        }

        #endregion

        #region IDocHostUIHandler Members

        public virtual uint ShowContextMenu(uint dwID, ref mshtml.tagPOINT ppt, object pcmdtReserved, object pdispReserved)
        {
            return MenuNotHandled;
        }

        public virtual void GetHostInfo(ref DOCHOSTUIINFO pInfo)
        {
            pInfo.dwFlags |= (uint) (DOCHOSTUIFLAG.DOCHOSTUIFLAG_SCROLL_NO |
                DOCHOSTUIFLAG.DOCHOSTUIFLAG_NO3DBORDER |
                DOCHOSTUIFLAG.DOCHOSTUIFLAG_DIALOG);
        }

        public virtual void ShowUI(uint dwID, ref object pActiveObject, ref object pCommandTarget, ref object pFrame, ref object pDoc)
        {
        }

        public virtual void HideUI()
        {
        }

        public virtual void UpdateUI()
        {
        }

        public virtual void EnableModeless(int fEnable)
        {
        }

        public virtual void OnDocWindowActivate(int fActivate)
        {
        }

        public virtual void OnFrameWindowActivate(int fActivate)
        {
        }

        public virtual void ResizeBorder(ref mshtml.tagRECT prcBorder, int pUIWindow, int fRameWindow)
        {
        }

        public virtual uint TranslateAccelerator(ref tagMSG lpMsg, ref Guid pguidCmdGroup, uint nCmdID)
        {
            return AllowAccelerator;
        }

        public virtual void GetOptionKeyPath(ref string pchKey, uint dw)
        {
        }

        public virtual object GetDropTarget(ref object pDropTarget)
        {
            return null;
        }

        public virtual void GetExternal(out object ppDispatch)
        {
            ppDispatch = this.External;
        }

        public virtual uint TranslateUrl(uint dwTranslate, string pchURLIn, ref string ppchURLOut)
        {
            return UrlNotTranslated;
        }

        public virtual System.Windows.Forms.IDataObject FilterDataObject(System.Windows.Forms.IDataObject pDO)
        {
            return null;
        }

        #endregion	
        #region IOleDocumentSite Members

        public virtual void ActivateMe(ref object pViewToActivate)
        {
            // TODO:  Add Form1.ActivateMe implementation
        }

        #endregion
    
        #region IDocHostShowUI Members

        public virtual uint ShowMessage(IntPtr hwnd, string lpstrText, string lpstrCaption, uint dwType, string lpstrHelpFile, uint dwHelpContext, out int lpResult)
        {
            lpResult = 0;
            return MessageNotHandled;
        }

        public virtual uint ShowHelp(IntPtr hwnd, string pszHelpFile, uint uCommand, uint dwData, mshtml.tagPOINT ptMouse, object pDispatchObjectHit)
        {
            return HelpNotHandled;
        }

        #endregion

        private IDispatch external;  

        protected const int HelpHandled = 0;    // MsHtml won't display its Help window
        protected const int HelpNotHandled = 1; // MsHtml will display its Help window 
       
        protected const int MessageHandled = 0;    // MsHtml won't display its MessageBox
        protected const int MessageNotHandled = 1; // MsHtml will display its MessageBox

        protected const int UrlTranslated = 0;
        protected const int UrlNotTranslated = 1;

        protected const int KillAccelerator = 0;
        protected const int AllowAccelerator = 1;

        protected const int MenuHandled = 0;
        protected const int MenuNotHandled = 1;
    }
}
