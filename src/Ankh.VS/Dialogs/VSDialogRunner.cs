using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI;
using Microsoft.VisualStudio.Shell;
using Ankh.ContextServices;
using System.Windows.Forms;
using AnkhSvn.Ids;
using System.ComponentModel.Design;
using System.Security.Permissions;

namespace Ankh.VS.Dialogs
{
    class VSDialogRunner : IDialogRunner
    {
        readonly IAnkhServiceProvider _context;

        public VSDialogRunner(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        IAnkhDialogOwner _dlgOwner;
        /// <summary>
        /// Gets the dialog owner.
        /// </summary>
        /// <value>The dialog owner.</value>
        IAnkhDialogOwner DialogOwner
        {
            get { return _dlgOwner ?? (_dlgOwner = _context.GetService<IAnkhDialogOwner>()); }
        }


        #region IDialogRunner Members

        public DialogResult ShowDialog(System.Windows.Forms.Form form)
        {
            return ShowDialog(form, new DialogRunnerArgs());
        }

        public DialogResult ShowDialog(System.Windows.Forms.Form form, DialogRunnerArgs args)
        {
            if (form == null)
                throw new ArgumentNullException("form");
            else if (args == null)
                throw new ArgumentNullException("args");

            using (AnkhDialogContainerWithToolbar container = new AnkhDialogContainerWithToolbar(_context, form))
            using (new DialogButtonCloser(form, container))
            {
                if (args.ToolBar != AnkhCommandMenu.None)
                {
                    container.ToolbarID = new CommandID(AnkhId.CommandSetGuid, (int)args.ToolBar);
                    container.ToolbarLocation = args.ToolBarLocation;
                }

                IWin32Window owner = null;
                if (args.Owner != null)
                    owner = args.Owner;
                else if (DialogOwner != null)
                    owner = DialogOwner.DialogOwner;

                return container.ShowDialog(owner);
            }
        }

        #endregion

        #region DialogButtonCloser
        
        #endregion
    }
}
