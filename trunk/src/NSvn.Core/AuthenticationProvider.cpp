// $Id$


#include <svn_client.h>
#include <svn_auth.h>

#include "credentials.h"
#include "AuthenticationProvider.h"
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

svn_error_t* svn_auth_ssl_client_cert_prompt_func( svn_auth_cred_ssl_client_cert_t **cred, 
                                                  void *baton, apr_pool_t *pool );

svn_error_t* svn_auth_ssl_client_cert_pw_prompt_func( svn_auth_cred_ssl_client_cert_pw_t **cred, 
                                                  void *baton, apr_pool_t *pool );



// implementation of GetSimplePromptProvider
AuthenticationProvider* NSvn::Core::AuthenticationProvider::GetSimplePromptProvider(
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

    return new AuthenticationProvider( provider, pool );
}


// implementation of GetUsernameProvider
AuthenticationProvider* NSvn::Core::AuthenticationProvider::GetUsernameProvider()
{
    GCPool* pool = new GCPool();
    svn_auth_provider_object_t* provider;

    svn_client_get_username_provider( &provider, pool->ToAprPool() );

    return new AuthenticationProvider( provider, pool );
}

// implementation of GetSimpleProvider
AuthenticationProvider* NSvn::Core::AuthenticationProvider::GetSimpleProvider()
{
    // TODO: refactor this
    GCPool* pool = new GCPool();
    svn_auth_provider_object_t* provider;

    svn_client_get_simple_provider( &provider, pool->ToAprPool() );

    return new AuthenticationProvider( provider, pool );
}

// implementation of GetSslServerTrustPromptProvider
AuthenticationProvider* NSvn::Core::AuthenticationProvider::GetSslServerTrustPromptProvider(
    SslServerTrustPromptDelegate* promptDelegate )
{
    GCPool* pool = new GCPool();
    svn_auth_provider_object_t* provider;

    void* baton = pool->GetPool()->AllocateObject( 
        ManagedPointer<SslServerTrustPromptDelegate*>(promptDelegate));

    svn_client_get_ssl_server_trust_prompt_provider( &provider, 
        svn_auth_ssl_server_trust_prompt_func, baton, pool->ToAprPool() );

    return new AuthenticationProvider( provider, pool );
}

// implementation of GetSslServerTrustFileProvider
AuthenticationProvider* NSvn::Core::AuthenticationProvider::GetSslServerTrustFileProvider()
{
    GCPool* pool = new GCPool();
    svn_auth_provider_object_t* provider;

    svn_client_get_ssl_server_trust_file_provider( &provider, 
        pool->ToAprPool() );

    return new AuthenticationProvider( provider, pool );
}

// implementation of GetSslServerTrustFileProvider
AuthenticationProvider* NSvn::Core::AuthenticationProvider::GetSslClientCertFileProvider()
{
    GCPool* pool = new GCPool();
    svn_auth_provider_object_t* provider;

    svn_client_get_ssl_client_cert_file_provider( &provider, 
        pool->ToAprPool() );

    return new AuthenticationProvider( provider, pool );
}

// implementation of GetSslServerTrustFileProvider
AuthenticationProvider* NSvn::Core::AuthenticationProvider::GetSslClientCertPasswordFileProvider()
{
    GCPool* pool = new GCPool();
    svn_auth_provider_object_t* provider;

    svn_client_get_ssl_client_cert_pw_file_provider( &provider, 
        pool->ToAprPool() );

    return new AuthenticationProvider( provider, pool );
}

// implementation of GetSslServerTrustFileProvider
AuthenticationProvider* NSvn::Core::AuthenticationProvider::GetSslClientCertPromptProvider(
    SslClientCertPromptDelegate* promptDelegate )
{
    GCPool* pool = new GCPool();
    svn_auth_provider_object_t* provider;

    void* baton = pool->GetPool()->AllocateObject( 
        ManagedPointer<SslClientCertPromptDelegate*>(promptDelegate));

    svn_client_get_ssl_client_cert_prompt_provider( &provider, 
        svn_auth_ssl_client_cert_prompt_func, baton, 
        pool->ToAprPool() );

    return new AuthenticationProvider( provider, pool );
}

// implementation of GetSslServerTrustFileProvider
AuthenticationProvider* NSvn::Core::AuthenticationProvider::GetSslClientCertPasswordPromptProvider(
    SslClientCertPasswordPromptDelegate* promptDelegate )
{
    GCPool* pool = new GCPool();
    svn_auth_provider_object_t* provider;

    void* baton = pool->GetPool()->AllocateObject( 
        ManagedPointer<SslClientCertPasswordPromptDelegate*>(promptDelegate));

    svn_client_get_ssl_client_cert_pw_prompt_provider( &provider, 
        svn_auth_ssl_client_cert_pw_prompt_func, baton, 
        pool->ToAprPool() );

    return new AuthenticationProvider( provider, pool );
}

// callback function for a simple prompt provider
svn_error_t* simple_prompt_func( svn_auth_cred_simple_t** cred, void* baton,
                                const char* realm, const char* username, apr_pool_t* pool )
{
    // get hold of the delegate and call it
    SimplePromptDelegate* delegate = *(static_cast<ManagedPointer<SimplePromptDelegate*>* >(
        baton ));
    String* realmString = (realm != 0) ? static_cast<String*>(StringHelper(realm)) : 0;
    String* usernameString = (username != 0) ? static_cast<String*>(StringHelper(username)) : 0;

    SimpleCredential* simpleCred = delegate->Invoke( realmString, usernameString );

    if ( simpleCred != 0 )
        *cred = simpleCred->GetCredential( pool );
    else 
        *cred = 0;

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

svn_error_t* svn_auth_ssl_client_cert_prompt_func( svn_auth_cred_ssl_client_cert_t **cred_p, 
                                                  void *baton, apr_pool_t *pool )
{
    SslClientCertPromptDelegate* delegate = *(static_cast<ManagedPointer<
        SslClientCertPromptDelegate*>*>(baton));

    // invoke the managed callback
    SslClientCertificateCredential* cred = delegate->Invoke();

    // null?
    if ( cred != 0 )
        *cred_p = cred->GetCredential( pool );
    else
        *cred_p = 0;

    return SVN_NO_ERROR;
}

svn_error_t* svn_auth_ssl_client_cert_pw_prompt_func( svn_auth_cred_ssl_client_cert_pw_t **cred_p, 
                                                  void *baton, apr_pool_t *pool )
{
    SslClientCertPasswordPromptDelegate* delegate = *(static_cast<ManagedPointer<
        SslClientCertPasswordPromptDelegate*>*>(baton));

    // invoke the managed callback
    SslClientCertificatePasswordCredential* cred = delegate->Invoke();

    // null?
    if ( cred != 0 )
        *cred_p = cred->GetCredential( pool );
    else
        *cred_p = 0;

    return SVN_NO_ERROR;
}