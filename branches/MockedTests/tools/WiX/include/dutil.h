#pragma once
//-------------------------------------------------------------------------------------------------
// <copyright file="dutil.h" company="Microsoft">
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
//    Header for utility layer that provides standard support for asserts, exit macros
// </summary>
//-------------------------------------------------------------------------------------------------

// enums
enum REPORT_LEVEL { 
	REPORT_NONE,      // turns off report (only valid for XXXSetLevel())
	REPORT_STANDARD,  // written if reporting is on
	REPORT_VERBOSE,   // written only if verbose reporting is on
	REPORT_DEBUG,     // reporting useful when debugging code
	REPORT_ERROR,     // always gets reported, but can never be specified
	};

// asserts and traces
#ifdef DEBUG

typedef BOOL (*DUTIL_ASSERTDISPLAYFUNCTION)(LPCSTR sz);

extern "C" void Dutil_SetAssertModule(IN HMODULE hAssertModule);
extern "C" void Dutil_SetAssertDisplayFunction(IN DUTIL_ASSERTDISPLAYFUNCTION pfn);
extern "C" void Dutil_Assert(const CHAR* szFile, int iLine);
extern "C" void Dutil_AssertSz(const CHAR* szFile, int iLine, const CHAR *szMsg);

extern "C" void Dutil_TraceSetLevel(IN REPORT_LEVEL ll, IN BOOL fTraceFilenames);
extern "C" REPORT_LEVEL Dutil_TraceGetLevel();
extern "C" void Dutil_Trace(IN LPCSTR szFile, IN int iLine, IN REPORT_LEVEL rl, IN LPCSTR szMessage, IN ...);
extern "C" void Dutil_TraceError(IN LPCSTR szFile, IN int iLine, IN REPORT_LEVEL rl, IN HRESULT hr, IN LPCSTR szMessage, IN ...);

#endif

#if defined DEBUG

#define AssertSetModule(m) (void)Dutil_SetAssertModule(m)
#define AssertSetDisplayFunction(pfn) (void)Dutil_SetAssertDisplayFunction(pfn)
#define Assert(f)          ((f)    ? (void)0 : (void)Dutil_Assert(__FILE__, __LINE__))
#define AssertSz(f, sz)    ((f)    ? (void)0 : (void)Dutil_AssertSz(__FILE__, __LINE__, sz))

#define TraceSetLevel(l, f) (void)Dutil_TraceSetLevel(l, f)
#define TraceGetLevel() (REPORT_LEVEL)Dutil_TraceGetLevel()
#define Trace(l, f) (void)Dutil_Trace(__FILE__, __LINE__, l, f, NULL)
#define Trace1(l, f, s) (void)Dutil_Trace(__FILE__, __LINE__, l, f, s)
#define Trace2(l, f, s, t) (void)Dutil_Trace(__FILE__, __LINE__, l, f, s, t)
#define Trace3(l, f, s, t, u) (void)Dutil_Trace(__FILE__, __LINE__, l, f, s, t, u)

#define TraceError(x, f) (void)Dutil_TraceError(__FILE__, __LINE__, REPORT_ERROR, x, f, NULL)
#define TraceError1(x, f, s) (void)Dutil_TraceError(__FILE__, __LINE__, REPORT_ERROR, x, f, s)
#define TraceError2(x, f, s, t) (void)Dutil_TraceError(__FILE__, __LINE__, REPORT_ERROR, x, f, s, t)
#define TraceError3(x, f, s, t, u) (void)Dutil_TraceError(__FILE__, __LINE__, REPORT_ERROR, x, f, s, t, u)

#define TraceErrorDebug(x, f) (void)Dutil_TraceError(__FILE__, __LINE__, REPORT_DEBUG, x, f, NULL)
#define TraceErrorDebug1(x, f, s) (void)Dutil_TraceError(__FILE__, __LINE__, REPORT_DEBUG, x, f, s)
#define TraceErrorDebug2(x, f, s, t) (void)Dutil_TraceError(__FILE__, __LINE__, REPORT_DEBUG, x, f, s, t)
#define TraceErrorDebug3(x, f, s, t, u) (void)Dutil_TraceError(__FILE__, __LINE__, REPORT_DEBUG, x, f, s, t, u)

#else // !DEBUG

#define AssertSetModule(m)
#define AssertSetDisplayFunction(pfn)
#define Assert(f)
#define AssertSz(f, sz)

#define TraceSetLevel(l, f)
#define Trace(l, f)
#define Trace1(l, f, s)
#define Trace2(l, f, s, t)
#define Trace3(l, f, s, t, u)

#define TraceError(x, f)
#define TraceError1(x, f, s)
#define TraceError2(x, f, s, t)
#define TraceError3(x, f, s, t, u)

#define TraceErrorDebug(x, f)
#define TraceErrorDebug1(x, f, s)
#define TraceErrorDebug2(x, f, s, t)
#define TraceErrorDebug3(x, f, s, t, u)

#endif // DEBUG


