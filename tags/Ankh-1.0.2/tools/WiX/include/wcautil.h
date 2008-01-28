#pragma once
//-------------------------------------------------------------------------------------------------
// <copyright file="wcautil.h" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
//
//    The use and distribution terms for this software are covered by the
//    Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
//    which can be found in the file CPL.TXT at the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by
//    the terms of this license.
//
//    You must not remove this notice, or any other, from this software.
// </copyright>
// 
// <summary>
//    Windows Installer XML CustomAction utility library.
// </summary>
//-------------------------------------------------------------------------------------------------

#ifdef __cplusplus
extern "C" {
#endif

#define ExitTrace WcaLogError
#define ExitTrace1 WcaLogError
#define ExitTrace2 WcaLogError
#define ExitTrace3 WcaLogError

#include "dutil.h"

#define MessageExitOnLastError(x, e, s)      { x = ::GetLastError(); x = HRESULT_FROM_WIN32(x); if (FAILED(x)) { ExitTrace(x, "%s", s); WcaErrorMessage(e, x, MB_OK, 0);  goto LExit; } }
#define MessageExitOnLastError1(x, e, f, s)  { x = ::GetLastError(); x = HRESULT_FROM_WIN32(x); if (FAILED(x)) { ExitTrace1(x, f, s); WcaErrorMessage(e, x, MB_OK, 1, s);  goto LExit; } }

#define MessageExitOnFailure(x, e, s)           if (FAILED(x)) { ExitTrace(x, "%s", s); WcaErrorMessage(e, x, INSTALLMESSAGE_ERROR | MB_OK, 0);  goto LExit; }
#define MessageExitOnFailure1(x, e, f, s)       if (FAILED(x)) { ExitTrace1(x, f, s); WcaErrorMessage(e, x, INSTALLMESSAGE_ERROR | MB_OK, 1, s);  goto LExit; }
#define MessageExitOnFailure2(x, e, f, s, t)    if (FAILED(x)) { ExitTrace2(x, f, s, t); WcaErrorMessage(e, x, INSTALLMESSAGE_ERROR | MB_OK, 2, s, t);  goto LExit; }
#define MessageExitOnFailure3(x, e, f, s, t, u) if (FAILED(x)) { ExitTrace2(x, f, s, t, u); WcaErrorMessage(e, x, INSTALLMESSAGE_ERROR | MB_OK, 3, s, t, u);  goto LExit; }


#define MAX_DARWIN_KEY 73
#define MAX_DARWIN_COLUMN 255

void WcaGlobalInitialize(
	IN HINSTANCE hInst
	);
void WcaGlobalFinalize();

HRESULT WcaInitialize(
	IN MSIHANDLE hInstall,
	IN const char* szCustomActionLogName
	);
UINT WcaFinalize(
	IN UINT iReturnValue
	);
BOOL WcaIsInitialized();

MSIHANDLE WcaGetInstallHandle();
MSIHANDLE WcaGetDatabaseHandle();

const char* WcaGetLogName();

void WcaSetReturnValue(
	IN UINT iReturnValue
	);
BOOL WcaCancelDetected();

const int LOG_BUFFER = 2048;
enum LOGLEVEL
{ 
	LOGMSG_TRACEONLY,  // Never written to the log file (except in DEBUG builds)
	LOGMSG_VERBOSE,    // Written to log when LOGVERBOSE
	LOGMSG_STANDARD    // Written to log whenever informational logging is enabled
};

void WcaLog(
	IN LOGLEVEL llv,
	IN const char* fmt, ...
	);
BOOL WcaDisplayAssert(
	IN LPCSTR sz
	);
void WcaLogError(
	IN HRESULT hr,
	IN LPCSTR szMessage,
	IN ...
	);


UINT WcaProcessMessage(
	IN INSTALLMESSAGE eMessageType,
	IN MSIHANDLE hRecord
	);
UINT WcaErrorMessage(
	IN int iError, 
	IN HRESULT hrError, 
	IN UINT uiType, 
	IN DWORD cArgs, 
	IN ...
	);
HRESULT WcaProgressMessage(
	IN UINT uiCost,
	IN BOOL fExtendProgressBar
	);

BOOL WcaIsInstalling(
	IN INSTALLSTATE isInstalled,
	IN INSTALLSTATE isAction
	);
BOOL WcaIsReInstalling(
	IN INSTALLSTATE isInstalled,
	IN INSTALLSTATE isAction
	);
BOOL WcaIsUninstalling(
	IN INSTALLSTATE isInstalled,
	IN INSTALLSTATE isAction
	);

HRESULT WcaSetComponentState(
	IN LPCWSTR wzComponent,
	IN INSTALLSTATE isState
	);

HRESULT WcaTableExists(
	IN LPCWSTR wzTable
	);

HRESULT WcaOpenView(
	IN LPCWSTR wzSql,
	OUT MSIHANDLE* phView
	);
HRESULT WcaExecuteView(
	IN MSIHANDLE hView,
	IN MSIHANDLE hRec
	);
HRESULT WcaOpenExecuteView(
	IN LPCWSTR wzSql,
	OUT MSIHANDLE* phView
	);
HRESULT WcaFetchRecord(
	IN MSIHANDLE hView,
	OUT MSIHANDLE* phRec
	);
HRESULT WcaFetchSingleRecord(
	IN MSIHANDLE hView,
	OUT MSIHANDLE* phRec
	);

HRESULT WcaGetProperty(
	IN LPCWSTR wzProperty,
	IN OUT LPWSTR* ppwzData
	);
HRESULT WcaGetFormattedProperty(
	IN LPCWSTR wzProperty,
	IN OUT LPWSTR* ppwzData
	);
HRESULT WcaGetIntProperty(
	IN LPCWSTR wzProperty,
	IN OUT int* piData
	);
HRESULT WcaGetTargetPath(
	IN LPCWSTR wzFolder,
	OUT LPWSTR* ppwzData
	);
HRESULT WcaSetProperty(
	IN LPCWSTR wzPropertyName,
	IN LPCWSTR wzPropertyValue
	);
HRESULT WcaSetIntProperty(
	IN LPCWSTR wzPropertyName,
	IN int nPropertyValue
	);
BOOL WcaIsPropertySet(
	IN LPCSTR szProperty
	);

HRESULT WcaGetRecordInteger(
	IN MSIHANDLE hRec,
	IN UINT uiField,
	IN OUT int* piData
	);
HRESULT WcaGetRecordString(
	IN MSIHANDLE hRec,
	IN UINT uiField,
	IN OUT LPWSTR* ppwzData
	);
HRESULT WcaGetRecordFormattedString(
	IN MSIHANDLE hRec,
	IN UINT uiField,
	IN OUT LPWSTR* ppwzData
	);

HRESULT WcaAllocStream(BYTE** ppbData, DWORD cbData);
HRESULT WcaFreeStream(BYTE* pbData);

HRESULT WcaGetRecordStream(
	IN MSIHANDLE hRecBinary,
	IN UINT uiField, 
	IN OUT BYTE** ppbData,
	IN OUT DWORD* pcbData
	);
HRESULT WcaSetRecordString(
	IN MSIHANDLE hRec,
	IN UINT uiField,
	IN LPCWSTR wzData
	);
HRESULT WcaSetRecordInteger(
	IN MSIHANDLE hRec,
	IN UINT uiField,
	IN int iValue
	);
HRESULT WcaSplitPath(
	IN LPCWSTR wzPath,
	IN BOOL fLong,
	IN OUT LPWSTR* ppwzData
	);

HRESULT WcaDoDeferredAction(
	IN LPCWSTR wzAction,
	IN LPCWSTR wzCustomActionData,
	IN UINT uiCost
	);
DWORD WcaCountOfCustomActionDataRecords(
	IN LPCWSTR wzData
	);
HRESULT WcaReadStringFromCaData(
	IN OUT LPWSTR* ppwzCustomActionData,
	IN OUT LPWSTR* ppwzString
	);
HRESULT WcaReadIntegerFromCaData(
	IN OUT LPWSTR* ppwzCustomActionData,
	IN OUT int* piResult
	);
HRESULT WcaReadStreamFromCaData(
	IN OUT LPWSTR* ppwzCustomActionData,
	OUT BYTE** ppbData,
	OUT DWORD* pcbData
	);
HRESULT WcaWriteStringToCaData(
	IN LPCWSTR wzString,
	IN OUT LPWSTR* ppwzCustomActionData
	);
HRESULT WcaWriteIntegerToCaData(
	IN int i, 
	IN OUT LPWSTR* ppwzCustomActionData
	);
HRESULT WcaWriteStreamToCaData(
	IN const BYTE* pbData,
	IN DWORD cbData,
	IN OUT LPWSTR* ppwzCustomActionData
	);

HRESULT WcaAddTempRecord(
	IN OUT MSIHANDLE* phTableView,
	IN OUT MSIHANDLE* phColumns,
	IN LPCWSTR wzTable,
	IN UINT uiUniquifyColumn,
	IN UINT cColumns,
	IN ...
	);

#ifdef __cplusplus
}
#endif
