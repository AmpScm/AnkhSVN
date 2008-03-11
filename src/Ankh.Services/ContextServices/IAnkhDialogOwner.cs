using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.ContextServices
{
    public interface IAnkhDialogOwner
    {
        /// <summary>
        /// Gets the dialog owner.
        /// </summary>
        /// <value>The dialog owner.</value>
        IWin32Window DialogOwner { get; }
    }
}
