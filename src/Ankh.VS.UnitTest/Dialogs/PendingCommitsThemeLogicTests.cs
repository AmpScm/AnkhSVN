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
using System.Drawing;
using Ankh.UI.PendingChanges.Commits;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture]
    public class PendingCommitsThemeLogicTests
    {
        [TestCase(false, false, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(true, true, false)]
        public void HeaderOptOutUsesSurfaceLuminanceAndHonorsHighContrast(
            bool darkSurface,
            bool highContrast,
            bool expected)
        {
            Assert.That(
                PendingCommitsThemeLogic.ShouldCancelVsHeaderTheming(
                    darkSurface,
                    highContrast),
                Is.EqualTo(expected));
        }

        [Test]
        public void ModifiedDateAlwaysIncludesDateAndTime()
        {
            DateTime recentUtc = DateTime.UtcNow.AddMinutes(-5);
            DateTime olderUtc = DateTime.UtcNow.AddDays(-10);

            Assert.Multiple(() =>
            {
                Assert.That(
                    PendingChangeDisplayLogic.FormatModifiedDate(recentUtc),
                    Is.EqualTo(recentUtc.ToLocalTime().ToString("g")));
                Assert.That(
                    PendingChangeDisplayLogic.FormatModifiedDate(olderUtc),
                    Is.EqualTo(olderUtc.ToLocalTime().ToString("g")));
                Assert.That(
                    PendingChangeDisplayLogic.FormatModifiedDate(DateTime.MinValue),
                    Is.EqualTo(""));
            });
        }

        [Test]
        public void ItemForeColorFallsBackToExplicitListForeground()
        {
            Color listFore = Color.FromArgb(32, 180, 90);
            Color listBack = Color.FromArgb(24, 28, 36);

            Assert.That(
                PendingCommitsThemeLogic.ResolveItemForeColor(
                    Color.Empty,
                    listFore,
                    listBack),
                Is.EqualTo(listFore));
        }

        [Test]
        public void ItemForeColorRejectsLowContrastStatusColor()
        {
            Color listFore = Color.FromArgb(241, 241, 241);
            Color listBack = Color.FromArgb(30, 30, 30);
            Color lowContrastStatus = Color.DarkBlue;

            Assert.That(
                PendingCommitsThemeLogic.ResolveItemForeColor(
                    lowContrastStatus,
                    listFore,
                    listBack),
                Is.EqualTo(listFore));
        }

        [Test]
        public void ItemForeColorKeepsReadableStatusColor()
        {
            Color listFore = Color.Black;
            Color listBack = Color.White;
            Color readableStatus = Color.DarkBlue;

            Assert.That(
                PendingCommitsThemeLogic.ResolveItemForeColor(
                    readableStatus,
                    listFore,
                    listBack),
                Is.EqualTo(readableStatus));
        }
    }
}
