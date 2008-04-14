// $Id$
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ankh
{
    /// <summary>
    /// Represents the info passed to the Switch dialog.
    /// </summary>
    public class SwitchDialogInfo : PathSelectorInfo
    {
        public SwitchDialogInfo(ICollection<SvnItem> items)
            : base("", items)
        {
        }

        /*public SwitchDialogInfo( ICollection<SvnItem> items, Predicate<SvnItem> checkedFilter ) :
            base( "", items, checkedFilter )
        {
        }*/

        public string SwitchToUrl
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.switchToUrl; }

            [System.Diagnostics.DebuggerStepThrough]
            set{ this.switchToUrl = value; }
        }

        public string Path
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.path; }

            [System.Diagnostics.DebuggerStepThrough]
            set{ this.path = value; }
        }

        string path;
        string switchToUrl;
    }
}
