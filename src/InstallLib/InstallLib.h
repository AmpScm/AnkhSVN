#ifndef InstallLib_
#define InstallLib_

#include <tchar.h>


const TCHAR* PROGID      = _T("Ankh");
const TCHAR* VS70REGPATH = _T("Software\\Microsoft\\VisualStudio\\7.0\\AddIns\\Ankh");
const TCHAR* VS71REGPATH = _T("Software\\Microsoft\\VisualStudio\\7.1\\AddIns\\Ankh");

#ifndef CUSTOM_DETAILS
const TCHAR* ABOUTBOXDETAILS = _T("AnkhSVN (built with VS.NET)");
#endif

const LPCOLESTR lpszVS70PROGID = L"VisualStudio.DTE.7";
const LPCOLESTR lpszVS71PROGID = L"VisualStudio.DTE.7.1";

#endif