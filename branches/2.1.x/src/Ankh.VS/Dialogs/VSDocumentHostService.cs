// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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
