// $Id$
#pragma once
#include "stdafx.h"
#include "StringHelper.h"
#include "svnenums.h"

namespace NSvn
{
    namespace Core
    {
        using namespace System;

        public __gc class Notification
        {
        public:
            Notification( const char *path, svn_wc_notify_action_t action, 
                svn_node_kind_t kind, const char *mime_type, 
                svn_wc_notify_state_t content_state, 
                svn_wc_notify_state_t prop_state, svn_revnum_t revision )
            {
                this->path = StringHelper( path );
                this->action = static_cast<NSvn::Core::NotifyAction>(action);
                this->nodeKind = static_cast<NSvn::Core::NodeKind>(kind);
                this->mimeType = StringHelper( mime_type );
                this->contentState = static_cast<NSvn::Core::NotifyState>(content_state);
                this->propertyState = static_cast<NSvn::Core::NotifyState>(prop_state);
                this->revisionNumber = revision;
            }


            ///<summary>The path affected in this notification</summary>
            __property String* get_Path()
            { return this->path; }

            __property NotifyAction get_Action()
            { return this->action; }

            __property NodeKind get_NodeKind()
            { return this->nodeKind; }

            __property String* get_MimeType()
            { return this->mimeType; }

            __property NotifyState get_ContentState()
            { return this->contentState; }

            __property NotifyState get_PropertyState()
            { return this->propertyState; }

            __property int get_RevisionNumber()
            { return this->revisionNumber; }

        private:
            String* path;
            NSvn::Core::NotifyAction action;
            NSvn::Core::NodeKind nodeKind;
            String* mimeType;
            NSvn::Core::NotifyState contentState;
            NSvn::Core::NotifyState propertyState;
            int revisionNumber;

        };

    }
}

