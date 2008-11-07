using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.UI;

namespace Ankh.VS.Dialogs
{
    [GlobalService(typeof(IAnkhDialogOwner))]
    sealed class AnkhDialogOwner : AnkhService, IAnkhDialogOwner
    {
        IVsUIShell _shell;
        IUIService _uiService;

        public AnkhDialogOwner(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IVsUIShell Shell
        {
            get { return _shell ?? (_shell = GetService <IVsUIShell>(typeof(SVsUIShell))); }
        }

        IUIService UIService
        {
            get { return _uiService ?? (_uiService = GetService<IUIService>()); }
        }

        #region IAnkhDialogOwner Members

        public IWin32Window DialogOwner
        {
            get 
            {
                if (UIService != null)
                    return UIService.GetDialogOwnerWindow();
                else
                    return null;
            }
        }

        #region IAnkhDialogOwner Members

        public IDisposable InstallFormRouting(Ankh.UI.VSContainerForm container, EventArgs eventArgs)
        {
            return new VSCommandRouting(Context, container);            
        }

        public void OnContainerCreated(Ankh.UI.VSContainerForm form)
        {
            VSCommandRouting routing = VSCommandRouting.FromForm(form);

            if (routing != null)
                routing.OnHandleCreated();
        }
        #endregion

        #endregion

        #region IAnkhDialogOwner Members

        public AnkhMessageBox MessageBox
        {
            get { return new AnkhMessageBox(this); }
        }

        #endregion

        #region IAnkhDialogOwner Members


        public void AddCommandTarget(Ankh.UI.VSContainerForm form, IOleCommandTarget commandTarget)
        {
            VSCommandRouting routing = VSCommandRouting.FromForm(form);

            if (routing != null)
                routing.AddCommandTarget(commandTarget);
            else
                throw new InvalidOperationException("Command routing not initialized yet");
        }

        public void AddWindowPane(Ankh.UI.VSContainerForm form, IVsWindowPane pane)
        {
            VSCommandRouting routing = VSCommandRouting.FromForm(form);

            if (routing != null)
                routing.AddWindowPane(pane);
            else
                throw new InvalidOperationException("Command routing not initialized yet");
        }

        #endregion

        #region IAnkhDialogOwner Members


        public void CreateDocumentForm(VSDocumentForm form)
        {
            VSDocumentFormPane pane = new VSDocumentFormPane(this, form);

            pane.Show();
            /*pane./
            throw new NotImplementedException();*/
        }

        #endregion
    }
}