// ExitTrace can be overriden
#ifndef ExitTrace
#define ExitTrace TraceError
#endif
#ifndef ExitTrace1
#define ExitTrace1 TraceError1
#endif
#ifndef ExitTrace2
#define ExitTrace2 TraceError2
#endif
#ifndef ExitTrace3
#define ExitTrace3 TraceError3
#endif

// Exit macros
#define ExitFunction()        { goto LExit; }
#define ExitFunction1(x)          { x; goto LExit; }

#define ExitOnLastError(x, s) { x = ::GetLastError(); x = HRESULT_FROM_WIN32(x); if (FAILED(x)) { ExitTrace(x, s); goto LExit; } }
#define ExitOnLastError1(x, f, s) { x = ::GetLastError(); x = HRESULT_FROM_WIN32(x); if (FAILED(x)) { ExitTrace1(x, f, s); goto LExit; } }
#define ExitOnLastError2(x, f, s, t) { x = ::GetLastError(); x = HRESULT_FROM_WIN32(x); if (FAILED(x)) { ExitTrace2(x, f, s, t); goto LExit; } }

#define ExitOnLastErrorDebugTrace(x, s) { x = ::GetLastError(); x = HRESULT_FROM_WIN32(x); if (FAILED(x)) { TraceErrorDebug(x, s); goto LExit; } }
#define ExitOnLastErrorDebugTrace1(x, f, s) { x = ::GetLastError(); x = HRESULT_FROM_WIN32(x); if (FAILED(x)) { TraceErrorDebug1(x, f, s); goto LExit; } }
#define ExitOnLastErrorDebugTrace2(x, f, s, t) { x = ::GetLastError(); x = HRESULT_FROM_WIN32(x); if (FAILED(x)) { TraceErrorDebug2(x, f, s, t); goto LExit; } }

#define ExitOnFailure(x, s)   if (FAILED(x)) { ExitTrace(x, s);  goto LExit; }
#define ExitOnFailure1(x, f, s)   if (FAILED(x)) { ExitTrace1(x, f, s);  goto LExit; }
#define ExitOnFailure2(x, f, s, t)   if (FAILED(x)) { ExitTrace2(x, f, s, t);  goto LExit; }
#define ExitOnFailure3(x, f, s, t, u) if (FAILED(x)) { ExitTrace3(x, f, s, t, u);  goto LExit; }

#define ExitOnFailureDebugTrace(x, s)   if (FAILED(x)) { TraceErrorDebug(x, s);  goto LExit; }
#define ExitOnFailureDebugTrace1(x, f, s)   if (FAILED(x)) { TraceErrorDebug1(x, f, s);  goto LExit; }
#define ExitOnFailureDebugTrace2(x, f, s, t)   if (FAILED(x)) { TraceErrorDebug2(x, f, s, t);  goto LExit; }
#define ExitOnFailureDebugTrace3(x, f, s, t, u) if (FAILED(x)) { TraceErrorDebug3(x, f, s, t, u);  goto LExit; }

#define ExitOnNull(p, x, e, s)   if (NULL == p) { x = e; ExitTrace(x, s);  goto LExit; }
#define ExitOnNull1(p, x, e, f, s)   if (NULL == p) { x = e; ExitTrace1(x, f, s);  goto LExit; }

#define ExitOnNullWithLastError(p, x, s) if (NULL == p) { x = ::GetLastError(); x = HRESULT_FROM_WIN32(x); if (!FAILED(x)) { x = E_FAIL; } ExitTrace(x, s); goto LExit; }

#define ExitOnNullDebugTrace(p, x, e, s)   if (NULL == p) { x = e; TraceErrorDebug(x, s);  goto LExit; }
#define ExitOnNullDebugTrace1(p, x, e, f, s)   if (NULL == p) { x = e; TraceErrorDebug1(x, f, s);  goto LExit; }

#define ExitOnNtError(x, s)   if (NT_ERROR(x))  { ExitTrace(x, s);  goto LExit; }
#define ExitOnNtError1(x, f, s)   if (NT_ERROR(x))  { ExitTrace1(x, f, s);  goto LExit; }


// release macros
#define ReleaseObject(x) if (x) { x->Release(); }
#define ReleaseVariant(x) { ::VariantClear(&x); }
#define ReleaseNullObject(x) if (x) { (x)->Release(); x = NULL; }
#define ReleaseCertificate(x) if (x) { ::CertFreeCertificateContext(x); x=NULL; }


// useful defines and macros
#define Unused(x) ((void)x)

#if 1
#define countof(ary) (sizeof(ary) / sizeof(ary[0]))
#else
#ifndef __cplusplus
#define countof(ary) (sizeof(ary) / sizeof(ary[0]))
#else
template<typename T> static char countofVerify(void const *, T) throw() { return 0; }
template<typename T> static void countofVerify(T *const, T *const *) throw() {};
#define countof(arr) (sizeof(countofVerify(arr,&(arr))) * sizeof(arr)/sizeof(*(arr)))
#endif
#endif


#define E_NOMOREITEMS HRESULT_FROM_WIN32(ERROR_NO_MORE_ITEMS)
#define AddRefAndRelease(x) { x->AddRef(); x->Release(); }
