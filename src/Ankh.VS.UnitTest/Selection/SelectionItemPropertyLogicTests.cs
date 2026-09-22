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

using Ankh.Selection;
using Microsoft.VisualStudio.Shell.Interop;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Selection
{
    [TestFixture]
    public class SelectionItemPropertyLogicTests
    {
        [TestCase((int)__VSHPROPID.VSHPROPID_Parent, "NilItemId")]
        [TestCase((int)__VSHPROPID.VSHPROPID_FirstChild, "NilItemId")]
        [TestCase((int)__VSHPROPID.VSHPROPID_NextSibling, "NilItemId")]
        [TestCase((int)__VSHPROPID.VSHPROPID_NextVisibleSibling, "NilItemId")]
        [TestCase((int)__VSHPROPID.VSHPROPID_ParentHierarchyItemid, "NilItemId")]
        [TestCase((int)__VSHPROPID.VSHPROPID_Root, "RootItemId")]
        [TestCase((int)__VSHPROPID.VSHPROPID_TypeGuid, "TypeGuid")]
        [TestCase((int)__VSHPROPID.VSHPROPID_CmdUIGuid, "EmptyGuid")]
        [TestCase((int)__VSHPROPID.VSHPROPID_Caption, "Text")]
        [TestCase((int)__VSHPROPID.VSHPROPID_Name, "Text")]
        [TestCase((int)__VSHPROPID.VSHPROPID_TypeName, "Text")]
        [TestCase((int)__VSHPROPID.VSHPROPID_IconImgList, "ImageList")]
        [TestCase((int)__VSHPROPID.VSHPROPID_IconIndex, "ImageIndex")]
        [TestCase((int)__VSHPROPID.VSHPROPID_Expandable, "False")]
        [TestCase((int)__VSHPROPID.VSHPROPID_Expanded, "False")]
        [TestCase((int)__VSHPROPID.VSHPROPID_ExpandByDefault, "False")]
        [TestCase((int)__VSHPROPID.VSHPROPID_HasEnumerationSideEffects, "False")]
        [TestCase((int)__VSHPROPID2.VSHPROPID_Container, "False")]
        [TestCase((int)__VSHPROPID.VSHPROPID_StateIconIndex, "NoStateIcon")]
        [TestCase((int)__VSHPROPID.VSHPROPID_ParentHierarchy, "Null")]
        [TestCase((int)__VSHPROPID2.VSHPROPID_StatusBarClientText, "Null")]
        [TestCase(123456789, "Fail")]
        public void GetAction_ClassifiesHierarchyProperties(
            int propertyId,
            string expected)
        {
            Assert.That(
                SelectionItemPropertyLogic.GetAction(propertyId).ToString(),
                Is.EqualTo(expected));
        }
    }
}
