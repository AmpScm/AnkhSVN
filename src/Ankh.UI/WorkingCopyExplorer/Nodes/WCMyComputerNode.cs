// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using Ankh.VS;
using Ankh.Scc;
using System.Runtime.InteropServices;

namespace Ankh.UI.WorkingCopyExplorer.Nodes
{
    class WCMyComputerNode : WCTreeNode
    {
        readonly int _imageIndex;
        public WCMyComputerNode(IAnkhServiceProvider context)
            : base(context, null)
        {
            _imageIndex = context.GetService<IFileIconMapper>().GetSpecialFolderIcon(WindowsSpecialFolder.MyComputer);
        }

        public override string Title
        {
            get { return "My Computer"; }
        }

        public override void GetResources(System.Collections.ObjectModel.Collection<SvnItem> list, bool getChildItems, Predicate<SvnItem> filter)
        {
        }

        public override IEnumerable<WCTreeNode> GetChildren()
        {
            IFileStatusCache cache = Context.GetService<IFileStatusCache>();

            foreach (string s in Environment.GetLogicalDrives())
            {
                switch (NativeMethods.GetDriveType(s))
                {
                    case NativeMethods.DriveType.Removable: // We should filter floppies.
                    case NativeMethods.DriveType.Fixed:
                    case NativeMethods.DriveType.Remote:
                    case NativeMethods.DriveType.RAMDisk:
                        yield return new WCDirectoryNode(Context, this, cache[s]);
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void RefreshCore(bool rescan)
        {
        }

        public override int ImageIndex
        {
            get { return _imageIndex; }
        }

        static class NativeMethods
        {
            public enum DriveType : uint
            {
                /// <summary>The drive type cannot be determined.</summary>
                Unknown = 0,    //DRIVE_UNKNOWN
                /// <summary>The root path is invalid, for example, no volume is mounted at the path.</summary>
                Error = 1,        //DRIVE_NO_ROOT_DIR
                /// <summary>The drive is a type that has removable media, for example, a floppy drive or removable hard disk.</summary>
                Removable = 2,    //DRIVE_REMOVABLE
                /// <summary>The drive is a type that cannot be removed, for example, a fixed hard drive.</summary>
                Fixed = 3,        //DRIVE_FIXED
                /// <summary>The drive is a remote (network) drive.</summary>
                Remote = 4,        //DRIVE_REMOTE
                /// <summary>The drive is a CD-ROM drive.</summary>
                CDROM = 5,        //DRIVE_CDROM
                /// <summary>The drive is a RAM disk.</summary>
                RAMDisk = 6        //DRIVE_RAMDISK
            }

            [DllImport("kernel32.dll")]
            public static extern DriveType GetDriveType([MarshalAs(UnmanagedType.LPStr)] string lpRootPathName);
        }
    }
}
