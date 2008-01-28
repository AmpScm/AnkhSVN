// $Id$
#pragma once
#include "stdafx.h"
#include <svn_types.h>

namespace NSvn
{
    namespace Core
    {
        public __gc class ChangedPath
        {
        public:
            ChangedPath( svn_log_changed_path_t* changedPath, apr_pool_t* pool ) :
              copyFromPath( Utf8ToString( changedPath->copyfrom_path, pool ) ),
                  copyFromRevision( changedPath->copyfrom_rev )
              {
                  switch( changedPath->action )
                  {
                  case 'A': this->action = ChangedPathAction::Add; break;
                  case 'D': this->action = ChangedPathAction::Delete; break;
                  case 'R': this->action = ChangedPathAction::Replace; break;
                  case 'M': this->action = ChangedPathAction::Modify; break;
                  }
              }

              /// <summary>Source path of copy(if any)</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property String* get_CopyFromPath()
              { return this->copyFromPath; }

              /// <summary>Source revision of copy(if any)</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property svn_revnum_t get_CopyFromRevision()
              { return this->copyFromRevision; }

              /// <summary>The action performed on the changed path</summary>
              [System::Diagnostics::DebuggerStepThrough]
              __property ChangedPathAction get_Action()
              { return this->action; }

        private:
            String* copyFromPath;
            svn_revnum_t copyFromRevision;
            ChangedPathAction action;
        };
    }
}

