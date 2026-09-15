// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Ankh.UI.Annotate
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(AnnotationMargin.MarginName)]
    [Order(After = PredefinedMarginNames.Glyph)]
    [MarginContainer(PredefinedMarginNames.Left)]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class AnnotationMarginProvider : IWpfTextViewMarginProvider
    {
        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            if (wpfTextViewHost == null || TextDocumentFactoryService == null)
                return null;

            ITextDocument textDocument;
            if (!TextDocumentFactoryService.TryGetTextDocument(
                    wpfTextViewHost.TextView.TextDataModel.DocumentBuffer,
                    out textDocument))
            {
                return null;
            }

            AnnotationDocument annotationDocument;
            if (!AnnotationDocumentRegistry.TryGet(textDocument.FilePath, out annotationDocument))
                return null;

            wpfTextViewHost.TextView.Options.SetOptionValue(
                DefaultTextViewOptions.ViewProhibitUserInputId,
                true);

            return new AnnotationMargin(wpfTextViewHost.TextView, textDocument.FilePath, annotationDocument);
        }
    }
}
