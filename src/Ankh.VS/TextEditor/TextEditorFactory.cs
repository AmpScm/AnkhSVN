using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI;
using Ankh.UI.VS.TextEditor;

namespace Ankh.VS.TextEditor
{
    [GlobalService(typeof(IVSTextEditorFactory))]
    class TextEditorFactory : AnkhService, IVSTextEditorFactory
    {
        public TextEditorFactory(IAnkhServiceProvider context)
            : base(context)
        {
        }

        #region IVSTextEditorFactory Members

        public bool TryInstantiateIn(VSTextEditor editor, out IVSTextEditorImplementation implementation)
        {
            TheVSTextEditor edit = new TheVSTextEditor();
            edit.Dock = System.Windows.Forms.DockStyle.Fill;

            implementation = edit;

            implementation.ForceLanguageService = editor.ForceLanguageService;
            implementation.WordWrap = editor.WordWrap;
            implementation.HideHorizontalScrollBar = editor.HideHorizontalScrollBar;
            implementation.EnableSplitter = editor.EnableSplitter;
            implementation.EnableNavigationBar = editor.EnableNavigationBar;
            implementation.ReadOnly = editor.ReadOnly;
            implementation.Text = editor.Text;

            editor.Controls.Add(edit);
            editor.ActiveControl = edit;

            return true;
        }

        #endregion
    }
}
