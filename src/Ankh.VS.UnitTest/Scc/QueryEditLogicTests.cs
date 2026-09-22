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

using Ankh.Scc;
using Microsoft.VisualStudio.Shell.Interop;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Scc
{
    [TestFixture]
    public class QueryEditLogicTests
    {
        [Test]
        public void AllowsUI_OnlyForInteractiveQueries()
        {
            Assert.That(QueryEditLogic.AllowsUI(0), Is.True);
            Assert.That(
                QueryEditLogic.AllowsUI(tagVSQueryEditFlags.QEF_SilentMode),
                Is.False);
            Assert.That(
                QueryEditLogic.AllowsUI(tagVSQueryEditFlags.QEF_ReportOnly),
                Is.False);
            Assert.That(
                QueryEditLogic.AllowsUI(tagVSQueryEditFlags.QEF_ForceEdit_NoPrompting),
                Is.False);
            Assert.That(
                QueryEditLogic.AllowsUI(
                    tagVSQueryEditFlags.QEF_SilentMode
                    | tagVSQueryEditFlags.QEF_ReportOnly),
                Is.False);
        }

        [TestCase(true, false, true, false)]
        [TestCase(true, true, true, true)]
        [TestCase(false, false, true, true)]
        [TestCase(false, false, false, false)]
        public void NeedsReadOnlyNonSccPolicy_MatchesProviderPrecedence(
            bool mustLock,
            bool isDirectory,
            bool isReadOnly,
            bool expected)
        {
            Assert.That(
                QueryEditLogic.NeedsReadOnlyNonSccPolicy(
                    mustLock,
                    isDirectory,
                    isReadOnly),
                Is.EqualTo(expected));
        }

        [Test]
        public void GetFileAction_MustLockFilesQueueOrRejectByUiMode()
        {
            Assert.That(
                QueryEditLogic.GetFileAction(true, false, true, true, true).ToString(),
                Is.EqualTo("QueueMustLock"));

            Assert.That(
                QueryEditLogic.GetFileAction(true, false, true, false, true).ToString(),
                Is.EqualTo("RejectMustLock"));
        }

        [Test]
        public void GetFileAction_ReadOnlyFilesRespectNonSccWritePolicy()
        {
            Assert.That(
                QueryEditLogic.GetFileAction(false, false, true, true, true).ToString(),
                Is.EqualTo("None"));

            Assert.That(
                QueryEditLogic.GetFileAction(false, false, true, true, false).ToString(),
                Is.EqualTo("QueueReadOnly"));

            Assert.That(
                QueryEditLogic.GetFileAction(false, false, true, false, false).ToString(),
                Is.EqualTo("RejectReadOnly"));
        }

        [Test]
        public void GetFileAction_DirectoryMustLockFallsThroughToReadOnlyHandling()
        {
            Assert.That(
                QueryEditLogic.GetFileAction(true, true, true, true, false).ToString(),
                Is.EqualTo("QueueReadOnly"));

            Assert.That(
                QueryEditLogic.GetFileAction(true, true, true, false, false).ToString(),
                Is.EqualTo("RejectReadOnly"));
        }

        [Test]
        public void GetFileAction_WritableFilesNeedNoAction()
        {
            Assert.That(
                QueryEditLogic.GetFileAction(false, false, false, true, false).ToString(),
                Is.EqualTo("None"));
        }
    }
}
