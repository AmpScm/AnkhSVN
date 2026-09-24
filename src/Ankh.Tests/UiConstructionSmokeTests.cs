using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using Ankh;
using SharpSvn;

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
            "Ankh.UI.PathSelector.VersionSelector",
            "Ankh.UI.RepositoryExplorer.Dialogs.RenameDialog",
            "Ankh.UI.RepositoryExplorer.Dialogs.ConfirmDeleteDialog",
            "Ankh.UI.AddRepositoryRootDialog",
            "Ankh.UI.WorkingCopyExplorer.AddWorkingCopyExplorerRootDialog",
            "Ankh.UI.SccManagement.CreateDirectoryDialog",
            "Ankh.UI.SccManagement.LockDialog",
            "Ankh.UI.SccManagement.UpdateAvailableDialog",
            "Ankh.UI.Commands.RecentMessageDialog",
            "Ankh.UI.Commands.AnnotateDialog",
            "Ankh.UI.Commands.CheckoutDialog",
            "Ankh.UI.Commands.ExportDialog",
            "Ankh.UI.Commands.SwitchDialog",
            "Ankh.UI.Commands.UpdateDialog",
            "Ankh.UI.ErrorDialog",
            "Ankh.UI.IssueTracker.IssueTrackerConfigDialog",
            "Ankh.UI.MergeWizard.MergeConflictHandlerDialog",
            "Ankh.UI.MergeWizard.MergeResultsDialog",
            "Ankh.UI.OptionsPages.ToolArgumentDialog",
            "Ankh.UI.PropertyEditors.PropertyDialog",
            "Ankh.UI.PropertyEditors.PropertyEditorDialog",
            "Ankh.UI.RepositoryExplorer.RepositoryFolderBrowserDialog",
            "Ankh.UI.RepositoryExplorer.RepositoryWizard.RepositorySelectionWizard",
            "Ankh.UI.RepositoryOpen.ProjectAddInfoDialog",
            "Ankh.UI.RepositoryOpen.RepositoryOpenDialog",
            "Ankh.UI.SccManagement.AddProjectToSubversion",
            "Ankh.UI.SccManagement.AddToSubversion",
            "Ankh.UI.SccManagement.CopyToDialog",
            "Ankh.UI.SccManagement.CreateBranchDialog",
            "Ankh.UI.SccManagement.ItemCompareDialog",
            "Ankh.UI.SccManagement.ItemUpdateDialog",
            "Ankh.UI.SccManagement.MultiWorkingCopyCommit",
            "Ankh.UI.SvnLog.EditLogMessageDialog",
            "Ankh.UI.SvnLog.LogViewerDialog"
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

                    ConstructorInfo constructor;
                    object[] arguments;

                    switch (typeName)
                    {
                        case "Ankh.UI.PropertyEditors.PropertyDialog":
                            constructor = type.GetConstructor(new[] { typeof(SvnNodeKind) });
                            arguments = new object[] { SvnNodeKind.File };
                            break;

                        case "Ankh.UI.PropertyEditors.PropertyEditorDialog":
                            constructor = type.GetConstructor(new[] { typeof(string) });
                            arguments = new object[] { "C:\\working-copy\\file.txt" };
                            break;

                        case "Ankh.UI.RepositoryExplorer.RepositoryWizard.RepositorySelectionWizard":
                            constructor = type.GetConstructor(new[] { typeof(IAnkhServiceProvider) });
                            arguments = new object[] { null };
                            break;

                        default:
                            constructor = type.GetConstructor(
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                null,
                                Type.EmptyTypes,
                                null);
                            arguments = null;
                            break;
                    }

                    Assert.That(constructor, Is.Not.Null,
                        typeName + " should retain its supported runtime constructor.");

                    control = constructor.Invoke(arguments) as Control;
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
