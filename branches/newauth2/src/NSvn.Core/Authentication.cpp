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
    GCPool* pool = new GCPool();
    svn_auth_provider_object_t* provider;

    svn_client_get_simple_provider( &provider, pool->ToAprPool() );

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