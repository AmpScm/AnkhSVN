// $Id$
#include "Stdafx.h"
#include "ClientContextTest.h"

// necessary since a .NET assembly does not export methods with native signatures
#include "ClientContext.cpp"


void NSvn::Core::Tests::MCpp::ClientContextTest::NotifyCallback( 
    Notification* notification )
{
    this->notification = notification;
}


void NSvn::Core::Tests::MCpp::ClientContextTest::TestNotifyCallback()
{
    Pool pool;

    ClientContext* ctx = new ClientContext(
        new NSvn::Core::NotifyCallback( this, &ClientContextTest::NotifyCallback ) );
    svn_client_ctx_t* svnCtx = ctx->ToSvnContext( pool );
    svnCtx->notify_func( svnCtx->notify_baton, "Moo", svn_wc_notify_copy,
        svn_node_file, "text/moo", svn_wc_notify_state_unchanged, 
        svn_wc_notify_state_changed, 42 );

    Assertion::AssertEquals( this->notification->Path, S"Moo" );
    Assertion::AssertEquals( this->notification->Action, NotifyAction::Copy );
    Assertion::AssertEquals( this->notification->NodeKind, NodeKind::File );
    Assertion::AssertEquals( this->notification->ContentState, NotifyState::Unchanged );
    Assertion::AssertEquals( this->notification->PropertyState, NotifyState::Changed );
    Assertion::AssertEquals( this->notification->RevisionNumber, 42 );    
    
}

__gc class DummyProvider : public IAuthenticationProvider
{
    Credential* FirstCredentials()
    {
        return new Credential( "foo", "bar" );
    }

    Credential* NextCredentials()
    {
        return new Credential( "kung", "fu" );
    }
};

__gc class InvalidProvider : public IAuthenticationProvider
{
    Credential* FirstCredentials()
    {
        return Credential::Invalid;
    }

    Credential* NextCredentials()
    {
        return Credential::Invalid;
    }
};


struct svn_auth_iterstate_t
{};


void NSvn::Core::Tests::MCpp::ClientContextTest::TestAuthBaton()
{
    Pool pool;

    IAuthenticationProvider* provider = new DummyProvider();
    AuthenticationBaton* bat = new AuthenticationBaton();
    bat->Providers->Add( provider );

    ClientContext* c = new ClientContext( 0, bat );

    svn_client_ctx_t* ctx = c->ToSvnContext( pool );
    svn_auth_cred_simple_t* cred;
    svn_auth_iterstate_t* iterState;

    // first the first
    svn_auth_first_credentials( reinterpret_cast<void**>(&cred), &iterState, SVN_AUTH_CRED_SIMPLE, 
        ctx->auth_baton, pool );

    Assertion::Assert( S"Username not foo", strcmp( cred->username, "foo" ) == 0 );
    Assertion::Assert( S"Password not bar", strcmp( cred->password, "bar" ) == 0 );

    // next, the next
    svn_auth_next_credentials( reinterpret_cast<void**>(&cred), iterState, pool );

    Assertion::Assert( S"Username not kung", strcmp( cred->username, "kung" ) == 0 );
    Assertion::Assert( S"Password not fu", strcmp( cred->password, "fu" ) == 0 );


    // now try with the invalid credentials
    provider = new InvalidProvider();
    bat = new AuthenticationBaton();
    bat->Providers->Add( provider );

    c = new ClientContext( 0, bat );

    ctx = c->ToSvnContext( pool );

    // again the first
    svn_auth_first_credentials( reinterpret_cast<void**>(&cred), &iterState, SVN_AUTH_CRED_SIMPLE, 
        ctx->auth_baton,  pool );

    Assertion::Assert( "cred not NULL", cred == 0 );
}





