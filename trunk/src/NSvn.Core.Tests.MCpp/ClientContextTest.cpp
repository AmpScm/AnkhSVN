#include "Stdafx.h"
#include "ClientContextTest.h"

void NSvn::Core::Tests::MCpp::ClientContextTest::NotifyCallback( 
    Notification* notification )
{
    this->notification = notification;
}


void NSvn::Core::Tests::MCpp::ClientContextTest::TestSvnContextTConversion()
{
    Pool pool;

    //throw new Exception();

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
    Assertion::AssertEquals( this->notification->RevisionNumber, 43 );    
}
