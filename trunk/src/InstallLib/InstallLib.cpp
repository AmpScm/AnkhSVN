#include "stdafx.h"
#include "resource.h"

#ifdef CUSTOM_DETAILS
#include "customdetails.h"
#endif

#include "InstallLib.h"

// get the DTE libs
#pragma warning( disable : 4278 )
#import "libid:80cc9f66-e7d8-4ddd-85b6-d9e6cd0e93e2" version("7.0") lcid("0") raw_interfaces_only named_guids
#pragma warning( default : 4278 )

BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved )
{ return TRUE; }

HRESULT RegRemove(const TCHAR* RegistryRoot)
{
    CRegKey rkCurrentUser(HKEY_CURRENT_USER);
    CRegKey rkLocalMachine(HKEY_LOCAL_MACHINE);

    rkCurrentUser.DeleteSubKey(RegistryRoot);
    rkLocalMachine.DeleteSubKey(RegistryRoot);

    return ERROR_SUCCESS;
}

HRESULT AddAboutBoxDetails(const TCHAR* RegistryRoot)
{
    CRegKey regKey;

    regKey.Create(HKEY_CURRENT_USER, RegistryRoot);
    regKey.SetStringValue(_T("AboutBoxDetails"), ABOUTBOXDETAILS);

    regKey.Create(HKEY_LOCAL_MACHINE, RegistryRoot);
    regKey.SetStringValue(_T("AboutBoxDetails"), ABOUTBOXDETAILS);

    return S_OK;
}

HRESULT RemoveCommands (LPCOLESTR vsProgID)
{
	CComPtr<EnvDTE::_DTE> m_pDTE;
    HRESULT hr = S_OK;

    CLSID clsid;
    CLSIDFromProgID (vsProgID, &clsid);
    hr = m_pDTE.CoCreateInstance(clsid);

    if (FAILED (hr))
    {
		return ERROR;
    }
		
    CComPtr<EnvDTE::Commands> pCommands;
    hr = m_pDTE->get_Commands(&pCommands);

    if (FAILED (hr))
        return ERROR;

    long lCount = 0;
    hr = pCommands->get_Count(&lCount);

    if (FAILED (hr))
        return ERROR;

    CComPtr<EnvDTE::Command> pCommand;
    BSTR bpName; 
    for (long i = 0; i < lCount; i++)
    {
        hr = pCommands->Item(CComVariant(i), -1, &pCommand);

        if (FAILED (hr))
            continue;

        bpName = NULL;
        hr = pCommand->get_Name(&bpName);

        if (FAILED (hr))
            continue;

        if (CString(bpName).Find(PROGID) == 0)
            pCommand->Delete ();
    }

    return ERROR_SUCCESS;
}

bool vsIsRunning(LPCOLESTR lpszProgID)
{
    CComPtr<EnvDTE::_DTE> m_pDTE;
    CLSID clsid;

    CLSIDFromProgID(lpszProgID, &clsid);

    CComPtr<IUnknown> punk;

    
    if(SUCCEEDED(GetActiveObject(clsid, NULL, &punk)))
    {
        // We found a running VS
        return true;
    }

    return false;
}

UINT __stdcall UnInstall ( MSIHANDLE hModule )
{
    while (vsIsRunning(lpszVS70PROGID) || vsIsRunning(lpszVS71PROGID))
    {
        MessageBox(NULL, "One or more instances of VS.NET are running. Please close these before continuing",
            "VS.NET is running", MB_OK | MB_ICONWARNING);
    }

    RegRemove(VS70REGPATH);
    RegRemove(VS71REGPATH);

    RemoveCommands(lpszVS70PROGID);
    RemoveCommands(lpszVS71PROGID);

    return S_OK;
}

UINT __stdcall Install ( MSIHANDLE hModule )
{
    AddAboutBoxDetails(VS70REGPATH);
    AddAboutBoxDetails(VS71REGPATH);

    return S_OK;
}

