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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using Ankh.Scc;
using Ankh.UI.Annotate;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using NUnit.Framework;
using SharpSvn;

namespace Ankh.Tests.Annotate
{
    /// <summary>
    /// Regression coverage for:
    /// https://github.com/AmpScm/AnkhSVN/issues/7
    /// https://github.com/AmpScm/AnkhSVN/issues/47
    ///
    /// Both issues exercised the legacy Annotate viewer architecture that embedded a
    /// Visual Studio editor inside a WinForms document control. The replacement uses
    /// Visual Studio's native IWpfTextView and a MEF-provided WPF annotation margin.
    ///
    /// These tests intentionally cover that wiring, not just the layout math, so a
    /// regression back to an unregistered/broken Annotate surface fails CI.
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class AnnotateViewerRegressionTests
    {
        const string RegressionContext =
            "Regression protection for AmpScm/AnkhSVN issues #7 and #47";

        [Test]
        public void Issues7And47_RegisteredDocumentCreatesWpfMarginAndUnregistersOnClose()
        {
            string fileName = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                "ankh-annotate-issues-7-47.cs");

            Uri repositoryRoot = new Uri("https://example.invalid/svn/");
            SvnOrigin origin = new SvnOrigin(
                new Uri(repositoryRoot, "trunk/ankh-annotate-issues-7-47.cs"),
                repositoryRoot);

            AnnotationDocumentRegistry.Register(
                fileName,
                new AnkhServiceContainer(),
                origin,
                new Collection<SvnBlameEventArgs>());

            Mock<ITextBuffer> buffer = new Mock<ITextBuffer>();

            Mock<ITextDataModel> dataModel = new Mock<ITextDataModel>();
            dataModel
                .SetupGet(x => x.DocumentBuffer)
                .Returns(buffer.Object);

            Mock<IEditorOptions> options = new Mock<IEditorOptions>();

            Mock<IWpfTextView> textView = new Mock<IWpfTextView>();
            textView
                .SetupGet(x => x.TextDataModel)
                .Returns(dataModel.Object);
            textView
                .SetupGet(x => x.Options)
                .Returns(options.Object);

            Mock<IWpfTextViewHost> host = new Mock<IWpfTextViewHost>();
            host
                .SetupGet(x => x.TextView)
                .Returns(textView.Object);

            Mock<ITextDocument> textDocument = new Mock<ITextDocument>();
            textDocument
                .SetupGet(x => x.FilePath)
                .Returns(fileName);

            Mock<ITextDocumentFactoryService> documentFactory =
                new Mock<ITextDocumentFactoryService>();

            ITextDocument resolvedDocument = textDocument.Object;
            documentFactory
                .Setup(x => x.TryGetTextDocument(buffer.Object, out resolvedDocument))
                .Returns(true);

            IWpfTextViewMarginProvider provider =
                CreateAnnotationMarginProvider(documentFactory.Object);

            IWpfTextViewMargin margin = provider.CreateMargin(host.Object, null);

            Assert.That(
                margin,
                Is.Not.Null,
                RegressionContext + ": the registered Annotate document must create the WPF margin.");

            Assert.That(
                margin.GetTextViewMargin("AnkhSVNAnnotationMargin"),
                Is.SameAs(margin),
                RegressionContext + ": the native editor must expose the AnkhSVN annotation margin.");

            Assert.That(
                margin.Enabled,
                Is.True,
                RegressionContext + ": the annotation margin must be enabled.");

            Assert.That(
                margin.MarginSize,
                Is.GreaterThan(0),
                RegressionContext + ": the annotation margin must have visible width.");

            // Closing the native VS text view is also the lifecycle boundary for the
            // registry entry. If this stops working, stale registrations can make a later
            // editor instance bind to the wrong Annotate state.
            textView.Raise(x => x.Closed += null, EventArgs.Empty);

            Assert.That(
                provider.CreateMargin(host.Object, null),
                Is.Null,
                RegressionContext + ": closing the editor must unregister the Annotate document.");
        }

