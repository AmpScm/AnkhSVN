#include "ClientContext.h"
//#include <svn_client.h>
//#include <apr_pools.h>
//#include "Notification.h"
//#include "ManagedPointer.h"

//
//namespace
//{
//    using namespace NSvn::Core;
//    void notify_func( void *baton, const char *path, 
//        svn_wc_notify_action_t action, svn_node_kind_t kind, 
//        const char *mime_type, svn_wc_notify_state_t content_state, 
//        svn_wc_notify_state_t prop_state, svn_revnum_t revision )
//    {
//        Notification* notification = new Notification( path, action, kind,
//            mime_type, content_state, prop_state, revision );
//        NotifyCallback* callback = 
//            *(static_cast<ManagedPointer<NotifyCallback*>* >(baton) );
//        callback->Invoke( notification );
//
//    }
//
//
//}
//
//namespace NSvn{namespace Core
//{
//    svn_client_ctx_t* ClientContext::ToSvnContext( const Pool& pool )
//    {
//        svn_client_ctx_t* ctx = static_cast<svn_client_ctx_t*>(
//            apr_palloc( pool, sizeof( svn_client_ctx_t ) ) );
//
//        if ( this->NotifyCallback != 0 )
//        {
//            ctx->notify_func = notify_func;
//
//            ManagedPointer<NSvn::Core::NotifyCallback*> callback( this->NotifyCallback );
//            ctx->notify_baton = callback;
//        }
//
//        return ctx; 
//
//
//    }
//}}