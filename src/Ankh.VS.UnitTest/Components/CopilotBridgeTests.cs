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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Ankh.Copilot;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Components
{
    [TestFixture]
    public class CopilotBridgeTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GenerateAsyncRejectsMissingPromptBeforeVisualStudioLookup(string prompt)
        {
            Assert.ThrowsAsync<ArgumentException>(
                async () => await VisualStudioCopilot.GenerateAsync(prompt));
        }

        [Test]
        public void ExtractResponseTextPrefersCommitEnvelopeAndSkipsModelOnlyParts()
        {
            var response = new FakeResponse
            {
                Content = new object[]
                {
                    new FakePart { Content = "private planning", Visibility = FakeVisibility.Model },
                    new FakePart { Content = "Preparing answer", Visibility = FakeVisibility.User },
                    new FakePart
                    {
                        Content = " <commit-message>Fix annotate history</commit-message> ",
                        Visibility = FakeVisibility.All
                    }
                }
            };

            string actual = (string)InvokePrivate("ExtractResponseText", response);

            Assert.That(actual, Is.EqualTo("<commit-message>Fix annotate history</commit-message>"));
        }

        [Test]
        public void ExtractResponseTextFallsBackToLastVisibleTextPart()
        {
            var response = new FakeResponse
            {
                Content = new object[]
                {
                    new FakePart { Content = "First visible part", Visibility = FakeVisibility.User },
                    new FakePart { Content = "Final answer", Visibility = FakeVisibility.All }
                }
            };

            Assert.That(
                InvokePrivate("ExtractResponseText", response),
                Is.EqualTo("Final answer"));
        }

        [Test]
        public void ExtractResponseTextReportsStatusWhenNoVisibleTextExists()
        {
            var response = new FakeResponse
            {
                Status = "Filtered",
                Content = new object[]
                {
                    new FakePart { Content = "model reasoning", Visibility = FakeVisibility.Model }
                }
            };

            TargetInvocationException error = Assert.Throws<TargetInvocationException>(
                () => InvokePrivate("ExtractResponseText", response));

            Assert.That(error.InnerException, Is.TypeOf<InvalidOperationException>());
            Assert.That(error.InnerException.Message, Does.Contain("status: Filtered"));
        }

        [Test]
        public void SelectCommitMessageModelFamilyUsesPreferredUtilityModelOrder()
        {
            var models = new object[]
            {
                new FakeModel { Family = "gpt-4o" },
                new FakeModel { Family = "GPT-4O-MINI" },
                new FakeModel { Family = "other-model" }
            };

            Assert.That(
                InvokePrivate("SelectCommitMessageModelFamily", models),
                Is.EqualTo("gpt-4o-mini"));
        }

        [Test]
        public void SelectCommitMessageModelFamilyReturnsNullWhenNoPreferredModelExists()
        {
            var models = new object[]
            {
                new FakeModel { Family = "other-model" },
                new FakeModel { Family = " " },
                null
            };

            Assert.That(InvokePrivate("SelectCommitMessageModelFamily", models), Is.Null);
        }

        [Test]
        public async Task ConfigureCommitMessageRequestSetsGuidanceAndNonInteractiveIntent()
        {
            var request = new FakeRequest { Intent = FakeIntent.Auto };

            Task task = (Task)InvokePrivate(
                "ConfigureCommitMessageRequestAsync",
                null,
                request);
            await task;

            Assert.Multiple(() =>
            {
                Assert.That(request.Guidance, Does.Contain("headless, non-interactive"));
                Assert.That(request.Guidance, Does.Contain("<commit-message>"));
                Assert.That(request.Intent, Is.EqualTo(FakeIntent.None));
            });
        }

        [Test]
        public void CopilotSearchDirectoriesIncludeBothSupportedVisualStudioLocations()
        {
            var directories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string ideDirectory = Path.Combine("C:", "VS", "Common7", "IDE");

            InvokePrivate("AddCopilotDirectory", directories, ideDirectory);

            Assert.Multiple(() =>
            {
                Assert.That(directories, Does.Contain(
                    Path.Combine(ideDirectory, "Extensions", "Microsoft", "Copilot")));
                Assert.That(directories, Does.Contain(
                    Path.Combine(ideDirectory, "CommonExtensions", "Microsoft", "Copilot")));
                Assert.That(directories.Count, Is.EqualTo(2));
            });
        }

        static object InvokePrivate(string name, params object[] arguments)
        {
            MethodInfo method = typeof(VisualStudioCopilot).GetMethod(
                name,
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(method, Is.Not.Null, "Expected private bridge method '{0}'", name);
            return method.Invoke(null, arguments);
        }

        enum FakeVisibility
        {
            User,
            Model,
            All
        }

        enum FakeIntent
        {
            Auto,
            None
        }

        sealed class FakePart
        {
            public string Content { get; set; }
            public FakeVisibility Visibility { get; set; }
        }

        sealed class FakeResponse
        {
            public IEnumerable Content { get; set; }
            public string Status { get; set; }
        }

        sealed class FakeModel
        {
            public string Family { get; set; }
        }

        sealed class FakeRequest
        {
            public string Guidance { get; set; }
            public FakeIntent Intent { get; set; }
        }
    }
}
