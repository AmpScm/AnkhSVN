using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Ankh.Ids;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.UI.Services;

namespace Ankh.UI
{
    [CLSCompliant(false)]
    public interface IAnkhToolWindowHost : IAnkhUISite
    {
        IVsWindowFrame Frame { get; }
        IVsWindowPane Pane { get; }

        /// <summary>
        /// Gets or sets the keyboard context of the frame window
        /// </summary>
        Guid KeyboardContext { get; set; }
        /// <summary>
        /// Gets or sets the command context of the frame window
        /// </summary>
        Guid CommandContext { get; set; }
    }
}
