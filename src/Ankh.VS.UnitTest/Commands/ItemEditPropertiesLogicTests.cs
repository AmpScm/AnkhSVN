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
using System.Collections.Generic;

using Ankh.Commands;
using NUnit.Framework;
using SharpSvn;

namespace AnkhSvn_UnitTestProject.Commands
{
    [TestFixture]
    public class ItemEditPropertiesLogicTests
    {
        [Test]
        public void HasPersistableChanges_RejectsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => ItemEditPropertiesLogic.HasPersistableChanges(null));
        }

        [Test]
        public void HasPersistableChanges_DetectsAnyPersistableItem()
        {
            Assert.That(
                ItemEditPropertiesLogic.HasPersistableChanges(
                    new[] { false, false, true, false }),
                Is.True);

            Assert.That(
                ItemEditPropertiesLogic.HasPersistableChanges(
                    new[] { false, false }),
                Is.False);

            Assert.That(
                ItemEditPropertiesLogic.HasPersistableChanges(
                    new List<bool>()),
                Is.False);
        }

        [Test]
        public void GetPersistenceAction_IgnoresItemsNotMarkedForPersistence()
        {
            Assert.That(
                ItemEditPropertiesLogic.GetPersistenceAction(
                    false,
                    new SvnPropertyValue("p", "old"),
                    new SvnPropertyValue("p", "new")),
                Is.EqualTo(PropertyPersistenceAction.None));
        }

        [Test]
        public void GetPersistenceAction_DeletesExistingPropertyWhenValueIsRemoved()
        {
            Assert.That(
                ItemEditPropertiesLogic.GetPersistenceAction(
                    true,
                    new SvnPropertyValue("p", "old"),
                    null),
                Is.EqualTo(PropertyPersistenceAction.Delete));
        }

        [Test]
        public void GetPersistenceAction_DoesNothingWhenBothValuesAreMissing()
        {
            Assert.That(
                ItemEditPropertiesLogic.GetPersistenceAction(true, null, null),
                Is.EqualTo(PropertyPersistenceAction.None));
        }

        [Test]
        public void GetPersistenceAction_DoesNothingForEqualStringValue()
        {
            Assert.That(
                ItemEditPropertiesLogic.GetPersistenceAction(
                    true,
                    new SvnPropertyValue("p", "same"),
                    new SvnPropertyValue("p", "same")),
                Is.EqualTo(PropertyPersistenceAction.None));
        }

        [Test]
        public void GetPersistenceAction_SetsChangedOrNewStringValue()
        {
            Assert.That(
                ItemEditPropertiesLogic.GetPersistenceAction(
                    true,
                    new SvnPropertyValue("p", "old"),
                    new SvnPropertyValue("p", "new")),
                Is.EqualTo(PropertyPersistenceAction.SetString));

            Assert.That(
                ItemEditPropertiesLogic.GetPersistenceAction(
                    true,
                    null,
                    new SvnPropertyValue("p", "new")),
                Is.EqualTo(PropertyPersistenceAction.SetString));
        }

        [Test]
        public void GetPersistenceAction_DoesNothingForEqualRawValue()
        {
            Assert.That(
                ItemEditPropertiesLogic.GetPersistenceAction(
                    true,
                    new SvnPropertyValue("p", new byte[] { 1, 2, 3 }),
                    new SvnPropertyValue("p", new byte[] { 1, 2, 3 })),
                Is.EqualTo(PropertyPersistenceAction.None));
        }

        [Test]
        public void GetPersistenceAction_SetsChangedOrNewRawValue()
        {
            Assert.That(
                ItemEditPropertiesLogic.GetPersistenceAction(
                    true,
                    new SvnPropertyValue("p", new byte[] { 1, 2, 3 }),
                    new SvnPropertyValue("p", new byte[] { 1, 2, 4 })),
                Is.EqualTo(PropertyPersistenceAction.SetRaw));

            Assert.That(
                ItemEditPropertiesLogic.GetPersistenceAction(
                    true,
                    null,
                    new SvnPropertyValue("p", new byte[] { 9, 8, 7 })),
                Is.EqualTo(PropertyPersistenceAction.SetRaw));
        }
    }
}
