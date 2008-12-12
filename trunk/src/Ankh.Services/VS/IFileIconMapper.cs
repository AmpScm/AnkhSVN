// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.VS
{
    public enum SpecialIcon
    {
        Blank = 0,
        SortUp,
        SortDown,
        Servers,
        Db,
        Server,
        Outgoing,
        Collision,
        Incoming
    }

    public enum StateIcon
    {
        Blank = 0,
        Outgoing,
        Collision,
        Incoming
    }

    public interface IFileIconMapper
    {
        int GetIcon(string path);
        ImageList ImageList { get; }

        ImageList StateImageList { get; }

        int DirectoryIcon { get; }
        int FileIcon { get; }

        int GetIconForExtension(string ext);

        int GetSpecialIcon(SpecialIcon icon);

        int GetStateIcon(StateIcon icon);
    }
}
