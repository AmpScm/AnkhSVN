// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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

    public enum WindowsSpecialFolder
    {
        Desktop = 0x0000,
        Internet = 0x0001,
        Programs = 0x0002,
        Controls = 0x0003,
        Printers = 0x0004,
        Personal = 0x0005,
        Favorites = 0x0006,
        Startup = 0x0007,
        Recent = 0x0008,
        Sendto = 0x0009,
        BitBucket = 0x000a,
        Startmenu = 0x000b,
        MyDocuments = 0x000c,
        MyMusic = 0x000d,
        MyVideo = 0x000e,
        DesktopDirectory = 0x0010,
        Drives = 0x0011,
        MyComputer=Drives,
        Network = 0x0012,
        Nethood = 0x0013,
        Fonts = 0x0014,
        Templates = 0x0015,
        CommonStartMenu = 0x0016,
        CommonPrograms = 0x0017,
        CommonStartup = 0x0018,
        CommonDesktopDirectory = 0x0019,
        AppData = 0x001a,
        PrintHood = 0x001b,
        LocalAppdata = 0x001c,
        AltStartup = 0x001d,
        CommonAltStartup = 0x001e,
        CommonFavorites = 0x001f,
        InternetCache = 0x0020,
        Cookies = 0x0021,
        History = 0x0022,
        CommonAppdata = 0x0023,
        Windows = 0x0024,
        System = 0x0025,
        ProgramFiles = 0x0026,
        MyPictures = 0x0027,
        Profile = 0x0028,
        Systemx86 = 0x0029,
        ProgramFilesX86 = 0x002a,
        ProgramFilesCommon = 0x002b,
        ProgramFilesCommonX86 = 0x002c,
        CommonTemplates = 0x002d,
        CommonDocuments = 0x002e,
        CommonAdmintools = 0x002f,
        Admintools = 0x0030,
        Connections = 0x0031,
        CommonMusic = 0x0035,
        CommonPictures = 0x0036,
        CommonVideo = 0x0037,
        Resources = 0x0038,
        ResourcesLocalized = 0x0039,
        CommonOemLinks = 0x003a,
        CDBurnArea = 0x003b,
        ComputersNearMe = 0x003d,
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
        int GetSpecialFolderIcon(Environment.SpecialFolder folder);
        int GetSpecialFolderIcon(WindowsSpecialFolder folder);
    }
}
