using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Ids;

namespace Ankh.VSPackage
{
    [ProvideEditorFactoryAttribute(typeof(AnkhDiffEditorFactory), 302)]
    [ProvideEditorFactoryAttribute(typeof(AnkhAnnotateEditorFactory), 303)]
    [ProvideEditorLogicalView(typeof(AnkhDiffEditorFactory), AnkhId.DiffEditorViewId)]
    [ProvideEditorLogicalView(typeof(AnkhAnnotateEditorFactory), AnkhId.AnnotateEditorViewId)]
    partial class AnkhSvnPackage
    {
        void RegisterEditors()
        {
            RegisterEditorFactory(new AnkhAnnotateEditorFactory(this));
            RegisterEditorFactory(new AnkhDiffEditorFactory(this));
        }
    }
}
