// $Id$
using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace Ankh
{
    /// <summary>
    /// The parameters passed to the UIShell.LogDialog method.
    /// </summary>
    public class LogDialogInfo : PathSelectorInfo
    {
        public LogDialogInfo(Collection<SvnItem> items)
            : base("", items)
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
