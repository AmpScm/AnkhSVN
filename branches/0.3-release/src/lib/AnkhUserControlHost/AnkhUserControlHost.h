
#pragma warning( disable: 4049 )  /* more than 64k source lines */

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 6.00.0347 */
/* at Tue Oct 14 20:33:13 2003
 */
/* Compiler settings for AnkhUserControlHost.idl:
    Oicf, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 440
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __AnkhUserControlHost_h__
#define __AnkhUserControlHost_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IAnkhUserControlHostCtlCtl_FWD_DEFINED__
#define __IAnkhUserControlHostCtlCtl_FWD_DEFINED__
typedef interface IAnkhUserControlHostCtlCtl IAnkhUserControlHostCtlCtl;
#endif 	/* __IAnkhUserControlHostCtlCtl_FWD_DEFINED__ */


#ifndef __AnkhUserControlHostCtl_FWD_DEFINED__
#define __AnkhUserControlHostCtl_FWD_DEFINED__

#ifdef __cplusplus
typedef class AnkhUserControlHostCtl AnkhUserControlHostCtl;
#else
typedef struct AnkhUserControlHostCtl AnkhUserControlHostCtl;
#endif /* __cplusplus */

#endif 	/* __AnkhUserControlHostCtl_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 

void * __RPC_USER MIDL_user_allocate(size_t);
void __RPC_USER MIDL_user_free( void * ); 

#ifndef __IAnkhUserControlHostCtlCtl_INTERFACE_DEFINED__
#define __IAnkhUserControlHostCtlCtl_INTERFACE_DEFINED__

/* interface IAnkhUserControlHostCtlCtl */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IAnkhUserControlHostCtlCtl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("412b14b5-b2fc-463b-a97d-ee072f939a38")
    IAnkhUserControlHostCtlCtl : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE HostUserControl( 
            IUnknown *userControl) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IAnkhUserControlHostCtlCtlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IAnkhUserControlHostCtlCtl * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IAnkhUserControlHostCtlCtl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IAnkhUserControlHostCtlCtl * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IAnkhUserControlHostCtlCtl * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IAnkhUserControlHostCtlCtl * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IAnkhUserControlHostCtlCtl * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IAnkhUserControlHostCtlCtl * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *HostUserControl )( 
            IAnkhUserControlHostCtlCtl * This,
            IUnknown *userControl);
        
        END_INTERFACE
    } IAnkhUserControlHostCtlCtlVtbl;

    interface IAnkhUserControlHostCtlCtl
    {
        CONST_VTBL struct IAnkhUserControlHostCtlCtlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IAnkhUserControlHostCtlCtl_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define IAnkhUserControlHostCtlCtl_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define IAnkhUserControlHostCtlCtl_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define IAnkhUserControlHostCtlCtl_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define IAnkhUserControlHostCtlCtl_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define IAnkhUserControlHostCtlCtl_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define IAnkhUserControlHostCtlCtl_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define IAnkhUserControlHostCtlCtl_HostUserControl(This,userControl)	\
    (This)->lpVtbl -> HostUserControl(This,userControl)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE IAnkhUserControlHostCtlCtl_HostUserControl_Proxy( 
    IAnkhUserControlHostCtlCtl * This,
    IUnknown *userControl);


void __RPC_STUB IAnkhUserControlHostCtlCtl_HostUserControl_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __IAnkhUserControlHostCtlCtl_INTERFACE_DEFINED__ */



#ifndef __AnkhUserControlHostLib_LIBRARY_DEFINED__
#define __AnkhUserControlHostLib_LIBRARY_DEFINED__

/* library AnkhUserControlHostLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_AnkhUserControlHostLib;

EXTERN_C const CLSID CLSID_AnkhUserControlHostCtl;

#ifdef __cplusplus

class DECLSPEC_UUID("88e212e9-dba6-4ca8-a820-05a9b54a3ba5")
AnkhUserControlHostCtl;
#endif
#endif /* __AnkhUserControlHostLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


