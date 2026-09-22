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

using Ankh.UI;
using Ankh.VS.Dialogs;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture]
    public class VSCommandRoutingLogicTests
    {
        [Test]
        public void BuildKeyPlan_RejectsMessagesOutsideKeyboardRange()
        {
            Assert.That(
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x00FF,
                    Keys.A,
                    Keys.None,
                    VSContainerMode.TranslateKeys).IsKeyboardMessage,
                Is.False);

            Assert.That(
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0110,
                    Keys.A,
                    Keys.None,
                    VSContainerMode.TranslateKeys).IsKeyboardMessage,
                Is.False);
        }

        [Test]
        public void BuildKeyPlan_AllowsOrdinaryKeyboardMessages()
        {
            VSCommandRoutingKeyPlan plan =
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0100,
                    Keys.A,
                    Keys.None,
                    VSContainerMode.TranslateKeys);

            Assert.That(plan.IsKeyboardMessage, Is.True);
            Assert.That(plan.BypassRouting, Is.False);
            Assert.That(plan.Mode, Is.EqualTo(VSContainerMode.TranslateKeys));
            Assert.That(plan.UseFilterKeys, Is.True);
            Assert.That(
                plan.TranslationFlags,
                Is.EqualTo(
                    (uint)__VSTRANSACCELEXFLAGS.VSTAEXF_AllowModalState));
        }

        [Test]
        public void BuildKeyPlan_CtrlTabBypassesOnKeyDownAndKeyUp()
        {
            Assert.That(
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0100,
                    Keys.Tab,
                    Keys.Control,
                    VSContainerMode.TranslateKeys).BypassRouting,
                Is.True);

            Assert.That(
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0101,
                    Keys.Tab,
                    Keys.Control | Keys.Shift,
                    VSContainerMode.TranslateKeys).BypassRouting,
                Is.True);

            Assert.That(
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0100,
                    Keys.Tab,
                    Keys.Shift,
                    VSContainerMode.TranslateKeys).BypassRouting,
                Is.False);
        }

        [Test]
        public void BuildKeyPlan_CtrlReturnDisablesVsTranslationOnlyForExactCtrl()
        {
            VSCommandRoutingKeyPlan ctrl =
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0100,
                    Keys.Return,
                    Keys.Control,
                    VSContainerMode.TranslateKeys
                        | VSContainerMode.UseTextEditorScope);

            Assert.That(ctrl.Mode, Is.EqualTo(VSContainerMode.Default));
            Assert.That(ctrl.UseFilterKeys, Is.False);
            Assert.That(ctrl.TranslationFlags, Is.Zero);

            VSCommandRoutingKeyPlan combined =
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0100,
                    Keys.Return,
                    Keys.Control | Keys.Shift,
                    VSContainerMode.TranslateKeys);

            Assert.That(
                combined.Mode,
                Is.EqualTo(VSContainerMode.TranslateKeys));
            Assert.That(combined.UseFilterKeys, Is.True);
        }

        [Test]
        public void BuildKeyPlan_EscapeBypassesOnKeyDownAndKeyUp()
        {
            Assert.That(
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0100,
                    Keys.Escape,
                    Keys.None,
                    VSContainerMode.TranslateKeys).BypassRouting,
                Is.True);

            Assert.That(
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0101,
                    Keys.Escape,
                    Keys.Shift,
                    VSContainerMode.TranslateKeys).BypassRouting,
                Is.True);
        }

        [Test]
        public void BuildKeyPlan_SystemF4BypassesButOtherSystemKeysDoNot()
        {
            Assert.That(
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0104,
                    Keys.F4,
                    Keys.Alt,
                    VSContainerMode.TranslateKeys).BypassRouting,
                Is.True);

            Assert.That(
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0105,
                    Keys.F4,
                    Keys.None,
                    VSContainerMode.TranslateKeys).BypassRouting,
                Is.True);

            Assert.That(
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0104,
                    Keys.F5,
                    Keys.Alt,
                    VSContainerMode.TranslateKeys).BypassRouting,
                Is.False);
        }

        [Test]
        public void BuildKeyPlan_UsesTextEditorScopeFlagWhenRequested()
        {
            VSCommandRoutingKeyPlan plan =
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0102,
                    Keys.None,
                    Keys.None,
                    VSContainerMode.UseTextEditorScope);

            uint expected =
                (uint)__VSTRANSACCELEXFLAGS.VSTAEXF_AllowModalState
                | (uint)__VSTRANSACCELEXFLAGS.VSTAEXF_UseTextEditorKBScope;

            Assert.That(plan.UseFilterKeys, Is.True);
            Assert.That(plan.TranslationFlags, Is.EqualTo(expected));
        }

        [Test]
        public void BuildKeyPlan_DefaultModeSkipsFilterKeys()
        {
            VSCommandRoutingKeyPlan plan =
                VSCommandRoutingLogic.BuildKeyPlan(
                    0x0102,
                    Keys.None,
                    Keys.None,
                    VSContainerMode.Default);

            Assert.That(plan.UseFilterKeys, Is.False);
            Assert.That(plan.TranslationFlags, Is.Zero);
        }

        [Test]
        public void ShouldConsumeTranslatedCommand_RequiresSuccessAndTranslation()
        {
            Assert.That(
                VSCommandRoutingLogic.ShouldConsumeTranslatedCommand(
                    VSConstants.S_OK,
                    1),
                Is.True);

            Assert.That(
                VSCommandRoutingLogic.ShouldConsumeTranslatedCommand(
                    VSConstants.S_OK,
                    0),
                Is.False);

            Assert.That(
                VSCommandRoutingLogic.ShouldConsumeTranslatedCommand(
                    VSConstants.S_FALSE,
                    1),
                Is.False);
        }
    }
}
