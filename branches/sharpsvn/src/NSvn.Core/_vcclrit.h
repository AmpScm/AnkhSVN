/***
*_vcclrit.h
*
*       Copyright (c) Microsoft Corporation. All rights reserved.
*
*Purpose:
*       This file defines the functions and variables used by user
*       to initialize CRT and the dll in IJW scenario.
*
****/

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

extern IMAGE_DOS_HEADER __ImageBase;

BOOL WINAPI _DllMainCRTStartup(
        HANDLE  hDllHandle,
        DWORD   dwReason,
        LPVOID  lpreserved
        );
#ifdef __cplusplus
}
#endif

#ifdef _cplusplus
#define __USE_GLOBAL_NAMESPACE  ::
#else
#define __USE_GLOBAL_NAMESPACE
#endif

// Used to lock 
__declspec( selectany ) LONG  volatile __lock_handle = 0;

// Init called
__declspec(selectany) BOOL volatile __initialized = FALSE;

// Term called
__declspec( selectany ) BOOL volatile __terminated = FALSE;

__inline BOOL WINAPI __crt_dll_initialize()
{
    // Try to make the variable names unique, so that the variables don't even clash with macros.
    static BOOL volatile (__retval) = FALSE;
    static DWORD volatile (__lockThreadId) = 0xffffffff;
    DWORD volatile (__currentThreadId) = __USE_GLOBAL_NAMESPACE(GetCurrentThreadId)();
    int (__int_var)=0;
    
    // Take Lock, This is needed for multithreaded scenario. Moreover the threads
    // need to wait here to make sure that the dll is initialized when they get
    // past this function.
    while ( __USE_GLOBAL_NAMESPACE(InterlockedExchange)( &(__lock_handle), 1) == 1 )
	{
        ++(__int_var);
        if ((__lockThreadId) == (__currentThreadId)) 
        {
            return TRUE;
        }
		__USE_GLOBAL_NAMESPACE(Sleep)( (__int_var)>1000?100:0 );

        // If you hang in this loop, this implies that your dllMainCRTStartup is hung on another
        // thread. The most likely cause of this is a hang in one of your static constructors or
        // destructors.
	}
    // Note that we don't really need any interlocked stuff here as the writes are always
    // in the lock. Only reads are outside the lock.
    (__lockThreadId) = (__currentThreadId);
    __try {
    
        if ( (__terminated) == TRUE )
        {
            (__retval) = FALSE;
        }
        else if ( (__initialized) == FALSE )
        {
            (__retval) = (_DllMainCRTStartup)( ( HINSTANCE )( &__ImageBase ), DLL_PROCESS_ATTACH, 0 );
            (__initialized) = TRUE;
        }

    } __finally {
        // revert the __lockThreadId
        (__lockThreadId) = 0xffffffff;
        // Release Lock
        __USE_GLOBAL_NAMESPACE(InterlockedExchange)( &(__lock_handle), 0 );
    }
    return (__retval);
}

__inline BOOL WINAPI __crt_dll_terminate()
{
    static BOOL volatile (__retval) = TRUE;
    static DWORD volatile (__lockThreadId) = 0xffffffff;
    DWORD volatile (__currentThreadId) = __USE_GLOBAL_NAMESPACE(GetCurrentThreadId)();
    int (__int_var)=0;
    
    // Take Lock, this lock is needed to keep Terminate in sync with Initialize.
    while ( __USE_GLOBAL_NAMESPACE(InterlockedExchange)( &(__lock_handle), 1) == 1 )
	{
        ++(__int_var);
        if ((__lockThreadId) == (__currentThreadId)) 
        {
            return TRUE;
        }
		__USE_GLOBAL_NAMESPACE(Sleep)( (__int_var)>1000?100:0 );

        // If you hang in this loop, this implies that your dllMainCRTStartup is hung on another
        // thread. The most likely cause of this is a hang in one of your static constructors or
        // destructors.
    }
    // Note that we don't really need any interlocked stuff here as the writes are always
    // in the lock. Only reads are outside the lock.
    (__lockThreadId) = (__currentThreadId);
    __try {

        if ( (__initialized) == FALSE )
        {
            (__retval) = FALSE;
        }
        else if ( (__terminated) == FALSE )
        {
            (__retval) = _DllMainCRTStartup( ( HINSTANCE )( &(__ImageBase) ), DLL_PROCESS_DETACH, 0 );
            (__terminated) = TRUE;
        }

    } __finally {
        // revert the __lockThreadId
        (__lockThreadId) = 0xffffffff;
        // Release Lock
        __USE_GLOBAL_NAMESPACE(InterlockedExchange)( &(__lock_handle), 0 );
    }
    return (__retval);
}
