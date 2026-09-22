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

using Ankh.Commands;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Commands
{
    [TestFixture]
    public class AnnotateCommandLogicTests
    {
        [TestCase(true, true, true, true)]
        [TestCase(false, true, true, false)]
        [TestCase(true, false, true, false)]
        [TestCase(true, true, false, false)]
        public void IsAnnotatableItem_RequiresFileVersionAndHistory(
            bool isFile,
            bool isVersioned,
            bool hasHistory,
            bool expected)
        {
            Assert.That(
                AnnotateCommandLogic.IsAnnotatableItem(
                    isFile,
                    isVersioned,
                    hasHistory),
                Is.EqualTo(expected));
        }

        [TestCase(false, false, false, true)]
        [TestCase(false, true, false, false)]
        [TestCase(true, false, false, false)]
        [TestCase(true, true, false, false)]
        [TestCase(true, true, true, true)]
        [TestCase(false, true, true, true)]
        public void ShouldPrompt_PreservesCommandPromptRules(
            bool dontPrompt,
            bool shiftPressed,
            bool promptUser,
            bool expected)
        {
            Assert.That(
                AnnotateCommandLogic.ShouldPrompt(
                    dontPrompt,
                    shiftPressed,
                    promptUser),
                Is.EqualTo(expected));
        }

        [TestCase(true, false, true, true)]
        [TestCase(false, true, true, true)]
        [TestCase(true, true, true, true)]
        [TestCase(false, false, true, false)]
        [TestCase(true, false, false, false)]
        [TestCase(false, true, false, false)]
        public void ShouldSaveDocument_RequiresWorkingRevisionAndPathTarget(
            bool startWorking,
            bool endWorking,
            bool targetIsPath,
            bool expected)
        {
            Assert.That(
                AnnotateCommandLogic.ShouldSaveDocument(
                    startWorking,
                    endWorking,
                    targetIsPath),
                Is.EqualTo(expected));
        }
    }
}
