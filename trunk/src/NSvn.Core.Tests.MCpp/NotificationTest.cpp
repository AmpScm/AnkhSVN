// $Id$
#include "StdAfx.h"

#include "../NSvn.Core/Notification.h"
#include "NotificationTest.h"
#include "../NSvn.Core/svnenums.h"
#include <svn_wc.h>




void NSvn::Core::Tests::MCpp::NotificationTest::TestBasic()
{
    Notification* notification = new Notification( "C:/foo", 
        svn_wc_notify_add, svn_node_file, 
        "text/moo", svn_wc_notify_state_inapplicable, 
        svn_wc_notify_state_changed, 42 );

    Assertion::AssertEquals( notification->Path, S"C:/foo" );
    Assertion::AssertEquals( notification->Action, NotifyAction::Add );
    Assertion::AssertEquals( notification->NodeKind, NodeKind::File  );
    Assertion::AssertEquals( notification->MimeType, S"text/moo" );
    Assertion::AssertEquals( notification->ContentState, NotifyState::Inapplicable );
    Assertion::AssertEquals( notification->PropertyState, NotifyState::Changed );
    Assertion::AssertEquals( notification->RevisionNumber, 42 );



   
}


