using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.UI
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public interface IAnkhDynamicEditorFactory
    {
        /// <summary>
        /// Creates the editor.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        IVsWindowFrame CreateEditor(string fullPath, VSEditorControl form);
    }
}
