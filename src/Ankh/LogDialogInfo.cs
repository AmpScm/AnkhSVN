// $Id$
using System;
using System.Collections;

namespace Ankh
{
    /// <summary>
    /// The parameters passed to the UIShell.LogDialog method.
    /// </summary>
    public class LogDialogInfo : PathSelectorInfo
    {
        public LogDialogInfo( IList items, IList checkedItems ) : 
            base( "", items, checkedItems )
        {			
        }

        public bool StopOnCopy
        {
            get{ return this.stopOnCopy; }
            set{ this.stopOnCopy = value; }
        }

        private bool stopOnCopy = false;
    }
}
