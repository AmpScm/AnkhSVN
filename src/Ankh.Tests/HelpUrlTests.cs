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
using System.Reflection;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    public class HelpUrlTests
    {
        [TestCase("dlgHelp", "Ankh.UI.Commands.UpdateDialog", "/AnkhSVN/help/working-copy/")]
        [TestCase("ctrlHelp", "Ankh.UI.PendingChanges.PendingCommitsPage", "/AnkhSVN/help/commit/")]
        [TestCase("ctrlHelp", "Ankh.UI.PendingChanges.PendingIssuesPage", "/AnkhSVN/help/issues/")]
        [TestCase("dlgHelp", "Ankh.UI.IssueTracker.IssueTrackerConfigDialog", "/AnkhSVN/help/issues/")]
        [TestCase("dlgHelp", "Ankh.UI.Commands.AnnotateDialog", "/AnkhSVN/help/annotate/")]
        [TestCase("dlgHelp", "Ankh.UI.PathSelector.CommonFileSelectorDialog", "/AnkhSVN/help/diff/")]
        [TestCase("dlgHelp", "Ankh.UI.PropertyEditors.ExternalsPropertyEditor", "/AnkhSVN/help/externals/")]
        [TestCase("dlgHelp", "Ankh.UI.Commands.ResolveConflictDialog", "/AnkhSVN/help/conflicts/")]
        [TestCase("dlgHelp", "Ankh.UI.Commands.UnknownDialog", "/AnkhSVN/help/")]
        public void HelpUrlsRouteToGitHubPagesTopics(string helpType, string dialogType, string expectedPath)
        {
            Type serviceType = Assembly.Load("Ankh").GetType("Ankh.Services.AnkhHelpService", true);
            MethodInfo builder = serviceType.GetMethod(
                "BuildHelpUri",
                BindingFlags.Static | BindingFlags.NonPublic);

            Assert.NotNull(builder);

            Uri uri = (Uri)builder.Invoke(null, new object[]
            {
                helpType,
                new Version(2, 9, 192),
                1033,
                dialogType
            });

            Assert.AreEqual("https", uri.Scheme);
            Assert.IsFalse(string.IsNullOrWhiteSpace(uri.Host));
            Assert.AreEqual(expectedPath, uri.AbsolutePath);
            StringAssert.Contains("t=" + helpType, uri.Query);
            StringAssert.Contains("v=2.9.192", uri.Query);
            StringAssert.Contains("l=1033", uri.Query);
            StringAssert.Contains("dt=" + dialogType, uri.Query);
            StringAssert.DoesNotContain("svc.ankhsvn.net", uri.AbsoluteUri);
        }
    }
}
