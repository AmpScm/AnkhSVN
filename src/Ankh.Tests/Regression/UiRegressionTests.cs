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
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Ankh.UI.PathSelector;
using Ankh.UI.SccManagement;
using Ankh.UI.WizardFramework;
using NUnit.Framework;

namespace Ankh.Tests.Regression
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DialogLayoutRegressionTests
    {
        [Test]
        public void CommitDialogPendingListFillsItsPanelAndLogMessageResizes()
        {
            using (ProjectCommitDialog dialog = new ProjectCommitDialog())
            {
                Control pendingList = GetControl(dialog, "pendingList");
                Control logMessage = GetControl(dialog, "logMessage");

                Assert.AreEqual(DockStyle.Fill, pendingList.Dock);
                Assert.AreEqual(
                    AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    logMessage.Anchor);
            }
        }

        [Test]
        public void PendingChangeSelectorCannotRestoreSmallerThanDesignedLayout()
        {
            using (PendingChangeSelector dialog = new PendingChangeSelector())
            {
                Size minimum = dialog.MinimumSize;

                Assert.Greater(minimum.Width, 0);
                Assert.Greater(minimum.Height, 0);

                dialog.Size = new Size(Math.Max(1, minimum.Width - 100), Math.Max(1, minimum.Height - 100));

                Assert.GreaterOrEqual(dialog.Width, minimum.Width);
                Assert.GreaterOrEqual(dialog.Height, minimum.Height);
                AssertButtonIsInsideClientArea(dialog, "okButton");
                AssertButtonIsInsideClientArea(dialog, "cancelButton");
            }
        }

        [Test]
        public void WizardFrameworkDesignerResourcesUseRuntimeNamespace()
        {
            string expectedResourceName = typeof(Wizard).FullName + ".resources";

            CollectionAssert.Contains(
                typeof(Wizard).Assembly.GetManifestResourceNames(),
                expectedResourceName);

            using (TestWizard wizard = new TestWizard())
            {
                Assert.NotNull(wizard.PageContainer);
                Assert.Greater(wizard.ClientSize.Width, 0);
                Assert.Greater(wizard.ClientSize.Height, 0);
            }
        }

        sealed class TestWizard : Wizard
        {
        }

        static Control GetControl(Form form, string fieldName)
        {
            FieldInfo field = form.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field, "Expected designer field '{0}'", fieldName);

            Control control = field.GetValue(form) as Control;
            Assert.NotNull(control, "Expected '{0}' to be a WinForms control", fieldName);
            return control;
        }

        static void AssertButtonIsInsideClientArea(Form form, string fieldName)
        {
            Control button = GetControl(form, fieldName);
            Assert.IsTrue(form.ClientRectangle.Contains(button.Bounds),
                "{0} should remain inside the dialog client area", fieldName);
        }
    }

    [TestFixture]
    public class HistoryColorRegressionTests
    {
        [Test]
        public void HistoryAccentFallsBackOnDarkSurface()
        {
            System.Drawing.Color fallback = System.Drawing.Color.FromArgb(241, 241, 241);
            System.Drawing.Color background = System.Drawing.Color.FromArgb(30, 30, 30);

            Assert.AreEqual(
                fallback,
                Ankh.UI.AnkhThemePalette.ResolveReadableForeground(
                    System.Drawing.Color.DarkBlue,
                    fallback,
                    background));
        }

        [Test]
        public void HistoryAccentIsKeptWhenReadable()
        {
            System.Drawing.Color background = System.Drawing.Color.FromArgb(245, 245, 245);

            Assert.AreEqual(
                System.Drawing.Color.DarkBlue,
                Ankh.UI.AnkhThemePalette.ResolveReadableForeground(
                    System.Drawing.Color.DarkBlue,
                    System.Drawing.Color.Black,
                    background));
        }

        [Test]
        public void EmptyHistoryAccentUsesExplicitVsForeground()
        {
            System.Drawing.Color fallback = System.Drawing.Color.FromArgb(241, 241, 241);

            Assert.AreEqual(
                fallback,
                Ankh.UI.AnkhThemePalette.ResolveReadableForeground(
                    System.Drawing.Color.Empty,
                    fallback,
                    System.Drawing.Color.FromArgb(30, 30, 30)));
        }
    }

    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class FileIconMapperDpiRegressionTests
    {
        [Test]
        public void ImageListUsesDpiScaledLogicalIconSize()
        {
            object mapper = CreateMapper();
            try
            {
                ImageList images = GetImageList(mapper);
                // Shell small-icon metrics can differ from the DPI used by the
                // host window (notably in a DPI-unaware test process).
                FieldInfo dpiField = mapper.GetType().GetField(
                    "_imageDpi", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.NotNull(dpiField);
                int dpi = (int)dpiField.GetValue(mapper);
                Assert.Greater(dpi, 0);
                int pixels = (int)Math.Round(16 * dpi / 96.0, MidpointRounding.AwayFromZero);
                Assert.AreEqual(new Size(pixels, pixels), images.ImageSize);
            }
            finally
            {
                DisposeMapper(mapper);
            }
        }

        [Test]
        public void SpecialIconStripCanBeLoadedAtHighDpiSize()
        {
            object mapper = CreateMapper();
            try
            {
                ImageList images = GetImageList(mapper);
                images.ImageSize = new Size(32, 32);

                MethodInfo ensureImages = mapper.GetType().GetMethod(
                    "EnsureSpecialImages",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.NotNull(ensureImages);

                ensureImages.Invoke(mapper, null);

                // AddStrip validates that the scaled strip height matches ImageSize and
                // that its width is an exact multiple of the requested icon width.
                Assert.Greater(images.Images.Count, 0);
                Assert.AreEqual(new Size(32, 32), images.ImageSize);
            }
            finally
            {
                DisposeMapper(mapper);
            }
        }

        static object CreateMapper()
        {
            Assembly assembly = Assembly.Load("Ankh.VS");
            Type mapperType = assembly.GetType("Ankh.VS.SolutionExplorer.FileIconMapper", true);
            ConstructorInfo constructor = mapperType.GetConstructor(new[] { typeof(IAnkhServiceProvider) });
            Assert.NotNull(constructor);

            return constructor.Invoke(new object[] { new AnkhServiceContainer() });
        }

        static ImageList GetImageList(object mapper)
        {
            PropertyInfo property = mapper.GetType().GetProperty("ImageList", BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(property);

            ImageList images = property.GetValue(mapper, null) as ImageList;
            Assert.NotNull(images);
            return images;
        }

        static void DisposeMapper(object mapper)
        {
            IDisposable disposable = mapper as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
