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
            /// <summary>Node doesn't exist.</summary>     
            None = svn_node_none, 
            /// <summary>Node is a file.</summary>          
            File = svn_node_file, 
            /// <summary>Node is a directory.</summary>                 
            Directory = svn_node_dir, 
            /// <summary>Node is unknown.</summary>                
            Unknown = svn_node_unknown 
        };
        ///TODO: Review comments.
        /// <summary>The type of action occuring. </summary> 
         public __value enum NotifyAction
        {
            /// <summary>Got an add.</summary>         
            Add = svn_wc_notify_add,
            /// <summary>Got a copy.</summary>          
            Copy = svn_wc_notify_copy,
            /// <summary>Got a delete.</summary>         
            Delete = svn_wc_notify_delete,
            /// <summary>Got a restore.</summary>          
            Restore = svn_wc_notify_restore,
            /// <summary>Got a revert.</summary>               
            Revert = svn_wc_notify_revert,
            /// <summary>Got a revert failed.</summary>          
            FailedRevert = svn_wc_notify_failed_revert,
            /// <summary>Got a resolve.</summary>          
            Resolve = svn_wc_notify_resolve,
            /// <summary>Got a status.</summary>              
            Status = svn_wc_notify_status,
            /// <summary>Got a skip.</summary>          
            Skip = svn_wc_notify_skip,
            /// <summary>Got a delete.</summary>          
            UpdateDelete = svn_wc_notify_update_delete,
            /// <summary>Got an add in an update.</summary>          
            UpdateAdd = svn_wc_notify_update_add,
            /// <summary>Got any other 
            ///          action in an update.</summary>          
            UpdateUpdate = svn_wc_notify_update_update,
            /// <summary>Got an update. </summary>          
            UpdateCompleted = svn_wc_notify_update_completed,
            /// <summary>About to update an external module, use for
            ///                                checkouts and switches too,
            ///                                end with @c svn_wc_update_completed.</summary>          
            UpdateExternal = svn_wc_notify_update_external,
            /// <summary>Got a commit modified.</summary>          
            CommitModified = svn_wc_notify_commit_modified,
            /// <summary>Got a commit added.</summary>              
            CommitAdded = svn_wc_notify_commit_added,
            /// <summary>Got a commit deleted.</summary>               
            CommitDeleted = svn_wc_notify_commit_deleted,
            /// <summary>Got a commit replaced.</summary>              
            CommitReplaced = svn_wc_notify_commit_replaced,
            ///<summary>Delta was sent to the repository.</summary>              
            CommitPostfixTxDelta = svn_wc_notify_commit_postfix_txdelta
        };
        
         ///<summary>The type of notification that is occuring. </summary>
         public __value enum NotifyState
        {
            ///<summary>Modified state in working copy's item is inapplicable (not usable).</summary>    
            Inapplicable = svn_wc_notify_state_inapplicable, 
            ///<summary>Notifier doesn't know or isn't saying.</summary>                 
            Unknown = svn_wc_notify_state_unknown, 
            ///<summary>The state did not change.</summary>                
            Unchanged = svn_wc_notify_state_unchanged, 
            ///<summary>Pristine state was modified.</summary>               
            Changed = svn_wc_notify_state_changed, 
            ///<summary>Modified state had mods merged in.</summary>          
            Merged = svn_wc_notify_state_merged, 
            ///<summary>Modified state got conflicting mods.</summary>                
            Conflicted = svn_wc_notify_state_conflicted 
        };
    }
}
