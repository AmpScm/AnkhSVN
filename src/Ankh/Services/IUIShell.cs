// $Id$
using System;
using Ankh.UI;
using System.Windows.Forms;

namespace Ankh
{
    /// <summary>
    /// Represents the UI of the addin.
    /// </summary>
    public interface IUIShell
    {
        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        DialogResult ShowMessageBox(string text, string caption,
            MessageBoxButtons buttons);

        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        DialogResult ShowMessageBox(string text, string caption,
            MessageBoxButtons buttons, MessageBoxIcon icon);

        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        DialogResult ShowMessageBox(string text, string caption,
            MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);

        /// <summary>
        /// Displays HTML in some suitable view.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="html"></param>
        void DisplayHtml(string caption, string html, bool reuse);

        /// <summary>
        /// Show a "path selector dialog".
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        PathSelectorResult ShowPathSelector(PathSelectorInfo info);
    }
}
