// $Id$


#include <svn_client.h>
#include <svn_auth.h>

#include "Authentication.h"
#include "credentials.h"
#include "AuthenticationProviderObject.h"
#include "ManagedPointer.h"
#include "GCPool.h"

using namespace NSvn::Core;

// forward declarations of callbacks
svn_error_t* simple_prompt_func( svn_auth_cred_simple_t** cred, void* baton, 
                                const char* realm, const char* username, apr_pool_t* pool );

svn_error_t* svn_auth_ssl_server_trust_prompt_func( 
    svn_auth_cred_ssl_server_trust_t **cred_p,
    void *baton,
    const char *realm,
    int failures,
    const svn_auth_ssl_server_cert_info_t *cert_info,
    apr_pool_t *pool );



// implementation of GetSimplePromptProvider
AuthenticationProviderObject* NSvn::Core::Authentication::GetSimplePromptProvider(
    SimplePromptDelegate* promptDelegate, int retryLimit )
{
    // We need to use GCPool here instead of plain pool because the 
    // provider created outlives this method
    GCPool* pool = new GCPool();
    
    void* baton = pool->GetPool()->AllocateObject( 
        ManagedPointer<SimplePromptDelegate*>(promptDelegate) );

    svn_auth_provider_object_t* provider;

    svn_client_get_simple_prompt_provider( &provider, simple_prompt_func, baton,
        retryLimit, pool->ToAprPool() );

    return new AuthenticationProviderObject( provider, pool );
}


// implementation of GetUsernameProvider
AuthenticationProviderObject* NSvn::Core::Authentication::GetUsernameProvider()
{
    GCPool* pool = new GCPool();
    svn_auth_provider_object_t* provider;

    svn_client_get_username_provider( &provider, pool->ToAprPool() );

    return new AuthenticationProviderObject( provider, pool );
}

// implementation of GetSimpleProvider
AuthenticationProviderObject* NSvn::Core::Authentication::GetSimpleProvider()
{
    // TODO: refactor this
    GCPool* pool = new GCPool();
    svn_auth_provider_object_t* provider;

    svn_client_get_simple_provider( &provider, pool->ToAprPool() );

    return new AuthenticationProviderObject( provider, pool );
}

// implementation of GetSslServerTrustPromptProvider
AuthenticationProviderObject* NSvn::Core::Authentication::GetSslServerTrustPromptProvider(
    SslServerTrustPromptDelegate* promptDelegate )
{
    GCPool* pool = new GCPool();
    svn_auth_provider_object_t* provider;

    void* baton = pool->GetPool()->AllocateObject( 
        ManagedPointer<SslServerTrustPromptDelegate*>(promptDelegate));

    svn_client_get_ssl_server_trust_prompt_provider( &provider, 
        svn_auth_ssl_server_trust_prompt_func, baton, pool->ToAprPool() );

    return new AuthenticationProviderObject( provider, pool );
}


// callback function for a simple prompt provider
svn_error_t* simple_prompt_func( svn_auth_cred_simple_t** cred, void* baton,
                                const char* realm, const char* username, apr_pool_t* pool )
{
    // get hold of the delegate and call it
    SimplePromptDelegate* delegate = *(static_cast<ManagedPointer<SimplePromptDelegate*>* >(
        baton ));
    SimpleCredential* simpleCred = delegate->Invoke( StringHelper(realm), StringHelper(username) );

    *cred = simpleCred->GetCredential( pool );

    return SVN_NO_ERROR;
}

// callback function for the ssl server trust prompt provider
svn_error_t* svn_auth_ssl_server_trust_prompt_func( 
    svn_auth_cred_ssl_server_trust_t **cred_p,
    void *baton,
    const char *realm,
    int failures,
    const svn_auth_ssl_server_cert_info_t *cert_info,
    apr_pool_t *pool )
{
    SslServerTrustPromptDelegate* delegate = *(static_cast<ManagedPointer<
        SslServerTrustPromptDelegate*>*>(baton));

    // invoke the managed callback
    String* realmString = StringHelper(realm);
    SslServerTrustCredential* cred = delegate->Invoke( realmString, 
        static_cast<SslFailures>(failures), new SslServerCertificateInfo(cert_info) );

    // null?
    if ( cred != 0 )
        *cred_p = cred->GetCredential( pool );
    else
        *cred_p = 0;

    return SVN_NO_ERROR;
}