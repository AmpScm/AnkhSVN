// AutorunDlg.cpp : implementation file
//

#include "stdafx.h"
#include "Autorun.h"
#include "AutorunDlg.h"
#include "utils.h"
#include <string>
#include <shlobj.h>
#include <stdlib.h>
using namespace std;

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CAutorunDlg dialog

BEGIN_DHTML_EVENT_MAP(CAutorunDlg)
	DHTML_EVENT_ONCLICK(_T("ButtonOK"), OnButtonOK)

    DHTML_EVENT_ONCLICK(_T("processdoc"), Launch )
    DHTML_EVENT_ONCLICK(_T("log"), LaunchIE )
    DHTML_EVENT_ONCLICK(_T("ankhsetup"), LaunchExplorer )
END_DHTML_EVENT_MAP()


CAutorunDlg::CAutorunDlg(CWnd* pParent /*=NULL*/)
	: CDHtmlDialog(CAutorunDlg::IDD, CAutorunDlg::IDH, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CAutorunDlg::DoDataExchange(CDataExchange* pDX)
{
	CDHtmlDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAutorunDlg, CDHtmlDialog)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


// CAutorunDlg message handlers

BOOL CAutorunDlg::OnInitDialog()
{
	CDHtmlDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	// TODO: Add extra initialization here
	
	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CAutorunDlg::OnPaint() 
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDHtmlDialog::OnPaint();
	}
}

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CAutorunDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}
HRESULT CAutorunDlg::OnButtonOK(IHTMLElement* /*pElement*/)
{
	OnOK();
	return S_OK;
}

HRESULT CAutorunDlg::OnButtonCancel(IHTMLElement* /*pElement*/)
{
	OnCancel();
	return S_OK;
}

HRESULT CAutorunDlg::Launch( IHTMLElement* pElement )
{
    COleVariant path;
    pElement->getAttribute( CComBSTR( "path" ), 0, &path );

    string baseDir = GetModuleDirectory();
   
    HINSTANCE ret = ShellExecute( NULL, "open", COLE2T(path.bstrVal), NULL, baseDir.c_str(), SW_SHOW );

    return S_OK;

}

HRESULT CAutorunDlg::LaunchIE( IHTMLElement* pElement )
{
    COleVariant path;
    pElement->getAttribute( CComBSTR("path"), 0, &path );
    string baseDir = GetModuleDirectory();
    ::SetCurrentDirectory( baseDir.c_str() );

    TCHAR absPath[ MAX_PATH ];
    ::_fullpath( absPath, COLE2T( path.bstrVal ), MAX_PATH );

    HINSTANCE ret = ShellExecute( NULL, "open", "iexplore.exe", absPath, baseDir.c_str(), SW_SHOW );

    return S_OK;
}

HRESULT CAutorunDlg::LaunchExplorer( IHTMLElement* pElement )
{
    COleVariant path;
    pElement->getAttribute( CComBSTR( "path" ), 0, &path );

    string baseDir = GetModuleDirectory();
    ::SetCurrentDirectory( baseDir.c_str() );

     TCHAR absPath[ MAX_PATH ];
    ::_fullpath( absPath, COLE2T( path.bstrVal ), MAX_PATH );

    string args = "/e,/select," + string(absPath);
   
    HINSTANCE ret = ShellExecute( NULL, "open", "explorer.exe", args.c_str(), baseDir.c_str(), SW_SHOW );

    return S_OK;


}