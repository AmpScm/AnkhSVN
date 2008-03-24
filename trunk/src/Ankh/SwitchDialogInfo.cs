// $Id$
using System;
using System.Collections;

namespace Ankh
{
    /// <summary>
    /// Represents the info passed to the Switch dialog.
    /// </summary>
    public class SwitchDialogInfo : PathSelectorInfo
    {
        public SwitchDialogInfo( IList items, IList checkedItems ) :
            base( "", items, checkedItems )
        {
            this.switchToUrl = null;
            this.path = null;
        }

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

        private string path;
        private string switchToUrl;
    }
}
