// $Id$
#pragma once
#using <mscorlib.dll>
#include <svn_types.h>
#include <svn_wc.h>
#include <svn_opt.h>

namespace NSvn
{
    namespace Core
    {
        public __value enum NodeKind
        { 
            None = svn_node_none, 
            File = svn_node_file, 
            Directory = svn_node_dir, 
            Unknown = svn_node_unknown 
         
        };

        public __value enum NotifyAction
        {
            Add = svn_wc_notify_add,
            Copy = svn_wc_notify_copy,
            Delete = svn_wc_notify_delete,
            Restore = svn_wc_notify_restore,
            Revert = svn_wc_notify_revert,
            FailedRevert = svn_wc_notify_failed_revert,
            Resolve = svn_wc_notify_resolve,
            Status = svn_wc_notify_status,
            Skip = svn_wc_notify_skip,
            UpdateDelete = svn_wc_notify_update_delete,

            /** Got an add in an update. */
            UpdateAdd = svn_wc_notify_update_add,

            /** Got any other action in an update. */
            UpdateUpdate = svn_wc_notify_update_update,

            /** The last notification in an update */
            UpdateCompleted = svn_wc_notify_update_completed,

            /** About to update an external module, use for checkouts and switches too,
            * end with @c svn_wc_update_completed.
            */
            UpdateExternal = svn_wc_notify_update_external,

            CommitModified = svn_wc_notify_commit_modified,
            CommitAdded = svn_wc_notify_commit_added,
            CommitDeleted = svn_wc_notify_commit_deleted,
            CommitReplaced = svn_wc_notify_commit_replaced,
            CommitPostfixTxDelta = svn_wc_notify_commit_postfix_txdelta
        };

        public __value enum NotifyState
        {
            Inapplicable = svn_wc_notify_state_inapplicable, 
            Unknown = svn_wc_notify_state_unknown, 
            Unchanged = svn_wc_notify_state_unchanged, 
            Changed = svn_wc_notify_state_changed, 
            Merged = svn_wc_notify_state_merged, 
            Conflicted = svn_wc_notify_state_conflicted 
        };

        public __value enum RevisionKind
        {
            Unspecified = svn_opt_revision_unspecified,  
            Number = svn_opt_revision_number,
            Date = svn_opt_revision_date,
            Committed = svn_opt_revision_committed,
            Previous = svn_opt_revision_previous,
            Base = svn_opt_revision_base,
            Current = svn_opt_revision_working,
            Head = svn_opt_revision_head  
        };
    }
}
