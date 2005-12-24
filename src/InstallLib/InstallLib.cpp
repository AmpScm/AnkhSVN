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


HRESULT RemoveCommands (LPCOLESTR vsProgID)
{
    CComPtr<EnvDTE::_DTE> m_pDTE;
    HRESULT hr = S_OK;

    CLSID clsid;
    CLSIDFromProgID (vsProgID, &clsid);
    hr = m_pDTE.CoCreateInstance(clsid);

    if (FAILED (hr))
    {
        return S_OK;
    }

    CComPtr<EnvDTE::Commands> pCommands;
    hr = m_pDTE->get_Commands(&pCommands);

    if (FAILED (hr))
        return S_OK;

    long lCount = 0;
    hr = pCommands->get_Count(&lCount);

    if (FAILED (hr))
        return S_OK;

    for (long i = 0; i < lCount; i++)
    {
        CComPtr<EnvDTE::Command> pCommand;
        hr = pCommands->Item(CComVariant(i), -1, &pCommand);

        if (FAILED (hr))
            continue;

        BSTR bpName;
        hr = pCommand->get_Name(&bpName);

        if (FAILED (hr) || bpName == NULL)
            continue;

        if (StrStrW(bpName, PROGID) != NULL)
            pCommand->Delete ();
    }

    return S_OK;
}

bool vsIsRunning(LPCOLESTR lpszProgID)
{
    CComPtr<EnvDTE::_DTE> m_pDTE;
    CLSID clsid;

    CLSIDFromProgID(lpszProgID, &clsid);

    CComPtr<IUnknown> pUnk;

    
    if(SUCCEEDED(GetActiveObject(clsid, NULL, &pUnk)))
    {
        // We found a running VS
        return true;
    }

    return false;
}

UINT __stdcall UnInstall ( MSIHANDLE hModule )
{
    CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
    while (vsIsRunning(lpszVS70PROGID) || vsIsRunning(lpszVS71PROGID) || vsIsRunning(lpszVS80PROGID))
    {
        MessageBox(NULL, "One or more instances of VS.NET are running. Please close these before continuing",
            "VS.NET is running", MB_OK | MB_ICONWARNING);
    }

    RemoveCommands(lpszVS70PROGID);
    RemoveCommands(lpszVS71PROGID);
    RemoveCommands(lpszVS80PROGID);

    CoUninitialize();

    return ERROR_SUCCESS;
}

