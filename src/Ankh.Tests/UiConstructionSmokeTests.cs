using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    public class UiConstructionSmokeTests
    {
        private static readonly string[] UiTypes =
        {
            "Ankh.UI.PropertyEditors.EolStylePropertyEditor",
            "Ankh.UI.PropertyEditors.ExecutablePropertyEditor",
            "Ankh.UI.PropertyEditors.ExternalsPropertyEditor",
            "Ankh.UI.PropertyEditors.IgnorePropertyEditor",
            "Ankh.UI.PropertyEditors.KeywordsPropertyEditor",
            "Ankh.UI.PropertyEditors.MimeTypePropertyEditor",
            "Ankh.UI.PropertyEditors.NeedsLockPropertyEditor",
            "Ankh.UI.PropertyEditors.PlainPropertyEditor",
            "Ankh.UI.OptionsPages.EnvironmentSettingsControl",
            "Ankh.UI.OptionsPages.AdvancedDiffUserToolSettingsControl",
            "Ankh.UI.OptionsPages.AdvancedMergeUserToolSettingsControl",
            "Ankh.UI.OptionsPages.UserToolSettingsControl",
            "Ankh.UI.OptionsPages.SvnProxyEditor",
            "Ankh.UI.Commands.CreateChangeListDialog",
            "Ankh.UI.PathSelector.DateSelector",
            "Ankh.UI.PathSelector.RevisionSelector",
            "Ankh.UI.PathSelector.VersionSelector"
        };

        [TestCaseSource(nameof(UiTypes))]
        public void WinFormsComponent_ConstructsAndDisposesOnStaThread(string typeName)
        {
            Exception failure = null;
            Control control = null;

            Thread thread = new Thread(() =>
            {
                try
                {
                    Assembly uiAssembly = typeof(Ankh.UI.AnkhUIModule).Assembly;
                    Type type = uiAssembly.GetType(typeName, true, false);

                    ConstructorInfo constructor = type.GetConstructor(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                        null,
                        Type.EmptyTypes,
                        null);

                    Assert.That(constructor, Is.Not.Null,
                        typeName + " should retain a parameterless constructor for designer/runtime activation.");

                    control = constructor.Invoke(null) as Control;
                    Assert.That(control, Is.Not.Null,
                        typeName + " should construct as a WinForms Control.");

                    // Force creation of the child-control collection. This catches
                    // common InitializeComponent/resource/event-wiring regressions
                    // without showing the UI or requiring Visual Studio services.
                    _ = control.Controls.Count;
                    _ = control.Size;
                }
                catch (Exception ex)
                {
                    failure = ex;
                }
                finally
                {
                    if (control != null)
                        control.Dispose();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (failure != null)
            {
                if (failure is TargetInvocationException && failure.InnerException != null)
                    failure = failure.InnerException;

                Assert.Fail(typeName + " failed WinForms construction: " + failure);
            }
        }
    }
}
