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

using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Selection
{
    internal enum SelectionItemPropertyAction
    {
        Fail,
        NilItemId,
        RootItemId,
        TypeGuid,
        EmptyGuid,
        Text,
        ImageList,
        ImageIndex,
        False,
        NoStateIcon,
        Null
    }

    internal static class SelectionItemPropertyLogic
    {
        public static SelectionItemPropertyAction GetAction(int propid)
        {
            switch ((__VSHPROPID)propid)
            {
                case __VSHPROPID.VSHPROPID_Parent:
                case __VSHPROPID.VSHPROPID_FirstChild:
                case __VSHPROPID.VSHPROPID_NextSibling:
                case __VSHPROPID.VSHPROPID_NextVisibleSibling:
                case __VSHPROPID.VSHPROPID_ParentHierarchyItemid:
                    return SelectionItemPropertyAction.NilItemId;

                case __VSHPROPID.VSHPROPID_Root:
                    return SelectionItemPropertyAction.RootItemId;

                case __VSHPROPID.VSHPROPID_TypeGuid:
                    return SelectionItemPropertyAction.TypeGuid;

                case __VSHPROPID.VSHPROPID_CmdUIGuid:
                    return SelectionItemPropertyAction.EmptyGuid;

                case __VSHPROPID.VSHPROPID_Caption:
                case __VSHPROPID.VSHPROPID_Name:
                case __VSHPROPID.VSHPROPID_TypeName:
                    return SelectionItemPropertyAction.Text;

                case __VSHPROPID.VSHPROPID_IconImgList:
                    return SelectionItemPropertyAction.ImageList;

                case __VSHPROPID.VSHPROPID_IconIndex:
                    return SelectionItemPropertyAction.ImageIndex;

                case __VSHPROPID.VSHPROPID_Expandable:
                case __VSHPROPID.VSHPROPID_Expanded:
                case __VSHPROPID.VSHPROPID_ExpandByDefault:
                case __VSHPROPID.VSHPROPID_HasEnumerationSideEffects:
                case (__VSHPROPID)__VSHPROPID2.VSHPROPID_Container:
                    return SelectionItemPropertyAction.False;

                case __VSHPROPID.VSHPROPID_StateIconIndex:
                    return SelectionItemPropertyAction.NoStateIcon;

                case __VSHPROPID.VSHPROPID_ParentHierarchy:
                case (__VSHPROPID)__VSHPROPID2.VSHPROPID_StatusBarClientText:
                    return SelectionItemPropertyAction.Null;

                default:
                    return SelectionItemPropertyAction.Fail;
            }
        }
    }
}
