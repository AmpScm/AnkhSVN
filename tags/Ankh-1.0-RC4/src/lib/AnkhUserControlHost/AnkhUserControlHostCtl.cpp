// AnkhUserControlHostCtl.cpp : Implementation of CAnkhUserControlHostCtl
#include "stdafx.h"
#include "AnkhUserControlHost.h"

//An interface defined by the .NET Frameworks, but it is not declared in any header file. 
MIDL_INTERFACE("458AB8A2-A1EA-4D7B-8EBE-DEE5D3D9442C")
IWin32Window : public IUnknown { public: STDMETHOD(get_Handle)(long *pHWnd); };

// CActiveXShimCtl
class ATL_NO_VTABLE CAnkhUserControlHostCtl : 
	public IDispatchImpl<IAnkhUserControlHostCtlCtl, &IID_IAnkhUserControlHostCtlCtl, &LIBID_AnkhUserControlHostLib, /*wMajor =*/ 1, /*wMinor =*/ 0>,
	public CComObjectRootEx<CComSingleThreadModel>,
	public IPersistStreamInitImpl<CAnkhUserControlHostCtl>,
	public IOleControlImpl<CAnkhUserControlHostCtl>,
	public IOleObjectImpl<CAnkhUserControlHostCtl>,
	public IOleInPlaceActiveObjectImpl<CAnkhUserControlHostCtl>,
	public IViewObjectExImpl<CAnkhUserControlHostCtl>,
	public IOleInPlaceObjectWindowlessImpl<CAnkhUserControlHostCtl>,
	public ISupportErrorInfo,
	public IPersistStorageImpl<CAnkhUserControlHostCtl>,
	public ISpecifyPropertyPagesImpl<CAnkhUserControlHostCtl>,
	public IQuickActivateImpl<CAnkhUserControlHostCtl>,
	public IDataObjectImpl<CAnkhUserControlHostCtl>,
	public CComCompositeControl<CAnkhUserControlHostCtl>,
	public IProvideClassInfo2Impl<&CLSID_AnkhUserControlHostCtl, NULL, &LIBID_AnkhUserControlHostLib>,
	public CComCoClass<CAnkhUserControlHostCtl, &CLSID_AnkhUserControlHostCtl>
{
public:
	DECLARE_OLEMISC_STATUS(OLEMISC_RECOMPOSEONRESIZE | OLEMISC_CANTLINKINSIDE | OLEMISC_INSIDEOUT | OLEMISC_ACTIVATEWHENVISIBLE | OLEMISC_SETCLIENTSITEFIRST)
	DECLARE_REGISTRY_RESOURCEID(IDR_XTREMESIMPLICITYCONTROLHOSTCTL)

	BEGIN_COM_MAP(CAnkhUserControlHostCtl)
		COM_INTERFACE_ENTRY(IAnkhUserControlHostCtlCtl)
		COM_INTERFACE_ENTRY(IDispatch)
		COM_INTERFACE_ENTRY(IViewObjectEx)
		COM_INTERFACE_ENTRY(IViewObject2)
		COM_INTERFACE_ENTRY(IViewObject)
		COM_INTERFACE_ENTRY(IOleInPlaceObjectWindowless)
		COM_INTERFACE_ENTRY(IOleInPlaceObject)
		COM_INTERFACE_ENTRY2(IOleWindow, IOleInPlaceObjectWindowless)
		COM_INTERFACE_ENTRY(IOleInPlaceActiveObject)
		COM_INTERFACE_ENTRY(IOleControl)
		COM_INTERFACE_ENTRY(IOleObject)
		COM_INTERFACE_ENTRY(IPersistStreamInit)
		COM_INTERFACE_ENTRY2(IPersist, IPersistStreamInit)
		COM_INTERFACE_ENTRY(ISupportErrorInfo)
		COM_INTERFACE_ENTRY(ISpecifyPropertyPages)
		COM_INTERFACE_ENTRY(IQuickActivate)
		COM_INTERFACE_ENTRY(IPersistStorage)
		COM_INTERFACE_ENTRY(IDataObject)
		COM_INTERFACE_ENTRY(IProvideClassInfo)
		COM_INTERFACE_ENTRY(IProvideClassInfo2)
	END_COM_MAP()

	BEGIN_PROP_MAP(CAnkhUserControlHostCtl)
	END_PROP_MAP()

	BEGIN_MSG_MAP(CAnkhUserControlHostCtl)
		MESSAGE_HANDLER(WM_ERASEBKGND, OnEraseBackground)
		MESSAGE_HANDLER(WM_SIZE, OnSize)
		MESSAGE_HANDLER(WM_SHOWWINDOW, OnShow) 
		CHAIN_MSG_MAP(CComCompositeControl<CAnkhUserControlHostCtl>)
	END_MSG_MAP()

	//BEGIN_SINK_MAP(CAnkhUserControlHostCtl)
	//END_SINK_MAP()

	STDMETHOD(InterfaceSupportsErrorInfo)(REFIID riid)
	{
		if(InlineIsEqualGUID(IID_IAnkhUserControlHostCtlCtl, riid))
			return S_OK;
		return S_FALSE;
	}

	DECLARE_VIEW_STATUS(VIEWSTATUS_OPAQUE)
	enum { IDD = IDD_XTREMESIMPLICITYCONTROLHOSTCTL };

	// Instance data:  
	IUnknown *m_pUserControl;   // the current hosted control's interface pointer, 
	HWND m_hwndUsercontrol;     // its window handle, and the connected and visibility states.
	bool m_fConnected;				   // have we connected the hosted user control?
	bool m_fVisible;						   // is this control visible?

	// Initialise instance data.
	CAnkhUserControlHostCtl()
	{
		m_hwndUsercontrol = 0;
		m_bWindowOnly = true;
		CalcExtent(m_sizeExtent);
		m_fConnected = m_fVisible = false;		
	}

	// This is where the addin calls to host the user control on the shim.
	// We store references to the user control and it's window handle, and, 
	// if visible, we connect the user control.
	STDMETHOD(HostUserControl)(IUnknown *pUserControl)
	{
		m_pUserControl = pUserControl;
		HRESULT hr;
		CComPtr<IWin32Window> pIWin32Window;
		if(SUCCEEDED(hr = pUserControl->QueryInterface(__uuidof(IWin32Window), (LPVOID*)&pIWin32Window))
			&& SUCCEEDED(hr = pIWin32Window->get_Handle((LONG*)&m_hwndUsercontrol)))
		{
			ConnectUserControl();
		}
		else
		{
			m_hwndUsercontrol = 0;
			m_pUserControl = NULL;
		}
		return hr;
	}

	// When the shim is resized, resize the hosted user control.
	STDMETHOD(OnSize)(UINT, WPARAM, LPARAM lParam, BOOL& bHandled)
	{
		::MoveWindow(m_hwndUsercontrol, 0, 0, LOWORD(lParam), HIWORD(lParam), true);
		bHandled = true;
		return 0;
	}

	// The hosted user control covers the entire client area. No background erasing is needed.
	STDMETHOD(OnEraseBackground)(UINT, WPARAM, LPARAM, BOOL& bHandled)
	{
		bHandled = TRUE;
		return 0;
	}

	// If we haven't already connected the hosted control, do it when the shim is shown.
	STDMETHOD(OnShow)(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
	{
		if(wParam == TRUE)
		{
			m_fVisible = true;
			if(!m_fConnected)
				ConnectUserControl();
		}
		return S_OK;
	}

	// Sets up the hosting arrangement, sizes the user control and sets focus to it.
	// This is called either when setting the hosted control, or when the shim becomes visible thru OnShow
	void ConnectUserControl()
	{
		if(m_hwndUsercontrol && m_fVisible)
		{
			::SetParent(m_hwndUsercontrol, m_hWnd);
			RECT rc;
			::GetWindowRect(m_hWnd, &rc);
			::MoveWindow(m_hwndUsercontrol, 0, 0, rc.right-rc.left, rc.bottom-rc.top, true);
			::ShowWindow(m_hwndUsercontrol, SW_SHOW);
			::SetFocus(m_hwndUsercontrol);
			m_fConnected = true;
		}
	}

	// Here we get the chance to specify which keys we want processed.
	// The purpose really is to pass the F3..5 keys up to VS.NET
	BOOL PreTranslateAccelerator(LPMSG pMsg, HRESULT& hRet)
	{
		if(pMsg->message == WM_KEYDOWN)
		{
			switch(LOWORD(pMsg->wParam))
			{
			case VK_TAB:
			case VK_LEFT:
			case VK_UP:
			case VK_RIGHT:
			case VK_DOWN:
			case VK_EXECUTE:
			case VK_RETURN:
			case VK_ESCAPE:
			case VK_CANCEL:
			case VK_F3:
			case VK_F4:
			case VK_F5:
				return ::SendMessage(m_hwndUsercontrol, pMsg->message, pMsg->wParam, pMsg->lParam) == 1;
			}
		}
		return CComCompositeControl<CAnkhUserControlHostCtl>::PreTranslateAccelerator(pMsg, hRet);
	}

	// The TranslateAccelerator method is proxied thru to the UserControl 
	//  using its IOleInPlaceActiveObject interface
	STDMETHOD(TranslateAccelerator)(LPMSG pMsg)
	{
		CComPtr<IOleInPlaceActiveObject> pIPAO;
		if(m_pUserControl && m_pUserControl->QueryInterface(__uuidof(IOleInPlaceActiveObject), (LPVOID*)&pIPAO) == S_OK && (pIPAO.p))
			if(pIPAO->TranslateAccelerator(pMsg) == S_OK) 
				return S_OK;
		return IOleInPlaceActiveObjectImpl<CAnkhUserControlHostCtl>::TranslateAccelerator(pMsg);
	}
};

// this line generates the control's self-registration and instantiation code
OBJECT_ENTRY_AUTO(__uuidof(AnkhUserControlHostCtl), CAnkhUserControlHostCtl)
