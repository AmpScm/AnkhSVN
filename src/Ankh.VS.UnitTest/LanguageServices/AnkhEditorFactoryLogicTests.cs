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

using Ankh.VS.LanguageServices.Core;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.LanguageServices
{
    [TestFixture]
    public class AnkhEditorFactoryLogicTests
    {
        [TestCase(false, false, false)]
        [TestCase(false, true, false)]
        [TestCase(true, false, false)]
        [TestCase(true, true, true)]
        public void ShouldRejectEncodingPrompt_RequiresPromptAndExistingData(
            bool prompt,
            bool existingData,
            bool expected)
        {
            Assert.That(
                AnkhEditorFactoryLogic.ShouldRejectEncodingPrompt(
                    prompt,
                    existingData),
                Is.EqualTo(expected));
        }

        [Test]
        public void EvaluateExtension_NoMonikerNeedsNoTakeover()
        {
            EditorExtensionDecision decision =
                AnkhEditorFactoryLogic.EvaluateExtension(
                    false,
                    false,
                    false,
                    false,
                    false,
                    true);

            Assert.That(decision.IsSupported, Is.True);
            Assert.That(decision.Takeover, Is.False);
            Assert.That(decision.RequiresFormatCheck, Is.False);
        }

        [Test]
        public void EvaluateExtension_RejectsUnclaimedUnsupportedExtension()
        {
            EditorExtensionDecision decision =
                AnkhEditorFactoryLogic.EvaluateExtension(
                    true,
                    false,
                    false,
                    false,
                    false,
                    false);

            Assert.That(decision.IsSupported, Is.False);
            Assert.That(decision.Takeover, Is.False);
            Assert.That(decision.RequiresFormatCheck, Is.False);
        }

        [Test]
        public void EvaluateExtension_RegisteredExtensionDoesNotTakeOver()
        {
            EditorExtensionDecision decision =
                AnkhEditorFactoryLogic.EvaluateExtension(
                    true,
                    false,
                    true,
                    false,
                    false,
                    true);

            Assert.That(decision.IsSupported, Is.True);
            Assert.That(decision.Takeover, Is.False);
            Assert.That(decision.RequiresFormatCheck, Is.False);
        }

        [Test]
        public void EvaluateExtension_WildcardTakeoverRequiresFormatSniff()
        {
            EditorExtensionDecision decision =
                AnkhEditorFactoryLogic.EvaluateExtension(
                    true,
                    false,
                    false,
                    false,
                    true,
                    true);

            Assert.That(decision.IsSupported, Is.True);
            Assert.That(decision.Takeover, Is.True);
            Assert.That(decision.RequiresFormatCheck, Is.True);
        }

        [Test]
        public void EvaluateExtension_UserDefinedWildcardSkipsFormatSniff()
        {
            EditorExtensionDecision decision =
                AnkhEditorFactoryLogic.EvaluateExtension(
                    true,
                    false,
                    false,
                    true,
                    false,
                    true);

            Assert.That(decision.IsSupported, Is.True);
            Assert.That(decision.Takeover, Is.True);
            Assert.That(decision.RequiresFormatCheck, Is.False);
        }

        [Test]
        public void EvaluateExtension_OpenSpecificWildcardSkipsFormatSniff()
        {
            EditorExtensionDecision decision =
                AnkhEditorFactoryLogic.EvaluateExtension(
                    true,
                    true,
                    false,
                    false,
                    false,
                    true);

            Assert.That(decision.IsSupported, Is.True);
            Assert.That(decision.Takeover, Is.True);
            Assert.That(decision.RequiresFormatCheck, Is.False);
        }

        [Test]
        public void EvaluateExtension_CanEditAnywayWithoutWildcardDoesNotTakeOver()
        {
            EditorExtensionDecision decision =
                AnkhEditorFactoryLogic.EvaluateExtension(
                    true,
                    false,
                    false,
                    false,
                    true,
                    false);

            Assert.That(decision.IsSupported, Is.True);
            Assert.That(decision.Takeover, Is.False);
            Assert.That(decision.RequiresFormatCheck, Is.False);
        }

        [Test]
        public void EvaluateLanguageService_EmptyRequestDoesNothing()
        {
            EditorLanguageDecision decision =
                AnkhEditorFactoryLogic.EvaluateLanguageService(
                    Guid.Empty,
                    Guid.NewGuid(),
                    Guid.NewGuid());

            Assert.That(
                decision.Action,
                Is.EqualTo(EditorLanguageAction.None));
            Assert.That(decision.Takeover, Is.False);
        }

        [Test]
        public void EvaluateLanguageService_DefaultServiceIsReplaced()
        {
            Guid requested = Guid.NewGuid();
            Guid defaultService = Guid.NewGuid();

            EditorLanguageDecision decision =
                AnkhEditorFactoryLogic.EvaluateLanguageService(
                    requested,
                    defaultService,
                    defaultService);

            Assert.That(
                decision.Action,
                Is.EqualTo(EditorLanguageAction.SetRequested));
            Assert.That(decision.Takeover, Is.True);
        }

        [Test]
        public void EvaluateLanguageService_ExistingRequestedServiceIsKept()
        {
            Guid requested = Guid.NewGuid();

            EditorLanguageDecision decision =
                AnkhEditorFactoryLogic.EvaluateLanguageService(
                    requested,
                    requested,
                    Guid.NewGuid());

            Assert.That(
                decision.Action,
                Is.EqualTo(EditorLanguageAction.KeepRequested));
            Assert.That(decision.Takeover, Is.True);
        }

        [Test]
        public void EvaluateLanguageService_DifferentServiceIsIncompatible()
        {
            EditorLanguageDecision decision =
                AnkhEditorFactoryLogic.EvaluateLanguageService(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid());

            Assert.That(
                decision.Action,
                Is.EqualTo(EditorLanguageAction.Incompatible));
            Assert.That(decision.Takeover, Is.False);
        }
    }
}
