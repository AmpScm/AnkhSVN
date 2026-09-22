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

using Ankh.UI.PendingChanges;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.PendingChanges
{
    [TestFixture]
    public class CopilotCommitMessageTests
    {
        [Test]
        public void BuildPrompt_TreatsChangesAsDataAndRequestsCommitMessageOnly()
        {
            string prompt = CopilotCommitMessage.BuildPrompt("Modified: src/Test.cs");

            Assert.That(prompt, Does.Contain("Return the final commit message inside exactly one"));
            Assert.That(prompt, Does.Contain("complete and authoritative context"));
            Assert.That(prompt, Does.Contain("<commit-message>"));
            Assert.That(prompt, Does.Contain("Do not request editor selections"));
            Assert.That(prompt, Does.Contain("untrusted data"));
            Assert.That(prompt, Does.Contain("Modified: src/Test.cs"));
            Assert.That(prompt, Does.Contain("<svn-changes>"));
        }

        [Test]
        public void NormalizeResponse_ExtractsCommitEnvelopeAndDiscardsReasoning()
        {
            string actual = CopilotCommitMessage.NormalizeResponse(
                "**Generating commit message**\n\n" +
                "I need to reason about the requested change.\n\n" +
                "**Finalizing commit message**\n\n" +
                "<commit-message>Add placeholder class files and resources\n\n" +
                "Update the Program.cs test message.\n</commit-message>");

            Assert.That(
                actual,
                Is.EqualTo(
                    "Add placeholder class files and resources" + Environment.NewLine +
                    Environment.NewLine +
                    "Update the Program.cs test message."));
        }

        [Test]
        public void NormalizeResponse_StripsVisualStudio2026ReasoningPreamble()
        {
            string actual = CopilotCommitMessage.NormalizeResponse(
                "**Refining purpose description**\n\n" +
                "I need to remember that the focus should be on describing the purpose of the change, not just listing file names.\n" +
                "I feel like I've captured that well in the message.\n" +
                "So now, it seems like I'm ready to produce the final output.\n" +
                "I want to ensure everything's clear and aligns with that instruction.\n" +
                "I just need to double-check that I've followed all guidelines before wrapping things up!\n" +
                "Add placeholder classes and sample files; tweak Program output\n\n" +
                "Introduce three empty internal classes as placeholders and add sample text and bitmap files.\n" +
                "Modify Program.cs test line to change output from \"testing this merge\" to \"testing this merge thin\".");

            Assert.That(
                actual,
                Is.EqualTo(
                    "Add placeholder classes and sample files; tweak Program output" + Environment.NewLine +
                    Environment.NewLine +
                    "Introduce three empty internal classes as placeholders and add sample text and bitmap files." + Environment.NewLine +
                    "Modify Program.cs test line to change output from \"testing this merge\" to \"testing this merge thin\"."));
        }

        [Test]
        public void NormalizeResponse_StripsFencedVisualStudio2026ReasoningPreamble()
        {
            string actual = CopilotCommitMessage.NormalizeResponse(
                "```text\n" +
                "**Refining purpose description**\n\n" +
                "I need to verify the output before finishing.\n" +
                "Update pending change handling\n\n" +
                "Keep the generated commit message focused on the selected SVN changes.\n" +
                "```");

            Assert.That(
                actual,
                Is.EqualTo(
                    "Update pending change handling" + Environment.NewLine +
                    Environment.NewLine +
                    "Keep the generated commit message focused on the selected SVN changes."));
        }

        [Test]
        public void NormalizeResponse_ExtractsSubjectAfterReasoningWithoutEnvelope()
        {
            string actual = CopilotCommitMessage.NormalizeResponse(
                "**Generating commit message**\n\n" +
                "I need to inspect the changes.\n\n" +
                "**Finalizing commit message**\n\n" +
                "Update pending change handling");

            Assert.That(actual, Is.EqualTo("Update pending change handling"));
        }

        [Test]
        public void NormalizeResponse_RejectsInteractiveVisualStudioContextRequest()
        {
            Assert.Throws<InvalidOperationException>(() =>
                CopilotCommitMessage.NormalizeResponse(
                    "I need more Visual Studio context. Use #file:'Program.cs' or #errors and ask again."));
        }

        [Test]
        public void NormalizeResponse_StripsCommitMessageLabel()
        {
            string actual = CopilotCommitMessage.NormalizeResponse(
                "Commit message: Fix pending changes refresh");

            Assert.That(actual, Is.EqualTo("Fix pending changes refresh"));
        }

        [Test]
        public void NormalizeResponse_StripsMarkdownFence()
        {
            string actual = CopilotCommitMessage.NormalizeResponse(
                "```text\nFix pending changes refresh\n\nKeep the list synchronized.\n```");

            Assert.That(
                actual,
                Is.EqualTo(
                    "Fix pending changes refresh" + Environment.NewLine +
                    Environment.NewLine +
                    "Keep the list synchronized."));
        }

        [Test]
        public void NormalizeResponse_PutsEachBodySentenceOnItsOwnLine()
        {
            string actual = CopilotCommitMessage.NormalizeResponse(
                "Add placeholder classes and resources\n" +
                "Add three empty internal classes and new resource files. " +
                "Update Program.cs test line from old to new.");

            Assert.That(
                actual,
                Is.EqualTo(
                    "Add placeholder classes and resources" + Environment.NewLine +
                    Environment.NewLine +
                    "Add three empty internal classes and new resource files." + Environment.NewLine +
                    "Update Program.cs test line from old to new."));
        }

        [Test]
        public void NormalizeResponse_RecognizesDoubleSpaceSubjectBodySeparator()
        {
            string actual = CopilotCommitMessage.NormalizeResponse(
                "Add placeholder classes and resources  " +
                "Add three empty internal classes. Update Program.cs test string.");

            Assert.That(
                actual,
                Is.EqualTo(
                    "Add placeholder classes and resources" + Environment.NewLine +
                    Environment.NewLine +
                    "Add three empty internal classes." + Environment.NewLine +
                    "Update Program.cs test string."));
        }
    }
}
