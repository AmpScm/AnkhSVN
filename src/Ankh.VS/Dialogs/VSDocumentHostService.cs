using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS.Dialogs
{
    [GlobalService(typeof(IAnkhDocumentHostService))]
    class VSDocumentHostService : AnkhService, IAnkhDocumentHostService
    {
        public VSDocumentHostService(IAnkhServiceProvider context)
            : base(context)
        {
        }
        public void ProvideEditor(VSEditorControl form, Guid factoryId, out object doc, out object pane)
        {
            VSDocumentInstance dc = new VSDocumentInstance(Context, factoryId);
            pane = new VSDocumentFormPane(Context, dc, form);

            doc = dc;
        }

        #region IAnkhDocumentHostService Members

        public void InitializeEditor(VSEditorControl form, IVsUIHierarchy hier, IVsWindowFrame frame, uint docid)
        {
            VSDocumentFormPane pane = null;
            object value;
            if (ErrorHandler.Succeeded(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out value)))
            {
                pane = value as VSDocumentFormPane;
            }


            if (pane != null)
                ((IVSEditorControlInit)form).InitializedForm(hier, docid, frame, pane.Host);
        }

        #endregion
    }
}
