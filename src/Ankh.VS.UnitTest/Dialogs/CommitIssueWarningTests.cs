using System;
using System.Collections.Generic;
using System.Reflection;
using Ankh;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.Scc;
using Ankh.Services.PendingChanges;
using Ankh.UI.IssueTracker;
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture]
    public class CommitIssueWarningTests
    {
        sealed class TestContext : AnkhServiceContainer
        {
            public IVsUIShell Shell;
            public override object GetService(Type type)
            {
                return type == typeof(SVsUIShell) ? Shell : base.GetService(type);
            }
        }

        [TestCase(true, true, false, null, false, 7, false, 1)]
        [TestCase(true, true, false, "", false, 6, true, 1)]
        [TestCase(false, true, false, null, false, 7, true, 0)]
        [TestCase(false, true, false, "", false, 7, true, 0)]
        [TestCase(true, false, false, null, false, 7, true, 0)]
        [TestCase(true, true, true, null, false, 7, true, 0)]
        [TestCase(true, true, false, "42", false, 7, true, 0)]
        [TestCase(true, true, false, null, true, 7, true, 0)]
        public void WarningHonorsAssociationPolicyAndExistingIssue(
            bool attached, bool warn, bool skip, string issue, bool issueInMessage,
            int answer, bool expected, int prompts)
        {
            using (var context = new TestContext())
            {
                var shell = CreateShell(answer);
                context.Shell = shell.Object;
                var settings = CreateSettings(warn);
                context.AddService(typeof(IProjectCommitSettings), settings.Object);
                var service = new Mock<IAnkhIssueService>(MockBehavior.Strict);
                service.SetupGet(s => s.CurrentIssueRepositorySettings)
                    .Returns(attached ? new LocalSvnIssuesSettings(".ankh/issues.xml") : null);
                IEnumerable<TextMarker> markers = issueInMessage
                    ? new[] { new TextMarker(15, 2, "42") } : new TextMarker[0];
                service.Setup(s => s.TryGetIssues(It.IsAny<string>(), out markers)).Returns(true);
                context.AddService(typeof(IAnkhIssueService), service.Object);
                var handler = new PendingChangeHandler(context);
                using (var state = new PendingCommitState(context, new PendingChange[0]))
                {
                    state.LogMessage = issueInMessage ? "Commit message 42" : "Commit message  ";
                    state.IssueText = issue;
                    state.SkipIssueVerify = skip;
                    Assert.That(Verify(handler, state), Is.EqualTo(expected));
                    Assert.That(shell.Invocations.Count, Is.EqualTo(prompts));
                    if (expected)
                        Assert.That(state.LogMessage, Is.EqualTo(
                            (issueInMessage ? "Commit message 42" : "Commit message") + Environment.NewLine));
                }
            }
        }

        [Test]
        public void RemovingTrackerStopsWarningImmediatelyAndReattachingRestoresIt()
        {
            using (var context = new TestContext())
            {
                var shell = CreateShell(7);
                context.Shell = shell.Object;
                context.AddService(typeof(IProjectCommitSettings), CreateSettings(true).Object);
                IssueRepositorySettings association = new LocalSvnIssuesSettings(".ankh/issues.xml");
                var service = new Mock<IAnkhIssueService>(MockBehavior.Strict);
                service.SetupGet(s => s.CurrentIssueRepositorySettings).Returns(() => association);
                IEnumerable<TextMarker> markers = new TextMarker[0];
                service.Setup(s => s.TryGetIssues(It.IsAny<string>(), out markers)).Returns(true);
                context.AddService(typeof(IAnkhIssueService), service.Object);
                var handler = new PendingChangeHandler(context);
                using (var state = new PendingCommitState(context, new PendingChange[0]))
                {
                    state.LogMessage = "Commit message";
                    Assert.That(Verify(handler, state), Is.False);
                    Assert.That(shell.Invocations.Count, Is.EqualTo(1));

                    // Removal retains warnifnoissue and other bugtraq properties.
                    association = null;
                    Assert.That(Verify(handler, state), Is.True);
                    Assert.That(Verify(handler, state), Is.True);
                    Assert.That(shell.Invocations.Count, Is.EqualTo(1));

                    association = new LocalSvnIssuesSettings(".ankh/issues.xml");
                    Assert.That(Verify(handler, state), Is.False);
                    Assert.That(shell.Invocations.Count, Is.EqualTo(2));
                }
            }
        }

        static Mock<IProjectCommitSettings> CreateSettings(bool warn)
        {
            var settings = new Mock<IProjectCommitSettings>(MockBehavior.Strict);
            settings.SetupGet(s => s.WarnIfNoIssue).Returns(warn);
            settings.SetupGet(s => s.ShowIssueBox).Returns(true);
            settings.Setup(s => s.BuildLogMessage(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((message, issue) => message);
            return settings;
        }

        static Mock<IVsUIShell> CreateShell(int answer)
        {
            var shell = new Mock<IVsUIShell>(MockBehavior.Strict);
            shell.Setup(s => s.ShowMessageBox(
                0, ref It.Ref<Guid>.IsAny, "", It.Is<string>(text => text.Contains("issue number")),
                null, 0, OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_WARNING, 0, out answer)).Returns(0);
            return shell;
        }

        static bool Verify(PendingChangeHandler handler, PendingCommitState state)
        {
            return (bool)typeof(PendingChangeHandler).GetMethod(
                "PreCommit_VerifyLogMessage", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(handler, new object[] { state });
        }
    }
}
