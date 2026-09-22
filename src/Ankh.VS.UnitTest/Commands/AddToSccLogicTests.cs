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

using Ankh.Commands;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Commands
{
    [TestFixture]
    public class AddToSccLogicTests
    {
        [TestCase(false, false, false, false, true)]
        [TestCase(true, true, true, false, true)]
        [TestCase(true, false, false, true, true)]
        [TestCase(true, false, false, false, false)]
        public void ShouldDisableBeforeLookup_CoversInitialGuard(
            bool solutionExists,
            bool projectCommand,
            bool emptySolution,
            bool otherProvider,
            bool expected)
        {
            Assert.That(
                AddToSccLogic.ShouldDisableBeforeLookup(
                    solutionExists,
                    projectCommand,
                    emptySolution,
                    otherProvider),
                Is.EqualTo(expected));
        }

        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        public void ShouldDisableForMissingServices_RequiresBothServices(
            bool hasScc,
            bool hasCache,
            bool expected)
        {
            Assert.That(
                AddToSccLogic.ShouldDisableForMissingServices(hasScc, hasCache),
                Is.EqualTo(expected));
        }

        [TestCase(false, false, true)]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        public void ShouldDisableSolutionCommand_RequiresUnmanagedFilename(
            bool hasFilename,
            bool managed,
            bool expected)
        {
            Assert.That(
                AddToSccLogic.ShouldDisableSolutionCommand(hasFilename, managed),
                Is.EqualTo(expected));
        }

        [TestCase(false, true, false, true)]
        [TestCase(true, false, false, true)]
        [TestCase(true, true, true, true)]
        [TestCase(true, true, false, false)]
        public void ShouldDisableSolutionItem_RequiresUsableWorkingCopyItem(
            bool exists,
            bool isFile,
            bool needsUpgrade,
            bool expected)
        {
            Assert.That(
                AddToSccLogic.ShouldDisableSolutionItem(
                    exists,
                    isFile,
                    needsUpgrade),
                Is.EqualTo(expected));
        }

        [TestCase(false, true, false, true)]
        [TestCase(false, true, true, false)]
        [TestCase(false, false, false, false)]
        [TestCase(true, true, false, false)]
        public void ShouldHideSolutionContext_OnlyForIgnoredUnversionedContextItem(
            bool versioned,
            bool ignored,
            bool solutionSelected,
            bool expected)
        {
            Assert.That(
                AddToSccLogic.ShouldHideSolutionContext(
                    versioned,
                    ignored,
                    solutionSelected),
                Is.EqualTo(expected));
        }

        [TestCase(false, true, false, false, true)]
        [TestCase(true, false, false, false, true)]
        [TestCase(true, true, true, true, true)]
        [TestCase(true, true, true, false, false)]
        [TestCase(true, true, false, true, false)]
        public void ShouldSkipProjectUpdate_HandlesBindableAndManagedStates(
            bool hasInfo,
            bool bindable,
            bool directoryVersioned,
            bool managed,
            bool expected)
        {
            Assert.That(
                AddToSccLogic.ShouldSkipProjectUpdate(
                    hasInfo,
                    bindable,
                    directoryVersioned,
                    managed),
                Is.EqualTo(expected));
        }

        [TestCase(0, true, true, false)]
        [TestCase(1, true, true, false)]
        [TestCase(2, false, true, false)]
        [TestCase(2, true, false, false)]
        [TestCase(2, true, true, true)]
        public void ShouldHideProjectContext_PreservesSelectionPassBehavior(
            int pass,
            bool hasProjectFile,
            bool ignored,
            bool expected)
        {
            Assert.That(
                AddToSccLogic.ShouldHideProjectContext(
                    pass,
                    hasProjectFile,
                    ignored),
                Is.EqualTo(expected));
        }

        [TestCase(false, false, false, false, "Skip")]
        [TestCase(false, true, false, true, "Skip")]
        [TestCase(true, true, false, false, "ManageExisting")]
        [TestCase(true, false, true, true, "Skip")]
        [TestCase(true, false, false, true, "PromptAddOrCheckout")]
        [TestCase(true, false, false, false, "Checkout")]
        public void GetProjectAction_ClassifiesProjectState(
            bool isBindable,
            bool sameWorkingCopy,
            bool isVersioned,
            bool isVersionable,
            string expected)
        {
            Assert.That(
                AddToSccLogic.GetProjectAction(
                    isBindable,
                    sameWorkingCopy,
                    isVersioned,
                    isVersionable).ToString(),
                Is.EqualTo(expected));
        }

        [TestCase(DialogResult.Yes, "AddToExisting")]
        [TestCase(DialogResult.No, "Checkout")]
        [TestCase(DialogResult.Cancel, "Cancel")]
        [TestCase(DialogResult.OK, "None")]
        [TestCase(DialogResult.None, "None")]
        public void GetPromptAction_MapsDialogResults(
            DialogResult result,
            string expected)
        {
            Assert.That(
                AddToSccLogic.GetPromptAction(result).ToString(),
                Is.EqualTo(expected));
        }
    }
}