        [Test]
        public void Issue47_WpfMarginRetainsRevisionContextMenuSelectionBridge()
        {
            Type marginType = typeof(AnnotationDocumentRegistry).Assembly.GetType(
                "Ankh.UI.Annotate.AnnotationMargin",
                true);

            Assert.That(marginType, Is.Not.Null, RegressionContext);

            BindingFlags declaredNonPublicInstance =
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            MethodInfo showContextMenu = marginType.GetMethods(declaredNonPublicInstance)
                .SingleOrDefault(method =>
                    method.Name == "ShowContextMenu" &&
                    method.GetParameters().Length == 1 &&
                    method.GetParameters()[0].ParameterType == typeof(System.Windows.Point));
            Assert.That(
                showContextMenu,
                Is.Not.Null,
                RegressionContext + ": the WPF annotation margin must retain a single deferred context-menu entry point.");

            MethodInfo previewRightClick = marginType.GetMethod(
                "OnPreviewMouseRightButtonDown",
                declaredNonPublicInstance);
            Assert.That(
                previewRightClick,
                Is.Not.Null,
                RegressionContext + ": Annotate must intercept right-click before the native editor context menu handles it.");

            MethodInfo previewRightClickRelease = marginType.GetMethod(
                "OnPreviewMouseRightButtonUp",
                declaredNonPublicInstance);
            Assert.That(
                previewRightClickRelease,
                Is.Not.Null,
                RegressionContext + ": Annotate must open its revision menu on right-button release, matching the legacy click timing.");

            MethodInfo suppressEditorContextMenu = marginType.GetMethod(
                "OnContextMenuOpening",
                declaredNonPublicInstance);
            Assert.That(
                suppressEditorContextMenu,
                Is.Not.Null,
                RegressionContext + ": Annotate must suppress the editor fallback context menu over the blame margin.");

            MethodInfo findRegionAt = marginType.GetMethod(
                "FindRegionAt",
                declaredNonPublicInstance);
            Assert.That(
                findRegionAt,
                Is.Not.Null,
                RegressionContext + ": the intercepted right-click must resolve the clicked blame region.");

            MethodInfo applyTheme = marginType.GetMethod(
                "ApplyTheme",
                declaredNonPublicInstance);
            Assert.That(
                applyTheme,
                Is.Not.Null,
                RegressionContext + ": the native Annotate margin must apply Visual Studio semantic theme colors.");

            MethodInfo themeChanged = marginType.GetMethod(
                "OnVsThemeChanged",
                declaredNonPublicInstance);
            Assert.That(
                themeChanged,
                Is.Not.Null,
                RegressionContext + ": the Annotate margin must refresh when Visual Studio changes theme.");

            MethodInfo scopedSelection = typeof(Ankh.Selection.ISelectionContextEx).GetMethod(
                "PushSelectionContainer",
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(
                scopedSelection,
                Is.Not.Null,
                RegressionContext + ": native Annotate context menus must be able to scope command selection to the clicked revision.");

            Type selectionContainerType = marginType.GetNestedType(
                "AnnotationSelectionContainer",
                BindingFlags.NonPublic);
            Assert.That(
                selectionContainerType,
                Is.Not.Null,
                RegressionContext + ": the WPF annotation margin must publish the clicked revision into VS selection.");

            Assert.That(
                typeof(ISelectionContainer).IsAssignableFrom(selectionContainerType),
                Is.True,
                RegressionContext + ": Annotate commands must receive the clicked revision through Visual Studio's selection container.");

            Assert.That(
                selectionContainerType.IsDefined(typeof(System.Runtime.InteropServices.ComVisibleAttribute), false),
                Is.True,
                RegressionContext + ": the annotation selection container must remain explicitly COM-visible for IVsTrackSelectionEx.");
        }

        [Test]
        public void Issues7And47_AnnotateMarginProviderRemainsMefExported()
        {
            Type providerType = GetAnnotationMarginProviderType();

            Assert.That(
                typeof(IWpfTextViewMarginProvider).IsAssignableFrom(providerType),
                Is.True,
                RegressionContext + ": Annotate must remain implemented as a native VS text-view margin.");

            string[] attributeNames = providerType
                .GetCustomAttributes(false)
                .Select(attribute => attribute.GetType().Name)
                .ToArray();

            CollectionAssert.Contains(
                attributeNames,
                "ExportAttribute",
                RegressionContext + ": the margin provider must remain a MEF export.");

            CollectionAssert.Contains(
                attributeNames,
                "NameAttribute",
                RegressionContext + ": the margin provider must retain its MEF name.");

            CollectionAssert.Contains(
                attributeNames,
                "MarginContainerAttribute",
                RegressionContext + ": the margin provider must remain attached to a VS editor margin container.");

            CollectionAssert.Contains(
                attributeNames,
                "ContentTypeAttribute",
                RegressionContext + ": the margin provider must remain associated with text content.");

            CollectionAssert.Contains(
                attributeNames,
                "TextViewRoleAttribute",
                RegressionContext + ": the margin provider must remain associated with interactive text views.");
        }

        static IWpfTextViewMarginProvider CreateAnnotationMarginProvider(
            ITextDocumentFactoryService documentFactory)
        {
            Type providerType = GetAnnotationMarginProviderType();

            object instance = Activator.CreateInstance(providerType, true);
            Assert.That(instance, Is.Not.Null, RegressionContext);

            PropertyInfo property = providerType.GetProperty(
                "TextDocumentFactoryService",
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null, RegressionContext);
            property.SetValue(instance, documentFactory, null);

            return (IWpfTextViewMarginProvider)instance;
        }

        static Type GetAnnotationMarginProviderType()
        {
            Type providerType = typeof(AnnotationDocumentRegistry).Assembly.GetType(
                "Ankh.UI.Annotate.AnnotationMarginProvider",
                true);

            Assert.That(providerType, Is.Not.Null, RegressionContext);
            return providerType;
        }
    }
}
