// $Id$
#include "Stdafx.h"

#include "ClientContextTest.h"
#include "SvnClientException.h"
#include "delegates.h"
#include "CommitItem.h"
#include "NotificationEventArgs.h"
#include "LogMessageEventArgs.h"
#include "CancelEventArgs.h"


#using <NSvn.Common.dll>

using namespace NSvn::Common;
using namespace NSvn::Core;
using namespace System::Collections;



void NSvn::Core::Tests::MCpp::ClientContextTest::OnNotification( 
    NotificationEventArgs* notification )
{
    this->notification = notification;
}


void NSvn::Core::Tests::MCpp::ClientContextTest::TestNotification()
{
    Pool pool;

    ClientContext* ctx = new ClientContext( this );
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

/*__gc class DummyProvider : public IAuthenticationProvider
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
};*/




struct svn_auth_iterstate_t
{};



void NSvn::Core::Tests::MCpp::ClientContextTest::OnLogMessage( LogMessageEventArgs* args )
{
    CommitItem* items[] = args->CommitItems;
    Assertion::AssertEquals( "Wrong number of items", 2, items->Length );
    Assertion::AssertEquals( "Wrong path", S"\\foo\\bar", items[0]->Path );
    Assertion::AssertEquals( "Wrong node kind", NodeKind::Directory, items[1]->Kind );
    Assertion::AssertEquals( "Wrong revision", 42, items[0]->Revision );
    Assertion::AssertEquals( "Wrong copy from url", S"http://copy.from.url", 
        items[1]->CopyFromUrl );
    Assertion::AssertEquals( "Wrong url", S"http://www.porn.com", items[0]->Url );

    args->Message = S"Hello world";
}

void NSvn::Core::Tests::MCpp::ClientContextTest::TestLogMessage()
{
    Pool pool;
    ClientContext* c = new ClientContext( this );


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

void NSvn::Core::Tests::MCpp::ClientContextTest::OnCancel( CancelEventArgs* args )
{
    args->Cancel = true;
}

void NSvn::Core::Tests::MCpp::ClientContextTest::TestCancel()
{
    Pool pool;
    ClientContext* c = new ClientContext( this );

    svn_client_ctx_t* ctx = c->ToSvnContext( pool );

    svn_error_t* err = ctx->cancel_func( ctx->cancel_baton );
    Assertion::Assert( "Not cancelled", err->apr_err == SVN_ERR_CANCELLED );
}





