using System;
using System.Reflection;
using System.Runtime.Serialization;
using Ankh.UI;
using NUnit.Framework;
using SharpSvn;

namespace Ankh.Tests
{
    [TestFixture]
    public class ProgressDialogActionTextTests
    {
        static string GetActionText(SvnNotifyAction action)
        {
            var dialog = (ProgressDialog)FormatterServices.GetUninitializedObject(typeof(ProgressDialog));
            MethodInfo method = typeof(ProgressDialog).GetMethod(
                "GetActionText",
                BindingFlags.Instance | BindingFlags.NonPublic);

            return (string)method.Invoke(dialog, new object[] { action });
        }

        [TestCase(SvnNotifyAction.UpdateAdd, "Add")]
        [TestCase(SvnNotifyAction.UpdateDelete, "Delete")]
        [TestCase(SvnNotifyAction.UpdateReplace, "Replace")]
        [TestCase(SvnNotifyAction.UpdateUpdate, "Update")]
        [TestCase(SvnNotifyAction.UpdateCompleted, "Completed")]
        [TestCase(SvnNotifyAction.UpdateExternal, "External")]
        [TestCase(SvnNotifyAction.UpdateSkipWorkingOnly, "SkipWorkingOnly")]
        [TestCase(SvnNotifyAction.UpdateSkipObstruction, "SkipObstruction")]
        [TestCase(SvnNotifyAction.UpdateSkipAccessDenied, "SkipAccessDenied")]
        [TestCase(SvnNotifyAction.UpdateShadowedAdd, "ShadowedAdd")]
        [TestCase(SvnNotifyAction.UpdateShadowedDelete, "ShadowedDelete")]
        [TestCase(SvnNotifyAction.UpdateShadowedUpdate, "ShadowedUpdate")]
        [TestCase(SvnNotifyAction.UpdateExternalRemoved, "ExternalRemoved")]
        [TestCase(SvnNotifyAction.UpdateBrokenLock, "BrokenLock")]
        public void UpdateActionsDropUpdatePrefix(SvnNotifyAction action, string expected)
        {
            Assert.That(GetActionText(action), Is.EqualTo(expected));
        }

        [TestCase(SvnNotifyAction.CommitAdded, "Added")]
        [TestCase(SvnNotifyAction.CommitDeleted, "Deleted")]
        [TestCase(SvnNotifyAction.CommitModified, "Modified")]
        [TestCase(SvnNotifyAction.CommitReplaced, "Replaced")]
        [TestCase(SvnNotifyAction.CommitAddCopy, "AddCopy")]
        [TestCase(SvnNotifyAction.CommitReplacedWithCopy, "ReplacedWithCopy")]
        [TestCase(SvnNotifyAction.CommitFinalizing, "Finalizing")]
        public void CommitActionsDropCommitPrefix(SvnNotifyAction action, string expected)
        {
            Assert.That(GetActionText(action), Is.EqualTo(expected));
        }

        [TestCase(SvnNotifyAction.UpgradedDirectory, "Upgraded")]
        [TestCase(SvnNotifyAction.CommitSendData, "Sending")]
        [TestCase(SvnNotifyAction.BlameRevision, "Annotating")]
        public void SpecialActionsHaveFriendlyText(SvnNotifyAction action, string expected)
        {
            Assert.That(GetActionText(action), Is.EqualTo(expected));
        }

        [TestCase(SvnNotifyAction.UpdateStarted)]
        [TestCase(SvnNotifyAction.RecordMergeInfoStarted)]
        [TestCase(SvnNotifyAction.FollowUrlRedirect)]
        [TestCase(SvnNotifyAction.OperationRequiresTarget)]
        public void StructuralActionsAreHidden(SvnNotifyAction action)
        {
            Assert.That(GetActionText(action), Is.Null);
        }

        [Test]
        public void UnknownActionFallsBackToEnumText()
        {
            var action = (SvnNotifyAction)int.MaxValue;
            Assert.That(GetActionText(action), Is.EqualTo(action.ToString()));
        }
    }
}
