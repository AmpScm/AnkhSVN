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

using System.Collections.Generic;

using Ankh.Commands;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Commands
{
    [TestFixture]
    public class LockCommandLogicTests
    {
        [Test]
        public void IsLockCandidate_RequiresLockableVersionedFile()
        {
            Assert.That(
                LockCommandLogic.IsLockCandidate(
                    true,
                    true,
                    false,
                    false),
                Is.True);

            Assert.That(
                LockCommandLogic.IsLockCandidate(
                    false,
                    true,
                    false,
                    false),
                Is.False);

            Assert.That(
                LockCommandLogic.IsLockCandidate(
                    true,
                    false,
                    false,
                    false),
                Is.False);

            Assert.That(
                LockCommandLogic.IsLockCandidate(
                    true,
                    true,
                    true,
                    false),
                Is.False);

            Assert.That(
                LockCommandLogic.IsLockCandidate(
                    true,
                    true,
                    false,
                    true),
                Is.False);
        }

        [Test]
        public void ShouldPrompt_MatchesCommandFlagsAndUiSuppression()
        {
            Assert.That(
                LockCommandLogic.ShouldPrompt(
                    false,
                    false,
                    false,
                    false),
                Is.True);

            Assert.That(
                LockCommandLogic.ShouldPrompt(
                    false,
                    true,
                    true,
                    true),
                Is.True);

            Assert.That(
                LockCommandLogic.ShouldPrompt(
                    false,
                    false,
                    true,
                    false),
                Is.False);

            Assert.That(
                LockCommandLogic.ShouldPrompt(
                    false,
                    false,
                    false,
                    true),
                Is.False);

            Assert.That(
                LockCommandLogic.ShouldPrompt(
                    true,
                    true,
                    false,
                    false),
                Is.False);
        }

        [Test]
        public void GetLockOwner_PrefersReportedOwner()
        {
            Assert.That(
                LockCommandLogic.GetLockOwner(
                    "reported-user",
                    "ignored"),
                Is.EqualTo("reported-user"));
        }

        [Test]
        public void GuessUserFromError_ParsesEnglishAndGermanQuoteStyles()
        {
            Assert.That(
                LockCommandLogic.GuessUserFromError(
                    "Path '/trunk/file.txt' is already locked by user 'alex' in filesystem 'repo'"),
                Is.EqualTo("alex"));

            Assert.That(
                LockCommandLogic.GuessUserFromError(
                    "Pfad »/trunk/file.txt« ist bereits vom Benutzer »alex« im Dateisystem »repo« gesperrt"),
                Is.EqualTo("alex"));
        }

        [Test]
        public void GetLockOwner_FallsBackWhenErrorCannotBeParsed()
        {
            Assert.That(
                LockCommandLogic.GetLockOwner(
                    null,
                    "unrecognized lock error"),
                Is.EqualTo("?"));
        }

        [Test]
        public void BuildAlreadyLockedMessage_FormatsOwnersAndUnknownOwners()
        {
            List<KeyValuePair<string, string>> items =
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(
                        "one.txt",
                        "alex"),
                    new KeyValuePair<string, string>(
                        "two.txt",
                        "")
                };

            string message =
                LockCommandLogic.BuildAlreadyLockedMessage(
                    "Already locked",
                    "{0} by {1}",
                    items);

            StringAssert.Contains("Already locked", message);
            StringAssert.Contains("one.txt by alex", message);
            StringAssert.Contains("two.txt", message);
            Assert.That(message.EndsWith("\r") || message.EndsWith("\n"), Is.False);
        }
    }
}
