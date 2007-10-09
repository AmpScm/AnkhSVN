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
    svn_client_ctx_t* svnCtx = ctx->ToSvnContext();
    svn_wc_notify_t* notify = svn_wc_create_notify( "Moo", svn_wc_notify_copy, pool );
    notify->kind = svn_node_file;
    notify->content_state = svn_wc_notify_state_unchanged;
    notify->revision = 42;
    notify->prop_state = svn_wc_notify_state_changed;
    void* clientBaton = pool.AllocateObject(
                ManagedPointer<NSvn::Core::Client*>(
                this) );

    svnCtx->notify_func2( clientBaton, notify, pool );

    Assert::AreEqual( this->notification->Path, S"Moo" );
    Assert::AreEqual( this->notification->Action, NotifyAction::Copy );
    Assert::AreEqual( this->notification->NodeKind, NodeKind::File );
    Assert::AreEqual( this->notification->ContentState, NotifyState::Unchanged );
    Assert::AreEqual( this->notification->PropertyState, NotifyState::Changed );
    Assert::AreEqual( this->notification->RevisionNumber, 42 );    

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
    Assert::AreEqual( 2, items->Length, "Wrong number of items" );
    Assert::AreEqual( S"\\foo\\bar", items[0]->Path, "Wrong path" );
    Assert::AreEqual( NodeKind::Directory, items[1]->Kind, "Wrong node kind" );
    Assert::AreEqual( 42, items[0]->Revision, "Wrong revision" );
    Assert::AreEqual( S"http://copy.from.url", items[1]->CopyFromUrl,
        "Wrong copy from url" );
    Assert::AreEqual( S"http://www.porn.com", items[0]->Url, "Wrong url" );

    args->Message = S"Hello world";
}

void NSvn::Core::Tests::MCpp::ClientContextTest::TestLogMessage()
{
    Pool pool;
    ClientContext* c = new ClientContext( this );


    svn_client_ctx_t* ctx = c->ToSvnContext();

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
    Assert::AreEqual( S"Hello world", Utf8ToString( logMsg, pool ), "Log message wrong" );
}

void NSvn::Core::Tests::MCpp::ClientContextTest::OnCancel( CancelEventArgs* args )
{
    args->Cancel = true;
}

void NSvn::Core::Tests::MCpp::ClientContextTest::TestCancel()
{
    Pool pool;
    ClientContext* c = new ClientContext( this );

    svn_client_ctx_t* ctx = c->ToSvnContext();

    svn_error_t* err = ctx->cancel_func( ctx->cancel_baton );
    Assert::IsTrue( err->apr_err == SVN_ERR_CANCELLED, "Not cancelled" );
}





