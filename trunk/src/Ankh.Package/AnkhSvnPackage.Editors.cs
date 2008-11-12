using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Ids;
using System.ComponentModel.Design;
using Ankh.UI;

namespace Ankh.VSPackage
{
    [ProvideEditorFactoryAttribute(typeof(AnkhDiffEditorFactory), 302)]
    [ProvideEditorFactoryAttribute(typeof(AnkhDynamicEditorFactory), 304)]
    [ProvideEditorLogicalView(typeof(AnkhDiffEditorFactory), AnkhId.DiffEditorViewId)]
    partial class AnkhSvnPackage
    {
        void RegisterEditors()
        {
            RegisterEditorFactory(new AnkhDiffEditorFactory(this));

            AnkhDynamicEditorFactory def = new AnkhDynamicEditorFactory(this);

            RegisterEditorFactory(def);
            _runtime.GetService<IServiceContainer>().AddService(typeof(IAnkhDynamicEditorFactory), def);
        }
    }
}
