#include "stdafx.h"
#include "resource.h"

#include "InstallLib.h"
#include "Logging.h"

// get the DTE libs
#pragma warning( disable : 4278 )
#import "libid:80cc9f66-e7d8-4ddd-85b6-d9e6cd0e93e2" version("7.0") lcid("0") raw_interfaces_only named_guids
#pragma warning( default : 4278 )

BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved )
{ return TRUE; }


HRESULT RemoveCommands (MSIHANDLE hModule, LPCOLESTR vsProgID)
{
    EnvDTE::_DTEPtr pDTE = NULL;
    pDTE.CreateInstance(vsProgID);

    if(pDTE == NULL)
    {
        LogString(hModule, _T("Unable to create DTE instance"));
        return S_OK;
    }

    EnvDTE::CommandsPtr pCommands = NULL;
    if(FAILED(pDTE->get_Commands(&pCommands)))
    {
        LogString(hModule, _T("Unable to get Commands collection"));
        return S_OK;
    }

    long lCount = 0;
    if(FAILED(pCommands->get_Count(&lCount)))
    {
        LogString(hModule, _T("Unable to get Commands count"));
        return S_OK;
    }
    for (long i = lCount - 1; i >= 0; i--)
    {
        EnvDTE::CommandPtr pCommand = NULL;
        if(SUCCEEDED(pCommands->Item(CComVariant(i), -1, &pCommand)))
        {
            BSTR bpName = NULL;
            if(SUCCEEDED(pCommand->get_Name(&bpName)) && bpName != NULL)
            {
                if (StrStrW(bpName, PROGID) != NULL)
                {
                    if(FAILED(pCommand->Delete()))
                    {
                        LogString(hModule, _T("Failed to delete command"));
                    }
                }
            }
        }

        if(pCommand != NULL)
            pCommand.Release();
    }

    pCommands.Release();

    pDTE->Quit();
    pDTE.Release();
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

    RemoveCommands(hModule, lpszVS70PROGID);
    RemoveCommands(hModule, lpszVS71PROGID);
    RemoveCommands(hModule, lpszVS80PROGID);

    CoUninitialize();

    return ERROR_SUCCESS;
}

