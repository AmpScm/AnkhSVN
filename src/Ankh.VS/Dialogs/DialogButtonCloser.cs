using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Ankh.VS.Dialogs
{
    /// <summary>
    /// The DialogContainerWithToolbar forgets to close the dialog on buttons; we resolve that ourselves here
    /// </summary>
    sealed class DialogButtonCloser : IDisposable, IMessageFilter
    {
        readonly Form _form;
        readonly Form _container;
        public DialogButtonCloser(Form form, Form container)
        {
            if (form == null)
                throw new ArgumentNullException("form");
            else if (container == null)
                throw new ArgumentNullException("container");

            _form = form;
            _container = container;
            Application.AddMessageFilter(this);

        }

        /// <summary>
        /// Filters out a message before it is dispatched.
        /// </summary>
        /// <param name="m">The message to be dispatched. You cannot modify this message.</param>
        /// <returns>
        /// true to filter the message and stop it from being dispatched; false to allow the message to continue to the next filter or control.
        /// </returns>
        [EnvironmentPermission(SecurityAction.LinkDemand)]
        public bool PreFilterMessage(ref Message m)
        {
            if ((_form.DialogResult != DialogResult.None) && (_container.DialogResult == DialogResult.None))
            {
                _container.DialogResult = _form.DialogResult;
            }

            return false;
        }

        public void Dispose()
        {
            Application.RemoveMessageFilter(this);
        }
    }
}
