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

using Ankh.Services;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Services
{
    [TestFixture]
    public class DiffToolTemplateLogicTests
    {
        [Test]
        public void FirstNonNull_ReturnsFirstAvailableValue()
        {
            Assert.That(
                DiffToolTemplateLogic.FirstNonNull(null, "second", "third"),
                Is.EqualTo("second"));
        }

        [Test]
        public void FirstNonNull_PreservesEmptyStringLikeNullCoalescing()
        {
            Assert.That(
                DiffToolTemplateLogic.FirstNonNull(null, "", "fallback"),
                Is.EqualTo(""));
        }

        [Test]
        public void FirstNonNull_ReturnsFirstValueWhenPresent()
        {
            Assert.That(
                DiffToolTemplateLogic.FirstNonNull("first", "second"),
                Is.EqualTo("first"));
        }

        [Test]
        public void FirstNonNull_ReturnsNullWhenNoValueExists()
        {
            Assert.That(
                DiffToolTemplateLogic.FirstNonNull(null, null),
                Is.Null);
            Assert.That(
                DiffToolTemplateLogic.FirstNonNull(null),
                Is.Null);
        }
    }
}
