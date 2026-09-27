using System;
using System.Reflection;
using Ankh;
using Ankh.Scc;
using Ankh.Services.PendingChanges;
using Ankh.VS;
using Moq;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture]
    public class CommitIssueWarningTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("42")]
        public void CommitFormatsMessageWithoutIssueConfirmationEvenWhenRepositoryRequestsWarning(string issue)
        {
            using (var context = new AnkhServiceContainer())
            {
                var settings = new Mock<IProjectCommitSettings>(MockBehavior.Strict);
                settings.SetupGet(s => s.WarnIfNoIssue).Returns(true);
                settings.SetupGet(s => s.ShowIssueBox).Returns(true);
                settings.Setup(s => s.BuildLogMessage("Commit message" + Environment.NewLine, issue))
                    .Returns<string, string>((message, id) => message + (string.IsNullOrEmpty(id) ? "" : "Issue " + id));
                context.AddService(typeof(IProjectCommitSettings), settings.Object);
                var handler = new PendingChangeHandler(context);
                using (var state = new PendingCommitState(context, new PendingChange[0]))
                {
                    state.LogMessage = "Commit message  ";
                    state.IssueText = issue;
                    var method = typeof(PendingChangeHandler).GetMethod("PreCommit_VerifyLogMessage", BindingFlags.Instance | BindingFlags.NonPublic);
                    Assert.That(method.Invoke(handler, new object[] { state }), Is.True);
                    Assert.That(state.LogMessage, Is.EqualTo("Commit message" + Environment.NewLine
                        + (string.IsNullOrEmpty(issue) ? "" : "Issue 42" + Environment.NewLine)));
                    Assert.That(typeof(PendingCommitState).GetField("_mb", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(state), Is.Null);
                    settings.Verify(s => s.BuildLogMessage(It.IsAny<string>(), issue), Times.Once);
                }
            }
        }
    }
}
