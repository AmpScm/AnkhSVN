// $Id$
#include "Stdafx.h"
#include "ClientContextTest.h"
#include "SvnClientException.h"
#include "SimpleCredential.h"
#include "delegates.h"

// necessary since a .NET assembly does not export methods with native signatures
#include "ClientContext.cpp"
#include "SvnClientException.cpp"


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
public:
    DummyProvider() : Saved(false)
    {;}
    bool Saved;
    String* Realm;

    ICredential* FirstCredentials( String* realm, ICollection* params )
    {
        this->Realm = realm;
        return new SimpleCredential( "foo", "bar" );
    }

    ICredential* NextCredentials( ICollection* params )
    {
        return new SimpleCredential( "kung", "fu" );
    }

    bool SaveCredentials( ICollection* params )
    {
        this->Saved = true;
        return true;
    }


    String* get_Kind()
    {
        return SVN_AUTH_CRED_SIMPLE;
    }
};




struct svn_auth_iterstate_t
{};

void NSvn::Core::Tests::MCpp::ClientContextTest::TestEmptyAuthBaton()
{
    Pool pool;
    
    ClientContext* c = new ClientContext( 0 );
    svn_client_ctx_t* ctx = c->ToSvnContext( pool );

    svn_auth_cred_simple_t* cred;
    svn_auth_iterstate_t* iterState;

   

    HandleError( svn_auth_first_credentials( reinterpret_cast<void**>(&cred), &iterState, 
        SVN_AUTH_CRED_SIMPLE, "Realm of terror",
        ctx->auth_baton,  pool ) );
}


void NSvn::Core::Tests::MCpp::ClientContextTest::TestAuthBaton()
{
    Pool pool;

    DummyProvider* provider = new DummyProvider();
    AuthenticationBaton* bat = new AuthenticationBaton();
    bat->Providers->Add( provider );

    ClientContext* c = new ClientContext( 0, bat );

    svn_client_ctx_t* ctx = c->ToSvnContext( pool );
    svn_auth_cred_simple_t* cred;
    svn_auth_iterstate_t* iterState;

    // first the first
    svn_auth_first_credentials( reinterpret_cast<void**>(&cred), &iterState, SVN_AUTH_CRED_SIMPLE, 
        "Realm of terror", ctx->auth_baton,  pool );

    Assertion::Assert( S"Username not foo", strcmp( cred->username, "foo" ) == 0 );
    Assertion::Assert( S"Password not bar", strcmp( cred->password, "bar" ) == 0 );

    // next, the next
    svn_auth_next_credentials( reinterpret_cast<void**>(&cred), iterState, pool );

    Assertion::Assert( S"Username not kung", strcmp( cred->username, "kung" ) == 0 );
    Assertion::Assert( S"Password not fu", strcmp( cred->password, "fu" ) == 0 );

    // now try to save
    svn_auth_save_credentials( iterState, pool );
    Assertion::Assert( S"Credentials not saved", provider->Saved );

    Assertion::AssertEquals( "Wrong realm", S"Realm of terror", provider->Realm );

}

String* NSvn::Core::Tests::MCpp::ClientContextTest::LogMsgCallback( CommitItem* items[] )
{
    Assertion::AssertEquals( "Wrong number of items", 2, items->Length );
    Assertion::AssertEquals( "Wrong path", S"\\foo\\bar", items[0]->Path );
    Assertion::AssertEquals( "Wrong node kind", NodeKind::Directory, items[1]->Kind );
    Assertion::AssertEquals( "Wrong revision", 42, items[0]->Revision );
    Assertion::AssertEquals( "Wrong copy from url", S"http://copy.from.url", 
        items[1]->CopyFromUrl );
    Assertion::AssertEquals( "Wrong url", S"http://www.porn.com", items[0]->Url );

    return S"Hello world";
}

void NSvn::Core::Tests::MCpp::ClientContextTest::TestLogMessageCallback()
{
    Pool pool;
    ClientContext* c = new ClientContext( 0 );
    c->LogMessageCallback = new LogMessageCallback( this, &ClientContextTest::LogMsgCallback );

    svn_client_ctx_t* ctx = c->ToSvnContext( pool );

    apr_array_header_t* commitItems = apr_array_make( pool, 2, 
        sizeof( svn_client_commit_item_t* ) );

    // TODO: deal with wcprop_changes
    svn_client_commit_item_t item1 = { "/foo/bar", svn_node_file,  "http://www.porn.com",
        42, "http://copy.from.url", 42, 0 };
     svn_client_commit_item_t item2 = { "/kung/fu", svn_node_dir,  "http://www.42.com",
        42, "http://copy.from.url", 43, 0 };

     *((svn_client_commit_item_t**)apr_array_push( commitItems ) ) = &item1;
     *((svn_client_commit_item_t**)apr_array_push( commitItems ) ) = &item2;
    
    const char* logMsg;
    const char* tmpFile;
    ctx->log_msg_func( &logMsg, &tmpFile, commitItems, ctx->log_msg_baton, pool );

    // TODO: check encoding?
    Assertion::AssertEquals( "Log message wrong", S"Hello world", StringHelper( logMsg ) );
}





