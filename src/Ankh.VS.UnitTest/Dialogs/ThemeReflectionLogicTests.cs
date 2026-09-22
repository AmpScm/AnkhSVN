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

using Ankh.WpfPackage.Services;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture]
    public class ThemeReflectionLogicTests
    {
        sealed class ThemeA
        {
            public Guid ThemeId { get; set; }
        }

        sealed class ThemeB
        {
            public Guid ThemeId { get; set; }
        }

        sealed class ThrowingTheme
        {
            public Guid ThemeId
            {
                get { throw new InvalidOperationException("Theme unavailable"); }
            }
        }

        [Test]
        public void CachedPropertyIsRefreshedWhenRuntimeTargetTypeChanges()
        {
            Guid firstId = Guid.NewGuid();
            Guid secondId = Guid.NewGuid();
            PropertyInfo cached = null;
            object value;

            Assert.That(
                ThemeReflectionLogic.TryGetPropertyValue(
                    new ThemeA { ThemeId = firstId },
                    "ThemeId",
                    ref cached,
                    out value),
                Is.True);
            Assert.That(value, Is.EqualTo(firstId));

            Assert.That(
                ThemeReflectionLogic.TryGetPropertyValue(
                    new ThemeB { ThemeId = secondId },
                    "ThemeId",
                    ref cached,
                    out value),
                Is.True);
            Assert.That(value, Is.EqualTo(secondId));
            Assert.That(cached.DeclaringType, Is.EqualTo(typeof(ThemeB)));
        }

        [Test]
        public void TargetInvocationFailureDoesNotEscapeThemeDetection()
        {
            PropertyInfo cached = null;
            object value;

            Assert.That(
                ThemeReflectionLogic.TryGetPropertyValue(
                    new ThrowingTheme(),
                    "ThemeId",
                    ref cached,
                    out value),
                Is.False);
            Assert.That(cached, Is.Null);
            Assert.That(value, Is.Null);
        }

        [Test]
        public void MissingPropertyReturnsFalse()
        {
            PropertyInfo cached = null;
            object value;

            Assert.That(
                ThemeReflectionLogic.TryGetPropertyValue(
                    new object(),
                    "ThemeId",
                    ref cached,
                    out value),
                Is.False);
            Assert.That(cached, Is.Null);
        }
    }
}
