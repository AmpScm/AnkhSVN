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

using System.Windows.Forms;

using Ankh.UI.PendingChanges.Commits;
using Ankh.UI.VSSelectionControls;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.PendingChanges
{
    [TestFixture]
    public class PendingCommitsViewDoubleClickTests
    {
        [TestCase(ListViewHitTestLocations.Label, true)]
        [TestCase(ListViewHitTestLocations.Image, true)]
        [TestCase(ListViewHitTestLocations.StateImage, false)]
        [TestCase(ListViewHitTestLocations.None, false)]
        public void ShouldOpenPendingChangeOnDoubleClick_DoesNotTreatCheckboxAsOpenAction(
            ListViewHitTestLocations location,
            bool expected)
        {
            Assert.That(
                PendingCommitsView.ShouldOpenPendingChangeOnDoubleClick(location),
                Is.EqualTo(expected));
        }

        [TestCase(true, true, true, ListViewHitTestLocations.Label, true)]
        [TestCase(true, true, true, ListViewHitTestLocations.Image, true)]
        [TestCase(true, true, true, ListViewHitTestLocations.StateImage, false)]
        [TestCase(true, true, false, ListViewHitTestLocations.Label, false)]
        [TestCase(true, false, true, ListViewHitTestLocations.Label, false)]
        [TestCase(false, true, true, ListViewHitTestLocations.Label, false)]
        [TestCase(true, true, true, ListViewHitTestLocations.None, false)]
        public void StrictCheckboxDoubleClick_SuppressesNativeToggleOnlyForRows(
            bool checkBoxes,
            bool strictCheckboxesClick,
            bool hasItem,
            ListViewHitTestLocations location,
            bool expected)
        {
            Assert.That(
                SmartListView.ShouldSuppressNativeCheckboxDoubleClick(
                    checkBoxes,
                    strictCheckboxesClick,
                    hasItem,
                    location),
                Is.EqualTo(expected));
        }
    }
}
